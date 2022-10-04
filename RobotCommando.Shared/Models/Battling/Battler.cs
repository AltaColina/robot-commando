namespace RobotCommando.Models.Battling;

public abstract class Battler : Entity
{
    public int Roll { get; set; }
    public int Damage { get; set; } = 2;

    protected Battler() => Tag = nameof(Battler);
}
