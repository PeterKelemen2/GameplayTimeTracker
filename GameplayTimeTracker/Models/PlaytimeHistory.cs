using System;

namespace GameplayTimeTracker.Models;

public class PlaytimeHistory : BaseDataModel
{
    public int GameId { get; set; }

    public DateTime Date { get; set; }
    public int TotalSeconds { get; set; }

    public override string ToString()
    {
        TimeSpan span = TimeSpan.FromSeconds(TotalSeconds);
        return
            $"PT History [Game: {GameId}] | On {Date.Date} | {(int)span.TotalHours:D2}h {span.Minutes:D2}m {span.Seconds:D2}s";
    }
}