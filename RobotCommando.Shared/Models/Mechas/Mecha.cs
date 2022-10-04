using RobotCommando.Models.Abilities;

namespace RobotCommando.Models.Mechas;

public abstract class Mecha : Entity
{
    public MechaTypes Types { get; init; }
    public int ArmorMax { get; init; }
    public int Armor { get; set; }
    public MechaSpeed SpeedMax { get; init; }
    public MechaSpeed Speed { get; set; }
    public List<Ability> Abilities { get; init; } = new();
}
