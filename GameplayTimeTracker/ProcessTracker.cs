using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Gtk;

namespace GameplayTimeTracker;

public class ProcessTracker
{
    List<Tile> tilesList = new();
    List<String> exeNames = new();
    TileContainer _tileContainer = new();

    public ProcessTracker()
    {
    }

    public void InitializeProcessTracker(TileContainer tileContainer)
    {
        exeNames = tileContainer.GetExecutableNames();
        _tileContainer = tileContainer;
        tilesList = tileContainer.GetTiles();

        foreach (var exeName in exeNames)
        {
            Console.WriteLine($"Exe name: {exeName}");
        }

        // Start tracking processes asynchronously
        TrackProcessesAsync();
    }


    public async Task TrackProcessesAsync()
    {
        Console.WriteLine("Starting process tracking...");

        Stopwatch stopwatch = new Stopwatch();
        while (true)
        {
            stopwatch.Restart();
            var runningProcesses = Process.GetProcesses();
            Console.WriteLine("=================");
            string runningString = "Running: ";
            string notRunningString = "Not running: ";
            foreach (var tile in tilesList)
            {
                var newExeName = System.IO.Path.GetFileNameWithoutExtension(tile.ExePath);
                var isRunning =
                    runningProcesses.Any(p => p.ProcessName.Equals(newExeName, StringComparison.OrdinalIgnoreCase));
                if (isRunning)
                {
                    tile.IsRunning = true;
                    if (tile.wasRunning == false)
                    {
                        tile.wasRunning = true;
                        tile.ResetLastPlaytime();
                        tile.UpdatePlaytimeText();
                        _tileContainer.UpdatePlaytimeBars();
                        _tileContainer.InitSave();
                        Console.WriteLine($"Setting new last playtime for {newExeName}");
                    }

                    tile.runningTextBlock.Text = "Running!";
                    tile.CurrentPlaytime++;
                    // Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} - {newExeName} is running.");
                    runningString += $"{tile.GameName} | ";

                    tile.CalculatePlaytimeFromSec(tile.CurrentPlaytime);
                }
                else
                {
                    // Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} - {newExeName} is not running.");
                    tile.runningTextBlock.Text = "";
                    tile.wasRunning = false;
                    notRunningString += $"{tile.GameName} | ";
                }

                tile.ToggleBgImageColor(isRunning);
            }

            _tileContainer.SortByProperty("IsRunning", false);
            _tileContainer.CallSubcribers();
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}");
            Console.WriteLine($"{runningString}");
            Console.WriteLine($"{notRunningString}");
            stopwatch.Stop();
            await Task.Delay(1000 - (int)stopwatch.ElapsedMilliseconds);
        }
    }
}