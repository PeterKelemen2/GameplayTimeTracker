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
        // _tilesList = _tileContainer.GetTiles();
        InitializeExeDictionary();

        foreach (var exeName in _exeNames)
        {
            Console.WriteLine($"Exe name: {exeName}");
        }
    }

    public void HandleProcesses()
    {
        // _tilesList = _tileContainer.GetTiles();
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

                if (!_tileContainer.IsInMoveList(tile)) _tileContainer.AddToMoveList(tile);
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

                if (_tileContainer.IsInMoveList(tile)) _tileContainer.RemoveFromMoveList(tile);
            }


            // Update text every cycle for seconds
            tile.UpdatePlaytimeText();
        }

        Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}");
    }


    // public async Task TrackProcessesAsync()
    // {
    //     Console.WriteLine("Starting process tracking...");
    //
    //     Stopwatch stopwatch = new Stopwatch();
    //     while (true)
    //     {
    //         stopwatch.Restart();
    //         var runningProcesses = Process.GetProcesses();
    //         Console.WriteLine("=================");
    //         string runningString = "Running: ";
    //         string notRunningString = "Not running: ";
    //         foreach (var tile in _tilesList)
    //         {
    //             var newExeName = System.IO.Path.GetFileNameWithoutExtension(tile.ExePath);
    //             var isRunning =
    //                 runningProcesses.Any(p => p.ProcessName.Equals(newExeName, StringComparison.OrdinalIgnoreCase));
    //             if (isRunning)
    //             {
    //                 tile.IsRunning = true;
    //                 if (tile.wasRunning == false)
    //                 {
    //                     tile.wasRunning = true;
    //                     tile.ResetLastPlaytime();
    //                     tile.UpdatePlaytimeText();
    //                     _tileContainer.UpdatePlaytimeBars();
    //                     _tileContainer.InitSave();
    //                     Console.WriteLine($"Setting new last playtime for {newExeName}");
    //                 }
    //
    //                 tile.runningTextBlock.Text = "Running!";
    //                 tile.CurrentPlaytime++;
    //                 // Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} - {newExeName} is running.");
    //                 runningString += $"{tile.GameName} | ";
    //
    //                 tile.CalculatePlaytimeFromSec(tile.CurrentPlaytime);
    //             }
    //             else
    //             {
    //                 // Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} - {newExeName} is not running.");
    //                 tile.runningTextBlock.Text = "";
    //                 tile.wasRunning = false;
    //                 notRunningString += $"{tile.GameName} | ";
    //             }
    //
    //             tile.ToggleBgImageColor(isRunning);
    //         }
    //
    //         // _tileContainer.SortByProperty("IsRunning", false);
    //         Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}");
    //         // Console.WriteLine($"{runningString}");
    //         // Console.WriteLine($"{notRunningString}");
    //         stopwatch.Stop();
    //         await Task.Delay(1000 - (int)stopwatch.ElapsedMilliseconds);
    //     }
    // }
}