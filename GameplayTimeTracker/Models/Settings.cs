using System.ComponentModel.DataAnnotations;

namespace GameplayTimeTracker.Models;

public class Settings : BaseDataModel
{
    public int ThemeId { get; set; } = 0;
    public int RemoteMachineId { get; set; } = 0;
    public string ProfileName { get; set; } = "";
    public bool StartWithSystem { get; set; } = true;
    public bool StartMinimized { get; set; } = false;
    public string SGDBApiKey { get; set; } = "";
    public bool PreferSGDBImages { get; set; } = false;
    public bool QuickAdd { get; set; } = false;
    public bool BackupOnExit { get; set; } = false;
    public DisplayType DpType { get; set; } = DisplayType.Horizontal;
    public int SavingFreq { get; set; } = 5;
    public bool RemoteSavingEnabled { get; set; } = false;
}