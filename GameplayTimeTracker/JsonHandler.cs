using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text.Json;
using WindowsShortcutFactory;

namespace GameplayTimeTracker;

public class JsonHandler
{
    private const string SampleImagePath = "assets/no_icon.png";
    private const string SettingsFileName = "settings.json";
    private const string DataFileName = "data.json";

    public static string DocumentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        Assembly.GetExecutingAssembly().GetName().Name);

    public static string DataFilePath = Path.Combine(DocumentsPath, DataFileName);
    public static string SettingsFilePath = Path.Combine(DocumentsPath, SettingsFileName);


    public string DefaultSettings =
        "{\n  \"startWithSystem\": true,\n  \"themeList\": [\n    {\n      \"themeName\": \"default\",\n      \"colors\": {\n        \"bgColor\": \"#1E2030\",\n        \"footerColor\": \"#90EE90\",\n        \"darkColor\": \"#1E2030\",\n        \"lightColor\": \"#2E324A\",\n        \"fontColor\": \"#DAE4FF\",\n        \"runningColor\": \"#C3E88D\",\n        \"leftColor\": \"#89ACF2\",\n        \"rightColor\": \"#B7BDF8\",\n        \"tileColor1\": \"#414769\",\n        \"tileColor2\": \"#2E324A\",\n        \"shadowColor\": \"#151515\",\n        \"editColor1\": \"#7DD6EB\",\n        \"editColor2\": \"#7DD6EB\"\n      }\n    },\n    {\n      \"themeName\": \"pink\",\n      \"colors\": {\n        \"bgColor\": \"#45092b\",\n        \"footerColor\": \"#90EE90\",\n        \"darkColor\": \"#1E2030\",\n        \"lightColor\": \"#2E324A\",\n        \"fontColor\": \"#DAE4FF\",\n        \"runningColor\": \"#C3E88D\",\n        \"leftColor\": \"#89ACF2\",\n        \"rightColor\": \"#B7BDF8\",\n        \"tileColor1\": \"#db1484\",\n        \"tileColor2\": \"#7d2055\",\n        \"shadowColor\": \"#151515\",\n        \"editColor1\": \"#7DD6EB\",\n        \"editColor2\": \"#7DD6EB\"\n      }\n    }\n  ]\n}\n";

    private string CheckForFile(string filePath)
    {
        return File.Exists(filePath) ? filePath : SampleImagePath;
    }
    
    public void InitializeSettings()
    {
        CheckForDataDirectory();
        if (!File.Exists(SettingsFilePath))
        {
            File.WriteAllText(SettingsFilePath, DefaultSettings);
        }
    }

    public Settings GetSettingsFromFile()
    {
        InitializeSettings();

        string json = File.ReadAllText(SettingsFilePath);
        Settings settings = JsonSerializer.Deserialize<Settings>(json);

        if (settings != null)
        {
            Console.WriteLine($"Start With System: {settings.StartWithSystem}");
            if (settings.StartWithSystem) CreateShortcutForStartup();

            Console.WriteLine($"Themes loaded from settings: {settings.ThemeList.Count}");
            foreach (var theme in settings.ThemeList)
            {
                Console.WriteLine($"Theme: {theme.ThemeName}");
                foreach (var color in theme.Colors)
                {
                    Console.WriteLine($"{theme.ThemeName}: {color.Key} - {color.Value}");
                }
            }
        }

        return settings;
    }

    public void CreateShortcutForStartup()
    {
        string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup),
            $"{Assembly.GetExecutingAssembly().GetName().Name}.lnk");
        if (!File.Exists(shortcutPath))
        {
            string exeLocation = Process.GetCurrentProcess().MainModule.FileName;
            string workingDirectory = Path.GetDirectoryName(exeLocation);
            using var shortcut = new WindowsShortcut
            {
                Path = exeLocation,
                WorkingDirectory = workingDirectory,
                Description = "Shortcut to Gameplay Time Tracker",
            };
            shortcut.Save(shortcutPath);
        }
        else
        {
            Console.WriteLine("Already in autostart!");
        }
    }

    private void CheckForDataDirectory()
    {
        if (!Directory.Exists(DocumentsPath))
        {
            Directory.CreateDirectory(DocumentsPath);

            // Get the current user's identity
            string currentUser = Environment.UserName;
            DirectoryInfo directoryInfo = new DirectoryInfo(DocumentsPath);
            DirectorySecurity security = directoryInfo.GetAccessControl();

            // Add access rule for the current user
            security.AddAccessRule(new FileSystemAccessRule(currentUser,
                FileSystemRights.Modify,
                AccessControlType.Allow));

            directoryInfo.SetAccessControl(security);
        }
    }

    public void InitializeContainer(TileContainer container)
    {
        CheckForDataDirectory();

        if (!File.Exists(DataFilePath))
        {
            File.WriteAllText(DataFilePath, "[]");
        }

        string jsonString = File.ReadAllText(DataFilePath);

        List<Params> paramsList = JsonSerializer.Deserialize<List<Params>>(jsonString);
        if (paramsList != null && paramsList.Count > 0)
        {
            foreach (var param in paramsList)
            {
                container.AddTile(new Tile(
                    container,
                    param.gameName,
                    param.totalTime,
                    param.lastPlayedTime,
                    CheckForFile(param.iconPath),
                    param.exePath));
            }
        }
    }

    public void WriteContentToFile(TileContainer container)
    {
        List<Params> paramsList = new List<Params>();

        foreach (var tile in container.tilesList)
        {
            paramsList.Add(new Params(tile.GameName, tile.TotalPlaytime, tile.LastPlaytime, tile.IconImagePath,
                tile.ExePath));
        }

        string jsonString = JsonSerializer.Serialize(paramsList, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(DataFilePath, jsonString);
        Console.WriteLine($"!! Saved data to {DataFilePath} !!");
    }
}