using RobotCommando.Models.Items;
using System.Diagnostics;

namespace RobotCommando.Models.Battling;

public sealed class HumanoidBattle : Battle<HumanoidBattler>
{
    public override bool CanEscape { get => Phase < BattlePhase.BeforeRoll && PossibleResult.Escape.HasValue; }

    public override bool MoveNext()
    {
        // End?
        if (Phase == BattlePhase.AfterBattle)
            return false;

        // Escape?
        if (CanEscape && AttemptEscape)
        {
            Phase = BattlePhase.AfterRound;
            return true;
        }

        // Process phase, if needed.
        switch (Phase)
        {
            // Calculate rolls.
            case BattlePhase.Roll:
                Player.Roll = Player.Skill + Die.Roll(times: 2);
                Enemy.Roll = Enemy.Skill + Die.Roll(times: 2);
                Debug.WriteLine($"'{Player.Name}' rolled {Player.Roll}");
                Debug.WriteLine($"'{Enemy.Name}' rolled {Enemy.Roll}");
                break;

            // Do damage.
            case BattlePhase.Hit when Player.Roll != Enemy.Roll:
                if (Player.Roll > Enemy.Roll)
                {
                    Enemy.Stamina -= Player.Damage;
                    Debug.WriteLine($"'{Player.Name}' deals {Player.Damage} damage to '{Enemy.Name}' ({Enemy.Stamina + Player.Damage} -> {Enemy.Stamina})");
                }
                else
                {
                    Player.Stamina -= Enemy.Damage;
                    Debug.WriteLine($"'{Player.Name}' takes {Enemy.Damage} damage ({Player.Stamina + Enemy.Damage} -> {Player.Stamina})");
                }
                break;

            // Check escape.
            case BattlePhase.AfterRound when AttemptEscape:
                Player.Stamina -= 2;
                Debug.WriteLine($"'{Player.Name}' escaped and took 2 damage ({Player.Stamina + 2} -> {Player.Stamina})");
                break;

            // Check winner.
            case BattlePhase.AfterBattle:
                Debug.WriteLineIf(Player.Stamina <= 0, $"'{Player.Name}' loses.");
                Debug.WriteLineIf(Enemy.Stamina <= 0, $"'{Enemy.Name}' loses.");
                return false;
        }

        // Select next phase.
        if (Player.Stamina <= 0)
        {
            Phase = BattlePhase.AfterBattle;
            Result = PossibleResult.Win;
            Debug.WriteLine($"'{Player.Name}' was defeated");
        }
        else if (Enemy.Stamina <= 0)
        {
            Phase = BattlePhase.AfterBattle;
            Result = PossibleResult.Lose;
            Debug.WriteLine($"'{Enemy.Name}' was defeated");
        }
        else
        {
            Phase = (BattlePhase)((((int)Phase) + 1) % ((int)BattlePhase.AfterRound + 1));
            if (Phase == BattlePhase.BeforeBattle)
                Phase = BattlePhase.BeforeRound;
        }

        return true;
    }
}
