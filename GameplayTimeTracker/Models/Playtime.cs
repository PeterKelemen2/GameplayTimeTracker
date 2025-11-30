using System;

namespace GameplayTimeTracker.Models;

public class Playtime : BaseDataModel
{
    public int GameId { get; set; }
    public int TotalSeconds { get; set; }
    public int LastTotalSeconds { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public override string ToString()
    {
        TimeSpan span = TimeSpan.FromSeconds(TotalSeconds);
        return $"Game: {GameId}, {(int)span.TotalHours:D2}h {span.Minutes:D2}m {span.Seconds:D2}s";
    }
}