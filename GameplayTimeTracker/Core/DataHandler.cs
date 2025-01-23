using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using GameplayTimeTracker.Settings;

namespace GameplayTimeTracker;

public static class DataHandler
{
    public static List<Entry> GetEntriesFromFile(string filePath)
    {
        List<Entry> entries = new();
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            entries = JsonSerializer.Deserialize<List<Entry>>(jsonString);
        }
        else
        {
            WriteEntriesToFile(entries, AppFiles.DataFilePath);
        }

        return entries;
    }

    public static void WriteEntriesToFile(List<Entry> entries, string filePath)
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
}