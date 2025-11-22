namespace GameplayTimeTracker.Models;

public class Settings : BaseDataModel
{
    public bool StartWithSystem { get; set; } = true;
    public bool StartMinimized { get; set; } = false;
}