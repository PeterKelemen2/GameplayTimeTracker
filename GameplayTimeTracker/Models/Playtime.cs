using System;

namespace GameplayTimeTracker.Models;

public class Playtime : BaseDataModel
{
    public int GameId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    public virtual Game Game { get; set; }

    public override string ToString() => $"Playtime: GameId={GameId}, StartDate={StartDate}, EndDate={EndDate}";
}