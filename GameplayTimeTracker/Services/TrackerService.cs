using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using GameplayTimeTracker.Models;

namespace GameplayTimeTracker.Services;

public class TrackerService
{
    private ManagementEventWatcher? _startWatch;

    public void StartListening(Game game)
    {
        TrackByGame(game);
        string query = "SELECT * FROM Win32_ProcessStartTrace";
        _startWatch = new ManagementEventWatcher(new WqlEventQuery(query));

        _startWatch.EventArrived += (s, e) =>
        {
            try
            {
                var processId = Convert.ToInt32(e.NewEvent.Properties["ProcessId"].Value);
                var process = Process.GetProcessById(processId);

                var exePath = process.MainModule?.FileName;
                if (exePath == null)
                    return;

                if (Path.GetFullPath(exePath).Equals(Path.GetFullPath(exePath), StringComparison.OrdinalIgnoreCase))
                {
                    AttachToProcess(process, game);
                }
            }
            catch (ManagementException ex)
            {
                Console.WriteLine(ex.Message);
            }
        };

        _startWatch.Start();
    }

    public void TrackByGame(Game game)
    {
        var normalizedPath = Path.GetFullPath(game.ExePath);

        foreach (var process in Process.GetProcesses())
        {
            try
            {
                var processPath = process.MainModule?.FileName;

                if (processPath == null || !Equals(Path.GetFullPath(processPath), normalizedPath))
                    continue;

                AttachToProcess(process, game);
            }
            catch (Win32Exception)
            {
                // Access denied → ignore
            }
        }
    }

    private void AttachToProcess(Process process, Game game)
    {
        var startTime = process.StartTime.ToUniversalTime();
        SaveSessionStart(game, startTime);

        process.EnableRaisingEvents = true;
        process.Exited += (_, _) => { SaveSessionEnd(game, process.ExitTime.ToUniversalTime()); };
    }

    private void SaveSessionStart(Game game, DateTime startTime)
    {
        if (game.IsTracked) return;

        game.IsTracked = true;
        Console.WriteLine($"Session Start: {game.DisplayName} - ({startTime})");
    }

    private void SaveSessionEnd(Game game, DateTime startTime)
    {
        game.IsTracked = false;
        
        Console.WriteLine($"Session End: {game.DisplayName} - ({startTime})");
    }

    public void StopListening()
    {
        _startWatch?.Stop();
        _startWatch?.Dispose();
        _startWatch = null;
    }
}