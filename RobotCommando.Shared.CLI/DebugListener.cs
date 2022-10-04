using System.Diagnostics;

namespace RobotCommando.Shared.CLI;
internal sealed class DebugListener : TraceListener
{
    public override void Write(string? message) => Console.Write(message);
    public override void WriteLine(string? message) => Console.WriteLine(message);
}
