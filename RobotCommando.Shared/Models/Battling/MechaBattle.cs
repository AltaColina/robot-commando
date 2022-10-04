using RobotCommando.Models.Items;
using System.Diagnostics;

namespace RobotCommando.Models.Battling;

public sealed class MechaBattle : Battle<MechaBattler>
{
    public override bool CanEscape { get => Phase < BattlePhase.BeforeRoll && Player.Speed > Enemy.Speed && PossibleResult.Escape.HasValue; }

    private void TriggerAbilities()
    {
        (This, Other) = (Player, Enemy);
        This.ActivateAbilities(this);
        (This, Other) = (Enemy, Player);
        This.ActivateAbilities(this);
    }

    private void UseAbilities()
    {
        (This, Other) = (Player, Enemy);
        This.UseAbilities(this);
        (This, Other) = (Enemy, Player);
        This.UseAbilities(this);
    }

    private void DeactivateAbilities()
    {
        (This, Other) = (Player, Enemy);
        This.DeactivateAbilities(this);
        (This, Other) = (Enemy, Player);
        This.DeactivateAbilities(this);
    }

    public override bool MoveNext()
    {
        if (Phase != BattlePhase.Roll && Phase != BattlePhase.Hit)
        {
            TriggerAbilities();
            UseAbilities();
        }

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
                if (Player.Speed != Enemy.Speed)
                {
                    if (Player.Speed > Enemy.Speed)
                        Player.Roll += 1;
                    else
                        Enemy.Roll += 1;
                }
                Debug.WriteLine($"'{Player.Name}' rolled {Player.Roll}");
                Debug.WriteLine($"'{Enemy.Name}' rolled {Enemy.Roll}");
                break;

            // Do damage.
            case BattlePhase.Hit when Player.Roll != Enemy.Roll:
                if (Player.Roll > Enemy.Roll)
                {
                    Enemy.Armor -= Player.Damage;
                    Debug.WriteLine($"'{Player.Name}' deals {Player.Damage} damage to '{Enemy.Name}' ({Enemy.Armor + Player.Damage} -> {Enemy.Armor})");
                }
                else
                {
                    Player.Armor -= Enemy.Damage;
                    Debug.WriteLine($"'{Player.Name}' takes {Enemy.Damage} damage ({Player.Armor + Enemy.Damage} -> {Player.Armor})");
                }
                break;

            // Check escape.
            case BattlePhase.AfterRound when AttemptEscape:
                DeactivateAbilities();
                Player.Armor -= 2;
                Debug.WriteLine($"'{Player.Name}' escaped and took 2 damage ({Player.Armor + 2} -> {Player.Armor})");
                break;

            case BattlePhase.AfterRound:
                DeactivateAbilities();
                break;

            // Check winner.
            case BattlePhase.AfterBattle:
                Debug.WriteLineIf(Player.Armor <= 0, $"'{Player.Name}' loses.");
                Debug.WriteLineIf(Enemy.Armor <= 0, $"'{Enemy.Name}' loses.");
                return false;
        }

        // Select next phase.
        if (Player.Armor <= 0)
        {
            Phase = BattlePhase.AfterBattle;
            Result = PossibleResult.Win;
            Debug.WriteLine($"'{Player.Name}' was destroyed");
        }
        else if (Enemy.Armor <= 0)
        {
            Phase = BattlePhase.AfterBattle;
            Result = PossibleResult.Lose;
            Debug.WriteLine($"'{Enemy.Name}' was destroyed");
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
