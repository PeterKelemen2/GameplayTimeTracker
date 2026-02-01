using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.ViewModels;

public class SettingsViewModel : Settings
{
    public string ProfileNameLabel => "Profile Name";
    public string StartWithSystemLabel => "Start With System";
    public string StartMinimizedLabel => "Start Minimized";
    public string SGDBApiKeyLabel => "SteamGridDB API Key";
    public string PreferSGDBImagesLabel => "Prefer SGDB Images";
    public string QuickAddLabel => "Quick Add";
    public string BackupOnExitLabel => "Backup On Exit";
    public string DpTypeLabelLabel => "Display Type";
    public string SavingFreqLabel => "Saving Frequency";
    public string RemoteSavingEnabledLabel => "Remote Saving Enabled";
}