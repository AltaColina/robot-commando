using RobotCommando.Models.Abilities;
using RobotCommando.Models.Humanoids;
using RobotCommando.Models.Mechas;

namespace RobotCommando.Models.Battling;

public sealed class MechaBattler : Battler
{
    public MechaType Type { get; init; }

    public int ArmorMax { get; init; }
    public int Armor { get; set; }

    public int SkillMax { get; init; }
    public int Skill { get; set; }
    
    public MechaSpeed SpeedMax { get; init; }
    public MechaSpeed Speed { get; set; }

    public List<Ability> Abilities { get; init; } = null!;

    public HashSet<MechaBattlerStatus> Statuses { get; } = new(EntityTagComparer.Default);

    public MechaBattler(Robot robot, Player player)
    {
        Name = robot.Name;
        Description = robot.Description;
        Icon = robot.Icon;
        Type = robot.Type;
        ArmorMax = robot.ArmorMax;
        Armor = robot.Armor;
        SkillMax = player.SkillMax + player.RobotSkill + robot.BonusMax;
        Skill = player.Skill + robot.Bonus;
        SpeedMax = robot.SpeedMax;
        Speed = robot.Speed;
        Abilities = robot.Abilities;
    }

    public MechaBattler(Monster monster)
    {
        Name = monster.Name;
        Description = monster.Description;
        Icon = monster.Icon;
        Type = monster.Type;
        ArmorMax = monster.ArmorMax;
        Armor = monster.Armor;
        SkillMax = monster.SkillMax;
        Skill = monster.Skill;
        SpeedMax = monster.SpeedMax;
        Speed = monster.Speed;
        Abilities = monster.Abilities;
    }

    public bool ActivateAbilities(MechaBattle battle)
    {
        var result = false;
        foreach (var ability in Abilities.OfType<TriggerAbility>())
            result |= ability.TryActivate(battle);
        return result;
    }

    public bool UseAbilities(MechaBattle battle)
    {
        var result = false;
        foreach (var ability in Abilities.OfType<BattleAbility>())
            result |= ability.TryUse(battle);
        return result;
    }

    public bool DeactivateAbilities(MechaBattle battle)
    {
        var result = false;
        foreach (var ability in Abilities.OfType<ActiveAbility>())
            result |= ability.TryDeactivate(battle);
        return result;
    }
}