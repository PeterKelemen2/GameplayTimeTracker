using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace GameplayTimeTracker.Services;

public class TrackerService
{
    public void TrackByExecutable(string exePath)
    {
        var normalizedPath = Path.GetFullPath(exePath);

        foreach (var process in Process.GetProcesses())
        {
            try
            {
                var processPath = process.MainModule?.FileName;
                if (processPath == null)
                    continue;

                if (!Path.Equals(
                        Path.GetFullPath(processPath),
                        normalizedPath))
                    continue;

                Attach(process, normalizedPath);
            }
            catch (Win32Exception)
            {
                // Access denied → ignore
            }
        }
    }

    private void Attach(Process process, string executablePath)
    {
        var startTime = process.StartTime.ToUniversalTime();
        SaveSessionStart(process.Id, executablePath, startTime);

        process.EnableRaisingEvents = true;
        process.Exited += (_, _) => { SaveSessionEnd(process.Id, DateTime.UtcNow); };
    }

    private void SaveSessionStart(int processId, string executablePath, DateTime startTime)
    {
        Console.WriteLine($"Saving session start: {executablePath}");
    }

    private void SaveSessionEnd(int processId, DateTime endTime)
    {
        Console.WriteLine("Saving Session End");
    }
}