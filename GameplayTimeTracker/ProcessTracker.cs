using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GameplayTimeTracker;

public class ProcessTracker
{
    // List<Tile> _tilesList;
    List<String> _exeNames;
    TileContainer _tileContainer;
    private string runningText = "Running!";
    private string currentlyRunningTimeString = "Current playtime:";
    private string LastPlaytimeString = "Last playtime:";
    private string notRunningText = "";
    private Dictionary<string, bool> runningDictionary;

    public bool IsDictSet { get; set; }

    public ProcessTracker()
    {
    }

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
                if (tile.wasRunning == false)
                {
                    tile.wasRunning = true;
                    tile.IsRunning = true;
                    tile.ResetLastPlaytime();
                    tile.UpdatePlaytimeText();
                    tile.ToggleBgImageColor(isRunning);
                    Console.WriteLine($"Setting new last playtime for {tile.ExePathName}");
                }

                // Only change text if it's not already the correct string
                if (!tile.lastPlaytimeTitle.Text.Equals(currentlyRunningTimeString))
                {
                    tile.lastPlaytimeTitle.Text = currentlyRunningTimeString;
                }

                if (!tile.runningTextBlock.Text.Equals(runningText))
                {
                    tile.runningTextBlock.Text = runningText;
                }

                tile.CurrentPlaytime++;
                tile.CalculatePlaytimeFromSec(tile.CurrentPlaytime);

                // Only update if a minute is passed
                if (tile.CurrentPlaytime % 60 == 0)
                {
                    _tileContainer.UpdatePlaytimeBars();
                }
            }
            else
            {
                // If it was running, set it back to initial state
                if (tile.IsRunning)
                {
                    tile.wasRunning = false;
                    tile.IsRunning = false;
                    tile.lastPlaytimeTitle.Text = LastPlaytimeString;
                    tile.runningTextBlock.Text = notRunningText;
                    tile.ToggleBgImageColor(isRunning);
                }
            }

            // Update text every cycle for seconds
            tile.UpdatePlaytimeText();
        }

        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}");
    }
}