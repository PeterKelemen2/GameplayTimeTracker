using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using GameplayTimeTracker.Menu;
using Gdk;

namespace GameplayTimeTracker;

public static class Launcher
{
    public static async void Launch(Entry entry)
    {
        try
        {
            if (entry.IsRunning)
            {
                Console.WriteLine("Already running");
                // var alreadyRunningPrompt = new PromptMenu(
                //     width: 300, textArray: new[] { entry.Name, "is already running." }, boldArray: new[] { true, false }
                // );
                // alreadyRunningPrompt.Open();
                EventPopup alreadyRunning = new EventPopup($" {entry.Name} already running!");
                return;
            }

            // Capture UI-bound properties on the UI thread
            string gameName = entry.Name; // Cache the value
            string exePath = entry.ExePath; // Cache the value

            if (string.IsNullOrEmpty(exePath) || !System.IO.File.Exists(exePath))
            {
                throw new FileNotFoundException($"Executable not found: {exePath}");
            }

            string workingDir = System.IO.Path.GetDirectoryName(exePath);
            if (string.IsNullOrEmpty(workingDir) || !System.IO.Directory.Exists(workingDir))
            {
                throw new DirectoryNotFoundException($"Working directory not found: {workingDir}");
            }

            await Task.Run(() =>
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = exePath, // Use the cached variable
                    Arguments = entry.Arguments,
                    WorkingDirectory = workingDir,
                    UseShellExecute = true
                };

                Console.WriteLine($"Trying to launch {exePath} with arguments: {entry.Arguments}");

                var process = Process.Start(startInfo);

                if (process != null)
                {
                    Console.WriteLine($"Launched {gameName}. Waiting for it to exit...");
                    process.WaitForExit();

                    // Notify the user on the UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Console.WriteLine($"{gameName} has exited. Exit code: {process.ExitCode}");
                    });
                }
                else
                {
                    // Notify the user on the UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Console.WriteLine($"Failed to start {gameName}");
                        // PopupMenu popupMenu = new PopupMenu(text: $"Failed to start {gameName}", type: PopupType.OK);
                        // popupMenu.OpenMenu();
                    });
                }
            });
        }
        catch (Win32Exception win32Ex) when (win32Ex.NativeErrorCode == 740) // Error 740 means elevation required
        {
            MessageBoxResult result = MessageBox.Show(
                $"The application {entry.Name} requires administrator privileges. Do you want to run it as administrator?",
                "Elevation Required", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = entry.ExePath,
                    WorkingDirectory = System.IO.Path.GetDirectoryName(entry.ExePath),
                    UseShellExecute = true,
                    Verb = "runas"
                };

                Process.Start(startInfo);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex}");
            // Ensure error messages are shown on the UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                // MessageBox.Show($"Could not launch {GameName}\n\n{ex.Message}", "Something went wrong!",
                //     MessageBoxButton.OK, MessageBoxImage.Error);
                // PopupMenu popupMenu = new PopupMenu(text: $"Failed to start {GameName}", type: PopupType.OK);
                // popupMenu.OpenMenu();
            });
        }
    }
}