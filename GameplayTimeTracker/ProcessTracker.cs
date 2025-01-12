using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace GameplayTimeTracker;

public class ProcessTracker
{
    private string runningText = "Running!";
    private string currSessionText = "Current Session:";
    private string lastSessionText = "Last Session:";
    private string notRunningText = "";
    Stopwatch stopwatch = new();
    HashSet<string> targetProcesses = new();

    public ProcessTracker()
    {
    }
    // Checks if a tile is running and sets values accordingly
    // public void HandleProcesses()
    // {
    //     stopwatch.Restart();
    //     // var runningProcesses = Process.GetProcesses()
    //     //     .Select(p => p.ProcessName)
    //     //     .ToHashSet(StringComparer.OrdinalIgnoreCase);
    //     // var runningProcesses = targetProcesses.Where(ProcessIsRunning).ToHashSet(StringComparer.OrdinalIgnoreCase);
    //     
    //     var runningProcesses = Process.GetProcesses()
    //         .Where(p => targetProcesses.Contains(p.ProcessName, StringComparer.OrdinalIgnoreCase))
    //         .Select(p => p.ProcessName)
    //         .ToHashSet(StringComparer.OrdinalIgnoreCase);
    //     
    //     stopwatch.Stop();
    //     Console.WriteLine("=================");
    //     Console.WriteLine($"Running processes queried in {stopwatch.Elapsed.TotalMilliseconds.ToString("F2")} ms.");
    //     
    //     stopwatch.Restart();
    //     foreach (var tile in _tileContainer.tilesList)
    //     {
    //         bool isRunning = runningProcesses.Contains(tile.ExePathName);
    //         if (isRunning)
    //         {
    //             // Setting things up if first start
    //             if (tile.WasRunning == false)
    //             {
    //                 tile.WasRunning = true;
    //                 tile.IsRunning = true;
    //                 tile.ResetLastPlaytime();
    //                 tile.ToggleBgImageColor(isRunning);
    //                 Console.WriteLine($"Setting new playtime for {tile.ExePathName}");
    //             }
    //
    //             // Only change text if it's not already the correct string
    //             if (!tile.lastPlaytimeTitle.Text.Equals(currSessionText)) tile.lastPlaytimeTitle.Text = currSessionText;
    //             if (!tile.runningTextBlock.Text.Equals(runningText)) tile.runningTextBlock.Text = runningText;
    //
    //             tile.IncrementPlaytime();
    //             tile.UpdatePlaytimeText();
    //
    //             // Only update if a minute is passed
    //             if (tile.LastS % 60 == 0 || tile.TotalS % 60 == 0)
    //             {
    //                 _tileContainer.UpdatePlaytimeBars();
    //                 _tileContainer.InitSave();
    //                 _tileContainer.TotalTimeRun.Text = $"{Utils.GetPrettyTime(_tileContainer.GetTLTotalTimeDouble())}";
    //             }
    //         }
    //         else
    //         {
    //             // If it was running, set it back to initial state
    //             if (tile.IsRunning)
    //             {
    //                 tile.WasRunning = false;
    //                 tile.IsRunning = false;
    //                 tile.lastPlaytimeTitle.Text = lastSessionText;
    //                 tile.runningTextBlock.Text = notRunningText;
    //                 tile.UpdateDateInfo();
    //                 tile.ToggleBgImageColor(isRunning);
    //             }
    //         }
    //     }
    //
    //     stopwatch.Stop();
    //     Console.WriteLine($"Looking for running entries took {stopwatch.Elapsed.TotalMilliseconds.ToString("F2")} ms.");
    //     Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")}");
    // }
}