using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace GameplayTimeTracker;

public class ProcessTracker
{
    // List<Tile> _tilesList;
    List<String> _exeNames;
    TileContainer _tileContainer;
    private string runningText = "Running!";
    private string currSessionText = "Current Session:";
    private string lastSessionText = "Last Session:";
    private string notRunningText = "";
    private Dictionary<string, bool> runningDictionary;

    public bool IsDictSet { get; set; }

    public ProcessTracker()
    {
    }

    // Creates a dictionary with the name and running state of each tile to track
    public void InitializeExeDictionary()
    {
        foreach (var tile in _tileContainer.tilesList)
        {
            if (runningDictionary != null)
            {
                if (!runningDictionary.ContainsKey(System.IO.Path.GetFileNameWithoutExtension(tile.ExePath)))
                {
                    // string newKey = System.IO.Path.GetFileNameWithoutExtension(tile.ExePath);
                    runningDictionary.Add(tile.GameName, tile.IsRunning);
                }
            }
        }

        IsDictSet = true;
    }

    // Sets up process tracker from the tile container
    public void InitializeProcessTracker(TileContainer tileContainer)
    {
        _tileContainer = tileContainer;
        _exeNames = _tileContainer.GetExecutableNames();
        InitializeExeDictionary();

        foreach (var exeName in _exeNames)
        {
            Console.WriteLine($"Exe name: {exeName}");
        }
    }

    // Checks if a tile is running and sets values accordingly
    public void HandleProcesses()
    {
        var runningProcesses = Process.GetProcesses();

        Console.WriteLine("=================");
        foreach (var tile in _tileContainer.tilesList)
        {
            var isRunning =
                runningProcesses.Any(p => p.ProcessName.Equals(tile.ExePathName, StringComparison.OrdinalIgnoreCase));

            if (isRunning)
            {
                // Setting things up if first start
                if (tile.WasRunning == false)
                {
                    tile.WasRunning = true;
                    tile.IsRunning = true;
                    tile.ResetLastPlaytime();
                    tile.ToggleBgImageColor(isRunning);
                    Console.WriteLine($"Setting new playtime for {tile.ExePathName}");
                }

                // Only change text if it's not already the correct string
                if (!tile.lastPlaytimeTitle.Text.Equals(currSessionText)) tile.lastPlaytimeTitle.Text = currSessionText;
                if (!tile.runningTextBlock.Text.Equals(runningText)) tile.runningTextBlock.Text = runningText;

                tile.IncrementPlaytime();
                tile.UpdatePlaytimeText();

                // Only update if a minute is passed
                if (tile.LastS % 60 == 0 || tile.TotalS % 60 == 0)
                {
                    _tileContainer.UpdatePlaytimeBars();
                    _tileContainer.InitSave();
                    TextBlock mainTotalTimeBlock = Utils.mainWindow.FindName("TotalPlaytimeTextBlock") as TextBlock;
                    mainTotalTimeBlock.Text =
                        $"Total Playtime: {Utils.GetPrettyTime(_tileContainer.GetTLTotalTimeDouble())}";
                }
            }
            else
            {
                // If it was running, set it back to initial state
                if (tile.IsRunning)
                {
                    tile.WasRunning = false;
                    tile.IsRunning = false;
                    tile.lastPlaytimeTitle.Text = lastSessionText;
                    tile.runningTextBlock.Text = notRunningText;
                    tile.UpdateDateInfo();
                    tile.ToggleBgImageColor(isRunning);
                }
            }
        }

        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}");
    }
}