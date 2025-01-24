using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameplayTimeTracker;

public class EntryRepository
{
    public List<Entry> EntriesList { get; set; }

    public EntryRepository()
    {
        EntriesList = DataHandler.GetEntriesFromFile(AppFiles.DataFilePath);
        foreach (Entry entry in EntriesList)
        {
            entry.Repository = this;
        }

        CheckForOldTime();
        UpdateTotalPercentages();
        // SetTimeArrays();
        PrintEntryList();
    }

    private void CheckForOldTime()
    {
        int[] empty = { 0, 0, 0 };

        foreach (var entry in EntriesList)
        {
            if (IsArrayEqual(entry.TotalPlay, empty) && entry.TotalTime > 0.0)
            {
                entry.TotalPlay = Common.GetArrayFromDoubleTime(entry.TotalTime);
            }

            if (IsArrayEqual(entry.LastPlay, empty) && entry.LastTime > 0.0)
            {
                entry.LastPlay = Common.GetArrayFromDoubleTime(entry.LastTime);
            }
        }
    }

    private bool IsArrayEqual(int[] array1, int[] array2)
    {
        return array1 != null && array2 != null && array1.SequenceEqual(array2);
    }

    public void AddEntry(Entry entry)
    {
        if (EntriesList.Any(e => e.ExePath == entry.ExePath))
        {
            Console.WriteLine("An entry with this ExePath already exists.");
            return;
        }

        entry.Repository = this;
        EntriesList.Add(entry);

        UpdateTotalPercentages();
    }

    public void RemoveEntry(Entry entry)
    {
        if (EntriesList.Contains(entry))
        {
            Console.WriteLine($"Removing entry {entry.Name}");
            EntriesList.Remove(entry);
            UpdateTotalPercentages();
            PrintEntryList();
        }
    }

    public List<string> GetExeNames()
    {
        return EntriesList.Select(entry => Path.GetFileName(entry.ExePath)).ToList();
    }

    private void SetTimeArrays()
    {
        foreach (var entry in EntriesList)
        {
            entry.TotalPlay = Common.GetArrayFromDoubleTime(entry.TotalTime);
            entry.LastPlay = Common.GetArrayFromDoubleTime(entry.LastTime);
        }
    }

    public void PrintEntryList()
    {
        int[] p = Common.p;
        string header = $" | {Common.Truncate("Name", p[0])}" +
                        $" | {Common.Truncate("TotalTime", p[1])}" +
                        $" | {Common.Truncate("LastTime", p[2])}" +
                        $" | {Common.Truncate("TotalPerc", p[3])}" +
                        $" | {Common.Truncate("LastPerc", p[4])}" +
                        $" | {Common.Truncate("LastDate", p[5])}" +
                        $" | {Common.Truncate("ExePath", p[6])}" +
                        $" | {Common.Truncate("IconPath", p[7])}" +
                        $" | {Common.Truncate("Arguments", p[8])} |";
        Console.WriteLine(header);
        string row = "";
        for (int i = 0; i < p.Length; i++)
        {
            row += $" | {string.Concat(Enumerable.Repeat("-", p[i]))}";
        }

        Console.WriteLine(row + " |");

        foreach (var entry in EntriesList)
        {
            Console.WriteLine(entry.ToString());
        }
    }

    public double GetTotalTime()
    {
        return EntriesList.Sum(entry => entry.GetTotalPlaytimeAsDouble());
    }

    public void UpdateTotalPercentages()
    {
        double globalTotalTime = EntriesList.Sum(entry => entry.GetTotalPlaytimeAsDouble());
        foreach (var entry in EntriesList)
        {
            entry.TotalPerc = Math.Round(entry.GetTotalPlaytimeAsDouble() / globalTotalTime, 2);
            entry.LastPerc = Math.Round(entry.GetLastPlaytimeAsDouble() / entry.GetTotalPlaytimeAsDouble(), 2);
        }
    }

    public bool IsExePresent(string exePath)
    {
        return EntriesList.Any(x => x.ExePath.Equals(exePath, StringComparison.OrdinalIgnoreCase));
    }

    public string GetNameByExePath(string exePath)
    {
        return EntriesList.FirstOrDefault(x => x.ExePath.Equals(exePath, StringComparison.OrdinalIgnoreCase))?.Name;
    }
}