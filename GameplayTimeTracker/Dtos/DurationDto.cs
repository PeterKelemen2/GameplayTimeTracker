namespace GameplayTimeTracker.Dtos;

public class DurationDto(int hours, int minutes, int seconds)
{
    public int Hours { get; init; } = hours;
    public int Minutes { get; init; } = minutes;
    public int Seconds { get; init; } = seconds;
}