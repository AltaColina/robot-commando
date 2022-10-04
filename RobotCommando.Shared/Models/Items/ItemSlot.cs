namespace RobotCommando.Models.Items;

public readonly struct ItemSlot
{
    public List<Item> Items { get; }

    public int Count { get => Items?.Count ?? 0; }

    public ItemSlot(List<Item> items) => Items = items;
}
