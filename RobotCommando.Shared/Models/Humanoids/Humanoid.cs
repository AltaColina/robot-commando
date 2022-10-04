namespace RobotCommando.Models.Humanoids;

public abstract class Humanoid : Entity
{
    public int StaminaMax { get; init; }
    public int Stamina { get; set; }
    public int SkillMax { get; init; }
    public int Skill { get; set; }
}
