namespace RobotCommando.Models.Humanoids;

public sealed class Player : Humanoid
{
    public int LuckMax { get; init; }
    public int Luck { get; set; }
    public int RobotSkill { get; set; }
}
