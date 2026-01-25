namespace GameplayTimeTracker.Models;

public class Game : BaseDataModel
{
    public string DisplayName { get; set; } = "";
    public string ExePath { get; set; } = "";
    public string IconPath { get; set; } = "";
    public string HeroPath { get; set; } = "";
    
    public override string ToString()
    {
        return $"ID: {Id}\n" +
               $"Name: {DisplayName}\n" +
               $"Executable: {ExePath}\n";
    }
}