using RobotCommando.Models.Battling;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace RobotCommando.Models.Abilities;

public sealed class ActiveAbility : BattleAbility
{
    [JsonIgnore]
    public bool IsActive { get; private set; }
    public int Usages { get; set; }

    public ActiveAbility() => Tag = nameof(ActiveAbility);

    public bool TryActivate(MechaBattle battle)
    {
        if (Usages == -1)
            IsActive = true;
        else if (Usages > 0)
        {
            Usages--;
            IsActive = true;
        }
        Debug.WriteLineIf(IsActive, $"Activated '{battle.This.Name}'s {Name}'");
        return IsActive;
    }

    public bool TryDeactivate(MechaBattle battle)
    {
        if (IsActive)
        {
            IsActive = false;
            Debug.WriteLine($"Deactivated '{battle.This.Name}'s {Name}'");
            return true;
        }
        return false;
    }

    public override bool TryUse(MechaBattle battle)
    {
        var result = false;
        if (IsActive)
        {
            result = base.TryUse(battle);
            if (result && Usages > 0)
                Usages--;
        }
        return result;
    }
}