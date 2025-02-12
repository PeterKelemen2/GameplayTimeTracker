using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using GameplayTimeTracker.Settings;
using WindowsShortcutFactory;

namespace GameplayTimeTracker;

public static class DataHandler
{
    public static ObservableCollection<Entry> GetEntriesFromFile(string filePath)
    {
        ObservableCollection<Entry> entries = new();
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            if (jsonString.Length == 0)
            {
                return entries;
            }

            entries = JsonSerializer.Deserialize<ObservableCollection<Entry>>(jsonString);
        }
        else
        {
            WriteEntriesToFile(entries, AppFiles.DataFilePath);
        }

        return entries;
    }

    public static void WriteEntriesToFile(ObservableCollection<Entry> entries, string filePath)
    {
        if (!Path.Exists(AppFiles.DocumentsPath))
        {
            Directory.CreateDirectory(AppFiles.DocumentsPath);
        }

        string jsonString = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, jsonString);
    }

    public static AppSettings GetSettingsFromFile()
    {
        AppSettings settings = new();
        if (File.Exists(AppFiles.SettingsFilePath))
        {
            string jsonString = File.ReadAllText(AppFiles.SettingsFilePath);
            settings = JsonSerializer.Deserialize<AppSettings>(jsonString);
        }
        else
        {
            WriteSettingsToFile(settings);
        }


        var requiredThemes = new List<AppTheme>
        {
            new AppTheme { ThemeName = "Dark", Colors = AppColors.GetDarkColorsDict() },
            new AppTheme { ThemeName = "Pink", Colors = AppColors.GetPinkColorsDict() },
            new AppTheme { ThemeName = "Custom", Colors = AppColors.GetCustomColorsDict() }
        };

        bool isModified = false;
        // Add missing themes
        foreach (var theme in requiredThemes)
        {
            if (!settings.ThemesList.Any(t => t.ThemeName == theme.ThemeName))
            {
                settings.ThemesList.Add(theme);
                isModified = true;
            }
        }

        // Ensure a theme is selected
        if (settings.CurrentTheme == null)
        {
            settings.CurrentTheme = settings.ThemesList.First();
        }

        if (isModified) WriteSettingsToFile(settings);

        ManageStartupShortcut(settings.StartWithSystem);

        return settings;
    }

    public static void WriteSettingsToFile(AppSettings settings)
    {
        if (!Path.Exists(AppFiles.DocumentsPath))
        {
            Directory.CreateDirectory(AppFiles.DocumentsPath);
        }

        string jsonString = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(AppFiles.SettingsFilePath, jsonString);
    }

    public static void ManageStartupShortcut(bool enable)
    {
        string shortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup),
            $"{Assembly.GetExecutingAssembly().GetName().Name}.lnk");

        if (enable)
        {
            if (!File.Exists(shortcutPath))
            {
                string exeLocation = Process.GetCurrentProcess().MainModule.FileName;
                string workingDirectory = Path.GetDirectoryName(exeLocation);
                using var shortcut = new WindowsShortcut
                {
                    Path = exeLocation,
                    WorkingDirectory = workingDirectory,
                    Description = "Gameplay Time Tracker Shortcut",
                };
                shortcut.Save(shortcutPath);
                Console.WriteLine("Shortcut created successfully.");
            }
            else Console.WriteLine("Already in autostart!");
        }
        else
        {
            if (File.Exists(shortcutPath))
            {
                try
                {
                    File.Delete(shortcutPath);
                    Console.WriteLine("Shortcut removed successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to remove shortcut: {ex.Message}");
                }
            }
            else Console.WriteLine("Shortcut does not exist.");
        }
    }

    public static void CreateBackup()
    {
        string currentData = File.ReadAllText(AppFiles.DataFilePath);
        DateTime d = DateTime.Now;
        string backupFileName = $"backup-{d.Year}-{d.Month}-{d.Day}-{d.Hour}-{d.Minute}-{d.Second}.json";
        File.WriteAllText(Path.Combine(AppFiles.BackupDataFolder, backupFileName), currentData);
    }

    public static void RestoreBackup(string backupFilePath)
    {
        string toLoad = File.ReadAllText(backupFilePath);
        File.WriteAllText(AppFiles.DataFilePath, toLoad);
    }
}