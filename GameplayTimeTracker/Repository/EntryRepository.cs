using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Application = System.Windows.Application;

namespace GameplayTimeTracker;

public class EntryRepository : INotifyPropertyChanged
{
    public int RunningEntryCount
    {
        get => EntriesList.Count(entry => entry.IsRunning);
    }

    private int _totalEntryCount;

    public int TotalEntryCount
    {
        get => _totalEntryCount;
        set
        {
            if (SetField(ref _totalEntryCount, value))
            {
                OnPropertyChanged(nameof(TotalEntryCount));
            }
        }
    }

    private int[] _totalRuntime;

    public int[] TotalRuntime
    {
        get => _totalRuntime;
        set
        {
            if (SetField(ref _totalRuntime, value))
            {
                OnPropertyChanged(nameof(TotalRuntimeFormatted));
                _totalRuntime = GetTotalTimeArrays();
            }
        }
    }

    public string TotalRuntimeFormatted =>
        TotalRuntime != null && TotalRuntime.Length == 3
            ? $"{TotalRuntime[0]}h {TotalRuntime[1]}m {TotalRuntime[2]}s"
            : "0h 0m 0s";

    // public ObservableCollection<Entry> EntriesList { get; set; }

    private ObservableCollection<Entry> _entriesList;

    public ObservableCollection<Entry> EntriesList
    {
        get => _entriesList;
        set
        {
            if (_entriesList != value)
            {
                // Unsubscribe from previous collection's change events (if necessary)
                if (_entriesList != null)
                {
                    _entriesList.CollectionChanged -= EntriesList_CollectionChanged;
                }

                _entriesList = value;

                // Subscribe to collection change events
                if (_entriesList != null)
                {
                    _entriesList.CollectionChanged += EntriesList_CollectionChanged;
                }

                // Update TotalEntryCount whenever the collection changes
                TotalEntryCount = _entriesList?.Count ?? 0;
                OnPropertyChanged(nameof(EntriesList));
            }
        }
    }

    private void EntriesList_CollectionChanged(object sender,
        System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        // Whenever the collection changes (item added, removed, etc.), update the TotalEntryCount
        TotalEntryCount = _entriesList.Count;
    }


    public EntryRepository()
    {
        EntriesList = new ObservableCollection<Entry>();
        EntriesList = DataHandler.GetEntriesFromFile(AppFiles.DataFilePath);
        foreach (Entry entry in EntriesList)
        {
            entry.Repository = this;
            entry.EnsureLastWeekData();
            entry.PrintHistory();
            entry.PropertyChanged += OnEntryPropertyChanged;
        }

        Common.CheckForOldTime(EntriesList);
        UpdateTotalPercentages();
        // SetTimeArrays();
        PrintEntryList();
        // DataHandler.WriteEntriesToFile(EntriesList, AppFiles.DataFilePath);
    }

    private void OnEntryPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Entry.TotalPlay))
        {
            TotalRuntime = GetTotalTimeArrays();
            Console.WriteLine(Common.GetPrettyTimeFromArray(TotalRuntime));
        }
    }

    public int[] GetTotalTimeArrays()
    {
        int[] totalTimeArray = new int[3];
        foreach (var entry in EntriesList)
        {
            totalTimeArray = Common.AddTimeArrays(totalTimeArray, entry.TotalPlay);
        }

        totalTimeArray = Common.NormalizeTime(totalTimeArray);

        return totalTimeArray;
    }

    public void ManageEntriesState()
    {
        var runningProcesses = Process.GetProcesses();
        Console.WriteLine($" ==== {DateTime.Now} ==== ");
        foreach (var entry in EntriesList)
        {
            var isRunning =
                runningProcesses.Any(p => p.ProcessName.Equals(Path.GetFileNameWithoutExtension(entry.ExePath),
                    StringComparison.OrdinalIgnoreCase));
            if (isRunning)
            {
                entry.IsRunning = true;
                entry.IncrementTime();
            }
            else
            {
                entry.IsRunning = false;
            }
        }
    }

    public List<Entry> GetEntriesSortedForTray()
    {
        List<Entry> result = EntriesList
            .Where(entry => !entry.IsRunning)
            .OrderByDescending(entry => Common.GetDoubleTimeFromArray(entry.TotalPlay))
            .Take(Math.Min(5, EntriesList.Count()))
            .ToList();
        foreach (Entry entry in result)
        {
            Console.WriteLine($"Sorted: {entry.Name} - {entry.IsRunning}");
        }

        return result;
    }

    public void SortEntries()
    {
        List<Entry> sortedList = new List<Entry>();
        sortedList = EntriesList
            .OrderByDescending(item => item.IsRunning) // Sort by IsRunning first
            .ThenByDescending(item => item.LastDate) // Then by LastPlayDate (descending)
            .ToList();
        EntriesList = new ObservableCollection<Entry>(sortedList);
        ((MainWindow)Application.Current.MainWindow).ShowCards();
    }

    public void AddEntry(Entry entry)
    {
        if (EntriesList.Any(e => e.ExePath == entry.ExePath))
        {
            Console.WriteLine("An entry with this ExePath already exists.");
            return;
        }

        entry.Repository = this;
        entry.EnsureLastWeekData();
        EntriesList.Add(entry);
        entry.PropertyChanged += OnEntryPropertyChanged;
        SortEntries();

        UpdateTotalPercentages();
        PrintEntryList();
        DataHandler.WriteEntriesToFile(EntriesList, AppFiles.DataFilePath);
    }

    public void RemoveEntry(Entry entry)
    {
        if (EntriesList.Contains(entry))
        {
            Console.WriteLine($"Removing entry {entry.Name}");
            EntriesList.Remove(entry);
            UpdateTotalPercentages();
            TotalRuntime = GetTotalTimeArrays();
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

    public void UpdateRunningEntryCount()
    {
        OnPropertyChanged(nameof(RunningEntryCount));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected bool SetField<T>(ref T field, T value,
        [System.Runtime.CompilerServices.CallerMemberName]
        string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    public virtual void OnPropertyChanged(string propertyName)
    {
        var propertyInfo = this.GetType().GetProperty(propertyName);
        var propertyValue = propertyInfo?.GetValue(this);

        // Console.WriteLine($"EntryRepository - PropertyChanged: {propertyName} - Value: {propertyValue}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}