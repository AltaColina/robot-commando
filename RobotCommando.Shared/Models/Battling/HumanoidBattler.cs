using RobotCommando.Models.Humanoids;

namespace RobotCommando.Models.Battling;

public sealed class HumanoidBattler : Battler
{
    public int StaminaMax { get; init; }
    public int Stamina { get; set; }

    public int SkillMax { get; init; }
    public int Skill { get; set; }

    public int LuckMax { get; init; }
    public int Luck { get; set; }

    public HumanoidBattler(Player player)
    {
        Name = player.Name;
        Description = player.Description;
        Icon = player.Icon;
        StaminaMax = player.StaminaMax;
        Stamina = player.Stamina;
        SkillMax = player.SkillMax;
        Skill = player.Skill;
        LuckMax = player.LuckMax;
        Luck = player.Luck;
    }

    public HumanoidBattler(Enemy enemy)
    {
        Name = enemy.Name;
        Description = enemy.Description;
        Icon = enemy.Icon;
        StaminaMax = enemy.StaminaMax;
        Stamina = enemy.Stamina;
        SkillMax = enemy.SkillMax;
        Skill = enemy.Skill;
        LuckMax = -1;
        Luck = -1;
    }
}
