using System.ComponentModel.DataAnnotations;

namespace GameplayTimeTracker.Models;

public class Settings : BaseDataModel
{
    public int ThemeId { get; set; } = 0;
    public int RemoteMachineId { get; set; } = 0;

    [Display(Name = "Start With The System")] public bool StartWithSystem { get; set; } = true;

    [Display(Name = "Start Minimized")] public bool StartMinimized { get; set; } = false;

    [Display(Name = "SteamGridDB API Key")] public string SGDBApiKey { get; set; } = "";

    [Display(Name = "Prefer SteamGridDB Images")] public bool PreferSGDBImages { get; set; } = false;

    [Display(Name = "Quick Add")] public bool QuickAdd { get; set; } = false;

    [Display(Name = "Backup On Exit")] public bool BackupOnExit { get; set; } = false;

    [Display(Name = "Display Type")] public DisplayType DpType { get; set; } = DisplayType.Horizontal;

    [Display(Name = "Saving Frequency")] public int SavingFreq { get; set; } = 5;

    [Display(Name = "Remote Saving Enabled")] public bool RemoteSavingEnabled { get; set; } = false;
}