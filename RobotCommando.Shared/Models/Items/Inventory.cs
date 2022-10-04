using RobotCommando.Messaging;
using System.Collections;
using System.Diagnostics;

namespace RobotCommando.Models.Items;

public sealed class Inventory : Entity, IReadOnlyList<ItemSlot>
{
    private const int InventorySize = 16;

    private readonly ItemSlot[] _slots = Enumerable.Range(0, InventorySize).Select(i => new ItemSlot(new List<Item>())).ToArray();
    private readonly Dictionary<string, int> _indices = new(InventorySize);

    public ItemSlot this[int index] { get => _slots[index]; }

    public int Count { get => InventorySize; }

    public event EventHandler<EventArgs<Item>>? ItemAdded;
    public event EventHandler<EventArgs<Item>>? ItemRemoved;
    public event EventHandler<EventArgs<(int index1, int index2)>>? ItemMoved;

    private int FindFreeSlotIndex()
    {
        var index = -1;
        for (int i = 0; i < _slots.Length; ++i)
        {
            if (_slots[i].Items.Count == 0)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    public void Add(Item item)
    {
        if (!_indices.TryGetValue(item.Tag, out var index))
        {
            index = FindFreeSlotIndex();
            if (index < 0)
                throw new InvalidOperationException("Inventory size exceeded");
            _indices[item.Tag] = index;
        }
        _slots[index].Items.Add(item);
        ItemAdded?.Invoke(this, new EventArgs<Item>(item));
    }

    public void Remove(Item item)
    {
        if (_indices.TryGetValue(item.Tag, out var index))
        {
            var items = _slots[index].Items;
            var count = items.RemoveAll(i => i.Id == item.Id);
            if (count != 1)
                throw new InvalidOperationException("Duplicated item id");
            if (items.Count == 0)
                _indices.Remove(item.Tag);
            ItemRemoved?.Invoke(this, new EventArgs<Item>(item));
        }
    }

    public bool Contains(Item item) => _indices.ContainsKey(item.Tag);

    public bool Contains(string tag) => _indices.ContainsKey(tag);

    public void Swap(int i1, int i2)
    {
        (_slots[i1], _slots[i2]) = (_slots[i2], _slots[i1]);
        ItemMoved?.Invoke(this, new EventArgs<(int index1, int index2)>((i1, i2)));
    }

    public IEnumerator<ItemSlot> GetEnumerator() => ((IEnumerable<ItemSlot>)_slots).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _slots.GetEnumerator();
}