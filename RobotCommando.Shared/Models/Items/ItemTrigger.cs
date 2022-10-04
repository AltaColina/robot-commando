using RobotCommando.Models.Expressions;

namespace RobotCommando.Models.Items;

public sealed class ItemTrigger
{
    public Condition<Game>? Condition { get; init; }
    public Effect<Game>? Effect { get; init; }
}
