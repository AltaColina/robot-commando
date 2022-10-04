using System.Diagnostics.CodeAnalysis;

namespace RobotCommando.Models;

public abstract class Entity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Tag { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string Icon { get; init; } = null!;
}
