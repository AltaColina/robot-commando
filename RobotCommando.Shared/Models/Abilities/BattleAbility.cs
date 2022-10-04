using RobotCommando.Models.Battling;
using RobotCommando.Models.Expressions;
using System.Diagnostics;

namespace RobotCommando.Models.Abilities;

public abstract class BattleAbility : Ability
{
    public Dictionary<BattlePhase, Effect<MechaBattle>> Effects { get; init; } = null!;

    public override Task CompileAsync() => Task.WhenAll(Effects.Select(e => e.Value.CompileAsync()));

    public virtual bool TryUse(MechaBattle battle)
    {
        if (Effects.TryGetValue(battle.Phase, out var effect))
        {
            effect.Execute(battle);
            Debug.WriteLine($"'{battle.This.Name}'s {Name}' effect: {effect.ExpressionText}");
            return true;
        }
        return false;
    }
}
