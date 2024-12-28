using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Animation;

namespace GameplayTimeTracker;

public class EntryRepository
{
    public List<Entry> EntriesList { get; set; }

    public EntryRepository()
    {
        JsonHandler handler = new JsonHandler();
        EntriesList = handler.GetEntriesFromFile(Utils.DataFilePath);

        CheckForOldTime();
        UpdateTotalPercs();
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
                entry.TotalPlay = Utils.GetArrayFromDoubleTime(entry.TotalTime);
            }

            if (IsArrayEqual(entry.LastPlay, empty) && entry.LastTime > 0.0)
            {
                entry.LastPlay = Utils.GetArrayFromDoubleTime(entry.LastTime);
            }
        }
    }

    private bool IsArrayEqual(int[] array1, int[] array2)
    {
        return array1 != null && array2 != null && array1.SequenceEqual(array2);
    }

    public void AddEntry(Entry entry)
    {
        EntriesList.Add(entry);
        UpdateTotalPercs();
    }

    public void RemoveEntry(Entry entry)
    {
        if (EntriesList.Contains(entry))
        {
            Console.WriteLine($"Removing entry {entry.Name}");
            EntriesList.Remove(entry);
            UpdateTotalPercs();
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
            entry.TotalPlay = Utils.GetArrayFromDoubleTime(entry.TotalTime);
            entry.LastPlay = Utils.GetArrayFromDoubleTime(entry.LastTime);
        }
    }

    private void PrintEntryList()
    {
        int[] p = Utils.p;
        string header = $" | {Utils.Truncate("Name", p[0])}" +
                        $" | {Utils.Truncate("TotalTime", p[1])}" +
                        $" | {Utils.Truncate("LastTime", p[2])}" +
                        $" | {Utils.Truncate("TotalPerc", p[3])}" +
                        $" | {Utils.Truncate("LastPerc", p[4])}" +
                        $" | {Utils.Truncate("LastDate", p[5])}" +
                        $" | {Utils.Truncate("ExePath", p[6])}" +
                        $" | {Utils.Truncate("IconPath", p[7])}" +
                        $" | {Utils.Truncate("Arguments", p[8])} |";
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

    private void UpdateTotalPercs()
    {
        double globalTotalTime = EntriesList.Sum(entry => entry.GetTotalPlaytimeAsDouble());
        foreach (var entry in EntriesList)
        {
            entry.TotalPerc = Math.Round(entry.GetTotalPlaytimeAsDouble() / globalTotalTime, 2);
            entry.LastPerc = Math.Round(entry.GetLastPlaytimeAsDouble() / entry.GetTotalPlaytimeAsDouble(), 2);
        }
    }
}