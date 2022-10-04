using RobotCommando.Models.Items;
using RobotCommando.Models.Mechas;

namespace RobotCommando.Models.Battling;
public abstract class Battle<T> where T : Battler
{
    public BattlePhase Phase { get; protected set; }
    public Die Die { get; init; } = null!;
    public T Player { get; init; } = null!;
    public T Enemy { get; init; } = null!;
    public BattleResult PossibleResult { get; init; } = null!;
    
    public T This { get; protected set; } = null!;
    public T Other { get; protected set; } = null!;
    public int? Result { get; protected set; }
    
    public abstract bool CanEscape { get; }
    public bool AttemptEscape { get; set; }

    public abstract bool MoveNext();
}
