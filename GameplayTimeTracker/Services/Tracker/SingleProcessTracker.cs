using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using GameplayTimeTracker.ViewModels;
using Microsoft.Extensions.Logging;

namespace GameplayTimeTracker.Services.Tracker;

public class SingleProcessTracker
{
    private readonly ILogger<SingleProcessTracker> _logger;
    private readonly GameViewModel _game;
    private readonly HashSet<int> _runningProcesses = new();
    private ManagementEventWatcher? _watcher;

    public SingleProcessTracker(GameViewModel game)
    {
        _game = game;
        _logger = AppLogger.CreateLogger<SingleProcessTracker>();
    }

    public void Start()
    {
        _logger.LogDebug("Starting single process tracker...");
        TrackExistingProcesses();

        string query = "SELECT * FROM Win32_ProcessStartTrace";
        _watcher = new ManagementEventWatcher(new WqlEventQuery(query));
        _watcher.EventArrived += OnProcessStarted;
        _watcher.Start();
        _logger.LogInformation("Single process tracker started.");
    }

    private void TrackExistingProcesses()
    {
        var normalizedPath = Path.GetFullPath(_game.ExePath);

        foreach (var process in Process.GetProcesses())
        {
            try
            {
                var path = process.MainModule?.FileName;
                if (path != null && Path.GetFullPath(path).Equals(normalizedPath, StringComparison.OrdinalIgnoreCase))
                    AttachProcess(process);
            }
            catch (Win32Exception)
            {
                // ignore access denied
            }
            catch (InvalidOperationException)
            {
                // Process exited between enumeration and access
            }
        }
    }

    private void OnProcessStarted(object sender, EventArrivedEventArgs e)
    {
        try
        {
            var processId = Convert.ToInt32(e.NewEvent.Properties["ProcessId"].Value);
            var process = Process.GetProcessById(processId);

            var path = process.MainModule?.FileName;
            if (path != null && Path.GetFullPath(path)
                    .Equals(Path.GetFullPath(_game.ExePath), StringComparison.OrdinalIgnoreCase))
                AttachProcess(process);
        }
        catch
        {
            // ignore errors
        }
    }

    private void AttachProcess(Process process)
    {
        if (_runningProcesses.Contains(process.Id))
            return;

        _runningProcesses.Add(process.Id);

        if (!_game.IsTracked)
            StartSession(process.StartTime);

        process.EnableRaisingEvents = true;
        process.Exited += (_, _) =>
        {
            _runningProcesses.Remove(process.Id);

            if (_runningProcesses.Count == 0)
                EndSession(process.ExitTime);
        };
    }

    // TODO: Fix concurrency issues
    private void StartSession(DateTime startTime)
    {
        if (_game.IsTracked) return;
        
        _game.IsTracked = true;
        _game.LastPlaytime.StartDate = startTime;
        _logger.LogInformation($"Session started: {_game.DisplayName} - ({startTime})");
    }

    private void EndSession(DateTime endTime)
    {
        if (!_game.IsTracked) return;

        _game.IsTracked = false;
        _game.LastPlaytime.EndDate = endTime;

        _logger.LogInformation(
            $"Session ended: {_game.DisplayName} - ({endTime}) - Duration: {_game.LastPlaytimeDur}");
    }

    public void Stop()
    {
        _logger.LogDebug("Stopping single process tracker...");
        _watcher?.Stop();
        _watcher?.Dispose();
        _watcher = null;
        _logger.LogDebug("Single process tracker stopped.");
    }
}