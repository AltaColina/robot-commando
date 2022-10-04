namespace RobotCommando.Models.Items;

public sealed class Die
{
    public int Sides { get; init; }

	public int Roll() => Random.Shared.Next(maxValue: Sides) + 1;

	public int Roll(int times) => Random.Shared.Next(times, times * Sides + 1);
}