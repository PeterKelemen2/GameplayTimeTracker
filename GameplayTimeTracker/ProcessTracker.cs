using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
        TrackProcessesAsync();
    }

    public async Task TrackProcessesAsync()
    {
        Console.WriteLine("Starting process tracking...");

        Stopwatch stopwatch = new Stopwatch();
        while (true)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
            var runningProcesses = Process.GetProcesses();
            Console.WriteLine("=================");
            foreach (var tile in tilesList)
            {
                var newExeName = System.IO.Path.GetFileNameWithoutExtension(tile.ExePath);
                var isRunning =
                    runningProcesses.Any(p => p.ProcessName.Equals(newExeName, StringComparison.OrdinalIgnoreCase));
                if (isRunning)
                {
                    tile.runningTextBlock.Text = "Running!";
                    tile.CurrentPlaytime++;
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} - {newExeName} is running.");
                    tile.CalculatePlaytimeFromSec(tile.CurrentPlaytime);
                    // _tileContainer.InitSave();
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} - {newExeName} is not running.");
                    tile.runningTextBlock.Text = "";
                }
                // tile.SmallSave();
            }

            stopwatch.Stop();
            await Task.Delay(1000 - (Int16)stopwatch.ElapsedMilliseconds);
        }
    }
}