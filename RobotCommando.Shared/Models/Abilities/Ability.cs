using System.Diagnostics;

namespace RobotCommando.Models.Abilities;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public abstract class Ability : Entity
{
    public abstract Task CompileAsync();

    private string GetDebuggerDisplay() => Name;
}
