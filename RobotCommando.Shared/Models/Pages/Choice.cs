using RobotCommando.Models.Expressions;

namespace RobotCommando.Models.Pages;

public sealed class Choice
{
    public string Text { get; init; } = null!;
    public int Link { get; set; }
    public Condition<Game>? Condition { get; init; }
    public Effect<Game>? Effect { get; init; }
    public bool VisibleWhenDisabled { get; init; }
}
