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
        foreach (var exeName in exeNames)
        {
            Console.WriteLine($"Exe name: {exeName}");
        }

        TrackProcessesAsync();
    }

    public async Task TrackProcessesAsync()
    {
        Console.WriteLine("Starting process tracking...");
        while (true)
        {
            var runningProcesses = Process.GetProcesses();
            Console.WriteLine("=================");
            foreach (var exeName in exeNames)
            {
                var newExeName = System.IO.Path.GetFileNameWithoutExtension(exeName);
                var isRunning =
                    runningProcesses.Any(p => p.ProcessName.Equals(newExeName, StringComparison.OrdinalIgnoreCase));
                if (isRunning)
                {
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} - {exeName} is running.");
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} - {exeName} is not running.");
                }
            }

            await Task.Delay(1000);
        }
    }
}