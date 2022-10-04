using RobotCommando.Models.Battling;
using RobotCommando.Models.Expressions;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace RobotCommando.Models.Abilities;

public sealed class TriggerAbility : BattleAbility
{
    [JsonIgnore]
    public bool IsActive { get; private set; }

    public Condition<MechaBattle> Condition { get; init; } = null!;
    public BattlePhase Phase { get; init; }

    public TriggerAbility() => Tag = nameof(TriggerAbility);

    public override Task CompileAsync() => Task.WhenAll(Condition.CompileAsync(), base.CompileAsync());

    public bool TryActivate(MechaBattle battle)
    {
        if (Phase != battle.Phase)
            return false;
        IsActive = Condition.IsTrue(battle);
        Debug.WriteLineIf(IsActive, $"Activated '{Name}'");
        return IsActive;
    }

    public override bool TryUse(MechaBattle battle) => IsActive && base.TryUse(battle);
}
