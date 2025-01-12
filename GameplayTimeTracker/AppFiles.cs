using System;
using System.IO;
using System.Reflection;

namespace GameplayTimeTracker;

public static class AppFiles
{
    public static string DocumentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        Assembly.GetExecutingAssembly().GetName().Name);

    public const string SampleImagePath = "assets/no_icon.png";
    public const string EditIcon = "assets/edit.png";
    public const string RemIcon = "assets/remove.png";
    public const string SaveIcon = "assets/save.png";
    public const string FolderIcon = "assets/folder.png";
    public const string AddIcon = "assets/add.png";
    public const string CogIcon = "assets/cog.png";
    public const string AppIconPath = "assets/GameplayTimeTracker.ico";
    public const string SettingsFileName = "settings.json";
    public const string DataFileName = "data.json";
    public const string SavedIconsFolderName = "Saved Icons";
    public const string BackupFolderName = "Backup Data";
    
    public static string DataFilePath = Path.Combine(DocumentsPath, DataFileName);
    public static string SettingsFilePath = Path.Combine(DocumentsPath, SettingsFileName);
    public static string SavedIconsPath = Path.Combine(DocumentsPath, SavedIconsFolderName);
    public static string BackupDataFolder = Path.Combine(DocumentsPath, BackupFolderName);

}