namespace RobotCommando.Models.Mechas;

public sealed class Robot : Mecha
{
    public int BonusMax { get; init; }
    public int Bonus { get; set; }

    public Robot() => Tag = nameof(Robot);
}
