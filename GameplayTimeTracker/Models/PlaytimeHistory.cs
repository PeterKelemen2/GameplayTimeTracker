using System;

namespace GameplayTimeTracker.Models;

public class PlaytimeHistory : BaseDataModel
{
    public int GameId { get; set; }
    public DateTime Date { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }

    public override string ToString()
    {
        return $"PT History [{GameId}] | On {Date.Date} | {Hours}h {Minutes}m {Seconds}s ";
    }
}