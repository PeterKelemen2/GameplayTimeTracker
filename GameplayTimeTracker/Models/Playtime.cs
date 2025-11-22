using System;

namespace GameplayTimeTracker.Models;

public class Playtime : BaseDataModel
{
    public int GameId { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public override string ToString()
    {
        return $"Game: {GameId}, {Hours}h {Minutes}m {Seconds}s";
    }
}