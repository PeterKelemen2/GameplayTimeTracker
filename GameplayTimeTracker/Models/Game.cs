using System.ComponentModel.DataAnnotations.Schema;

namespace GameplayTimeTracker.Models;

public class Game : BaseDataModel
{
    public int? LastPlaytimeId { get; set; }
    public int TotalPlaytimeId { get; set; }

    public Playtime LastPlaytime { get; set; } = new Playtime();
    public Playtime TotalPlaytime { get; set; } = new Playtime();

    public string Name { get; set; } = "";
}