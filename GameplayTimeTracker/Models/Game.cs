using System.ComponentModel.DataAnnotations.Schema;

namespace GameplayTimeTracker.Models;

public class Game : BaseDataModel
{
    public int? LastPlaytimeId { get; set; }
    public int TotalPlaytimeId { get; set; }

    [NotMapped] public Playtime LastPlaytime { get; set; } = new Playtime();
    [NotMapped] public Playtime TotalPlaytime { get; set; } = new Playtime();

    public string Name { get; set; } = "";
}