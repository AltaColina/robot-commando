using System.Diagnostics;

namespace RobotCommando.Models.Items;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public sealed class Item : Entity
{
    public ItemTrigger? OnAcquire { get; init; }
    public ItemTrigger? OnDiscard { get; init; }
    public ItemTrigger? OnUse { get; init; }

    private string GetDebuggerDisplay() => Name;
}
