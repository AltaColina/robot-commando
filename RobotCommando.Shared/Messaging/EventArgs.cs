namespace RobotCommando.Messaging;
public sealed class EventArgs<T> : EventArgs
{
    public T Data { get; init; } = default!;

	public EventArgs(T data) => Data = data;
}
