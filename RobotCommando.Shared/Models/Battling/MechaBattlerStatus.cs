namespace RobotCommando.Models.Battling;

public sealed class MechaBattlerStatus : Entity
{
    public int Duration { get; set; }

    private MechaBattlerStatus() => Tag = nameof(MechaBattlerStatus);

    public static MechaBattlerStatus Accelerated
    {
        get => new()
        {
            Name = "Accelerated",
            Description = "Automatically win the next round",
            Icon = "",
            Duration = 2
        };
    }

    public static MechaBattlerStatus Disarmed
    {
        get => new()
        {
            Name = "Disarmed",
            Description = "Cannot attack.",
            Icon = "",
            Duration = -1
        };
    }

    public static MechaBattlerStatus DoubleDamage
    {
        get => new()
        {
            Name = "Double Damage",
            Description = "Deals double damage on hit.",
            Icon = "",
            Duration = -1
        };
    }

    public static MechaBattlerStatus Entangled
    {
        get => new()
        {
            Name = "Entangled",
            Description = "Taking 1 damage at the end of each round.",
            Icon = "",
            Duration = -1
        };
    }

    public static MechaBattlerStatus Flying
    {
        get => new()
        {
            Name = "Flying",
            Description = "Flying.",
            Icon = "",
            Duration = -1
        };
    }

    public static MechaBattlerStatus Invencible
    {
        get => new()
        {
            Name = "Invencible",
            Description = "Takes no damage",
            Icon = "",
            Duration = 1
        };
    }

    public static MechaBattlerStatus KnockedDown
    {
        get => new()
        {
            Name = "KnockedDown",
            Description = "Deals no damage.",
            Icon = "",
            Duration = 2
        };
    }
}