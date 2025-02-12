using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GameplayTimeTracker;

public static class AppFiles
{
    public static string DocumentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        Assembly.GetExecutingAssembly().GetName().Name);

    public const string DefaultIconPath = "Assets/DefaultIcon.png";
    public const string DefaultHeroPath = "Assets/DefaultHero.png";
    public const string EditIcon = "Assets/edit.png";
    public const string RemoveIcon = "Assets/remove.png";
    public const string SaveIcon = "Assets/save.png";
    public const string FolderIcon = "Assets/folder.png";
    public const string AddIcon = "Assets/add.png";
    public const string CogIcon = "Assets/cog.png";
    public const string ArrowIcon = "Assets/arrow.png";
    public const string LoadingImage = "Assets/loading.png";
    public const string AppIconPath = "Assets/GameplayTimeTracker.ico";

    public const string SettingsFileName = "settings.json";
    public const string DataFileName = "data.json";
    public const string SavedImagesFolderName = "Images";
    public const string BackupFolderName = "Backup Data";

    public static readonly string DataFilePath = Path.Combine(DocumentsPath, DataFileName);
    public static readonly string SettingsFilePath = Path.Combine(DocumentsPath, SettingsFileName);
    public static readonly string SavedImagesPath = Path.Combine(DocumentsPath, SavedImagesFolderName);
    public static readonly string BackupDataFolder = Path.Combine(DocumentsPath, BackupFolderName);
    // public static readonly string SGDBFolder = Path.Combine(DocumentsPath, SGDBFolderName);

    public static void EnsureAppFolders()
    {
        List<string> folders = new List<string>();
        folders.AddRange(new[] { SavedImagesPath, BackupDataFolder });
        foreach (var folder in folders)
        {
            Directory.CreateDirectory(folder);
        }
    }
}