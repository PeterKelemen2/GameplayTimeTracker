using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.VisualBasic.CompilerServices;

namespace GameplayTimeTracker;

public class DataHandler
{
    public DataHandler()
    {
    }
    
    public List<Entry> GetEntriesFromFile(string filePath)
    {
        string jsonString = File.ReadAllText(filePath);
        List<Entry> entries = JsonSerializer.Deserialize<List<Entry>>(jsonString);
        return entries;
    }

    public void WriteEntriesToFile(EntryRepository repository)
    {
        string jsonString = JsonSerializer.Serialize(repository.EntriesList, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(AppFiles.DataFilePath, jsonString);
    }
}