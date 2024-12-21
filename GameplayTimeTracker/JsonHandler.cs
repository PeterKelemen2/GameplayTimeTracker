using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text.Json;
using MonoMac.CoreImage;
using WindowsShortcutFactory;

namespace GameplayTimeTracker;

public class JsonHandler
{
    // If there is no settings file, it creates one with the default configuration.
    public void InitializeSettings()
    {
        CheckForDataDirectory();
        if (!File.Exists(Utils.SettingsFilePath))
        {
            Settings settings = GetDefaultSettings();
            WriteSettingsToFile(settings);
        }
    }

    private void WriteSettingsToFile(Settings settings)
    {
        string defaultJson = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Utils.SettingsFilePath, defaultJson);
    }

    private Settings GetDefaultSettings()
    {
        Settings settings = new Settings();

        settings.StartWithSystem = true;
        CreateShortcutForStartup();

        settings.HorizontalTileGradient = true;
        settings.HorizontalEditGradient = true;

        settings.BigBgImages = false;

        settings.SelectedTheme = "Default";

        settings.ThemeList = new List<Theme>();
        FillMissingThemes(settings);

        return settings;
    }

    // Creates a Settings instance from the settings.json
    public Settings GetSettingsFromFile()
    {
        InitializeSettings();

        string json = File.ReadAllText(Utils.SettingsFilePath);
        Settings settings;

        if (string.IsNullOrWhiteSpace(json))
        {
            settings = GetDefaultSettings();
            WriteSettingsToFile(settings);
        }
        else
        {
            settings = JsonSerializer.Deserialize<Settings>(json);
        }

        if (settings != null)
        {
            if (settings.StartWithSystem == null)
            {
                settings.StartWithSystem = true;
                CreateShortcutForStartup();
            }
            else if (settings.StartWithSystem)
            {
                CreateShortcutForStartup();
            }
            else
            {
                RemoveShortcutForStartup();
            }

            if (settings.SelectedTheme == null) settings.SelectedTheme = "Default";

            if (settings.ThemeList == null) settings.ThemeList = new List<Theme>();
            FillMissingThemes(settings);
        }

        return settings;
    }

    private void FillMissingThemes(Settings settings)
    {
        bool containsDefault = settings.ThemeList.Any(theme => theme.ThemeName == "Default");
        bool containsPink = settings.ThemeList.Any(theme => theme.ThemeName == "Pink");
        bool containsCustom = settings.ThemeList.Any(theme => theme.ThemeName == "Custom");

        if (!containsDefault)
        {
            Theme theme = new Theme();
            theme.ThemeName = "Default";
            theme.Colors = Utils.GetDefaultColors();
            settings.ThemeList.Add(theme);
        }

        if (!containsPink)
        {
            Theme theme = new Theme();
            theme.ThemeName = "Pink";
            theme.Colors = Utils.GetPinkColors();
            settings.ThemeList.Add(theme);
        }

        if (!containsCustom)
        {
            Theme theme = new Theme();
            theme.ThemeName = "Custom";
            theme.Colors = Utils.GetCustomColors();
            settings.ThemeList.Add(theme);
        }

        foreach (Theme theme in settings.ThemeList)
        {
            theme.Colors = Utils.CheckThemeIntegrity(theme.Colors);
        }
    }

    public void WriteThemesToFile(List<Theme> themesList)
    {
        Settings currentSettings = GetSettingsFromFile();
        currentSettings.ThemeList = themesList;

        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(currentSettings, options);

        File.WriteAllText(Utils.SettingsFilePath, jsonString);
    }

    public void WriteSelectedThemeToFile(String tName)
    {
        Settings currentSettings = GetSettingsFromFile();
        currentSettings.SelectedTheme = tName;
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(currentSettings, options);
        File.WriteAllText(Utils.SettingsFilePath, jsonString);
    }

    public void WriteStartWithSystemToFile(bool value)
    {
        Settings currentSettings = GetSettingsFromFile();
        currentSettings.StartWithSystem = value;

        if (value)
        {
            CreateShortcutForStartup();
        }
        else
        {
            RemoveShortcutForStartup();
        }

        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(currentSettings, options);
        File.WriteAllText(Utils.SettingsFilePath, jsonString);
    }

    public void WritePrefsToFile(bool start, bool tileG, bool editG, bool bigBgImages)
    {
        Settings currentSettings = GetSettingsFromFile();

        currentSettings.StartWithSystem = start;
        if (start)
        {
            CreateShortcutForStartup();
        }
        else
        {
            RemoveShortcutForStartup();
        }

        currentSettings.HorizontalTileGradient = tileG;
        currentSettings.HorizontalEditGradient = editG;

        currentSettings.BigBgImages = bigBgImages;

        WriteSettingsToFile(currentSettings);
    }


    private void RemoveShortcutForStartup()
    {
        string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup),
            $"{Assembly.GetExecutingAssembly().GetName().Name}.lnk");
        // Check if the file exists before attempting to delete
        if (File.Exists(shortcutPath))
        {
            try
            {
                // Delete the shortcut file
                File.Delete(shortcutPath);
                Console.WriteLine("Shortcut removed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to remove shortcut: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Shortcut does not exist.");
        }
    }

    // Puts a shortcut for the app in the directory for it to be able to start with the system.
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
        if (!Directory.Exists(Utils.DocumentsPath))
        {
            Directory.CreateDirectory(Utils.DocumentsPath);

            // Get the current user's identity
            string currentUser = Environment.UserName;
            DirectoryInfo directoryInfo = new DirectoryInfo(Utils.DocumentsPath);
            DirectorySecurity security = directoryInfo.GetAccessControl();

            // Add access rule for the current user
            security.AddAccessRule(new FileSystemAccessRule(currentUser,
                FileSystemRights.Modify,
                AccessControlType.Allow));

            directoryInfo.SetAccessControl(security);
        }

        if (!Directory.Exists(Utils.SavedIconsPath))
        {
            Directory.CreateDirectory(Utils.SavedIconsPath);
        }
    }

    // Creates a list of parameters used for creating tiles in the container.
    public void InitializeContainer(TileContainer container, Settings settings)
    {
        CheckForDataDirectory();

        if (!File.Exists(Utils.DataFilePath))
        {
            File.WriteAllText(Utils.DataFilePath, "[]");
        }

        container.tilesList.Clear();
        string jsonString = File.ReadAllText(Utils.DataFilePath);

        List<Params> paramsList = JsonSerializer.Deserialize<List<Params>>(jsonString);
        if (paramsList != null && paramsList.Count > 0)
        {
            foreach (var param in paramsList)
            {
                container.AddTile(new Tile(
                    container,
                    param.gameName,
                    param.lastPlayDate.Year < 2000 || param.lastPlayDate == null
                        ? new DateTime(2, 1, 1)
                        : param.lastPlayDate,
                    settings.HorizontalTileGradient,
                    settings.HorizontalEditGradient,
                    settings.BigBgImages,
                    param.totalTime,
                    param.lastPlayedTime,
                    param.iconPath,
                    param.exePath,
                    param.arguments == null ? "" : param.arguments));
            }
        }
    }

    public bool CheckForDataToUpdate()
    {
        return Assembly.GetExecutingAssembly().GetName().Version < new Version(1, 3, 3);
    }

    // By using a list of parameters from the container, it writes the data to the file
    public void WriteContentToFile(TileContainer container, string outputPath)
    {
        List<Params> paramsList = new List<Params>();

        foreach (var tile in container.tilesList)
        {
            tile.TotalPlaytime = tile.GetTotalPlaytimeAsDouble();
            tile.LastPlaytime = tile.GetLastPlaytimeAsDouble();
            paramsList.Add(new Params(tile.GameName, tile.LastPlayDate, tile.TotalPlaytime, tile.LastPlaytime,
                tile.IconImagePath,
                tile.ExePath, tile.ShortcutArgs));
        }

        string jsonString = JsonSerializer.Serialize(paramsList, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outputPath, jsonString);
        Console.WriteLine($"!! Saved data to {outputPath} !!");
    }

    public bool BackupDataFile()
    {
        try
        {
            string oldContent = File.ReadAllText(Utils.DataFilePath);
            File.WriteAllText(Utils.BackupDataFilePath, oldContent);
            return true;
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine("File not found!");
        }

        return false;
    }

    public bool RestoreBackupDataFile()
    {
        try
        {
            if (File.Exists(Utils.BackupDataFilePath))
            {
                string backupData = File.ReadAllText(Utils.BackupDataFilePath);
                File.WriteAllText(Utils.DataFilePath, backupData);
                return true;
            }

            return false;
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine($"File not found!\n{e}");
        }

        return false;
    }
}