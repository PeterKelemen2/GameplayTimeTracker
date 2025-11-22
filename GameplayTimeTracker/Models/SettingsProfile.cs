namespace GameplayTimeTracker.Models;

public class SettingsProfile : BaseDataModel
{
    public int SettingsId { get; set; }
    public string ProfileName { get; set; } = "";
}