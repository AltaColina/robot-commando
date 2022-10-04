using System.Diagnostics.CodeAnalysis;

namespace RobotCommando.Models;

public sealed class EntityTagComparer : IEqualityComparer<Entity>
{
    public static EntityTagComparer Default { get; } = new();

    public bool Equals(Entity? x, Entity? y) => x is null ? y is null : x.Tag.Equals(y?.Tag);
    public int GetHashCode([DisallowNull] Entity obj) => obj.Tag.GetHashCode();
}
