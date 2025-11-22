using System;

namespace GameplayTimeTracker.Models;

public class PlaytimeHistory : BaseDataModel
{
    public int GameId { get; set; }
    public DateTime Date { get; set; }
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
}