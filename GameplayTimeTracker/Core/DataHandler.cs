using System.Collections.Generic;
using System.IO;
using System.Text.Json;

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
}