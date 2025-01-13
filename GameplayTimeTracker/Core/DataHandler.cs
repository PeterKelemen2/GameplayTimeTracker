using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.VisualBasic.CompilerServices;
using MonoMac.CoreWlan;

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
            WriteEntriesToFile(entries);
        }
        return entries;
    }

    public static void WriteEntriesToFile(List<Entry> entries)
    {
        if (!Path.Exists(AppFiles.DocumentsPath))
        {
            Directory.CreateDirectory(AppFiles.DocumentsPath);
        }
        string jsonString = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(AppFiles.DataFilePath, jsonString);
    }
}