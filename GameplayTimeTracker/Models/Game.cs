using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameplayTimeTracker.Models;

public class Game : BaseDataModel
{
    public string DisplayName { get; set; } = "";
    public string ExePath { get; set; } = "";
    public string IconPath { get; set; } = "";
    public string HeroPath { get; set; } = "";

    [NotMapped] public bool IsTracked { get; set; }
    [NotMapped] public DateTime StartTime { get; set; }
    [NotMapped] public DateTime EndTime { get; set; }

    public override string ToString()
    {
        return $"ID: {Id}\n" +
               $"Name: {DisplayName}\n" +
               $"Executable: {ExePath}\n";
    }
}