using RobotCommando.Models.Battling;

namespace RobotCommando.Models.Humanoids;

public sealed class Enemy : Humanoid
{
    public BattleResult BattleResult { get; init; } = new();
}
