using System;
using System.Collections.Generic;
using System.Linq;

namespace GameplayTimeTracker;

public class EntryRepository
{
    public List<Entry> EntriesList { get; set; }

    public EntryRepository()
    {
        JsonHandler handler = new JsonHandler();
        EntriesList = handler.GetEntriesFromFile(Utils.DataFilePath);
        UpdateTotalPercs();
        SetTimeArrays();
        PrintEntryList();
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
        double globalTotalTime = EntriesList.Sum(entry => entry.TotalTime);
        foreach (var entry in EntriesList)
        {
            entry.TotalPerc = Math.Round(entry.TotalTime / globalTotalTime, 2);
            entry.LastPerc = Math.Round(entry.LastTime / entry.TotalTime, 2);
        }
    }
    
}