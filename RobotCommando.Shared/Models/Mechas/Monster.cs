namespace RobotCommando.Models.Mechas;

public sealed class Monster : Mecha
{
    public int SkillMax { get; init; }
    public int Skill { get; set; }
    public BattleResult BattleResult { get; init; } = new();

    public Monster() => Tag = nameof(Monster);
}