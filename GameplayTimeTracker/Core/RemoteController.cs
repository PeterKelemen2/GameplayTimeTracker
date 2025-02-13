using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Renci.SshNet;

namespace GameplayTimeTracker;

public static class RemoteController
{
    private static double delayDuration = 10;

    public static async Task<bool> TestSftpConnection()
    {
        var remote = Common.Settings.RemoteMachine;

        if (!await IsRemoteMachineAvailableAsync())
        {
            Console.WriteLine("Remote machine is unreachable.");
            return false;
        }

        var sftp = new SftpClient(remote.Address, remote.Port, remote.User, remote.Password);

        try
        {
            Console.WriteLine($"Attempting SFTP connection to {remote.Address}:{remote.Port}...");
            sftp.Connect();
            return sftp.IsConnected;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SFTP connection failed: {ex.Message}");
            return false;
        }
        finally
        {
            if (sftp.IsConnected)
            {
                sftp.Disconnect();
            }

            sftp.Dispose();
        }
    }


    public static async Task<bool> IsRemoteMachineAvailableAsync()
    {
        var remote = Common.Settings.RemoteMachine;
        try
        {
            using (var tcpClient = new TcpClient())
            {
                Console.WriteLine($"Connecting to {remote.Address}:{remote.Port}");
                var connectTask = tcpClient.ConnectAsync(remote.Address, remote.Port);
                var timeoutTask = Task.Delay(3000); // Timeout: 3 sec
                var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                Console.WriteLine($"Connected: {tcpClient.Connected}");
                return completedTask == connectTask && tcpClient.Connected;
            }
        }
        catch
        {
            return false;
        }
    }

    public static async Task<bool> UploadFolderAsync(string localFolderPath, string remoteFolderPath)
    {
        var remote = Common.Settings.RemoteMachine;

        if (!await IsRemoteMachineAvailableAsync())
        {
            Console.WriteLine("Remote machine is unreachable.");
            return false;
        }

        using (var sftp = new SftpClient(remote.Address, remote.Port, remote.User, remote.Password))
        {
            try
            {
                await Task.Run(() => sftp.Connect());
                Console.WriteLine($"Trying to upload to {remoteFolderPath}");

                await Task.Run(() => EnsureRemoteFolderExists(sftp, remoteFolderPath));
                await UploadDirectoryRecursiveAsync(sftp, localFolderPath, remoteFolderPath);

                Console.WriteLine("Folder upload complete.");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    EventPopup stoppedRunning = new EventPopup($"Upload successful!");
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                sftp.Disconnect();
            }
        }
    }


    private static async Task UploadDirectoryRecursiveAsync(SftpClient sftp, string localFolderPath,
        string remoteFolderPath)
    {
        var files = Directory.GetFiles(localFolderPath);
        var directories = Directory.GetDirectories(localFolderPath);

        // Upload files asynchronously
        foreach (var file in files)
        {
            using (var fileStream = File.OpenRead(file))
            {
                string remoteFilePath = Path.Combine(remoteFolderPath, Path.GetFileName(file)).Replace("\\", "/");
                await Task.Run(() => sftp.UploadFile(fileStream, remoteFilePath));
                Console.WriteLine($"Uploaded file: {remoteFilePath}");
            }
        }

        // Upload directories asynchronously
        foreach (var directory in directories)
        {
            string folderName = Path.GetFileName(directory);
            string remoteSubFolderPath = Path.Combine(remoteFolderPath, folderName).Replace("\\", "/");

            await Task.Run(() => EnsureRemoteFolderExists(sftp, remoteSubFolderPath));
            await UploadDirectoryRecursiveAsync(sftp, directory, remoteSubFolderPath);
        }
    }


    private static void EnsureRemoteFolderExists(SftpClient sftp, string remoteFolderPath)
    {
        // Replace backslashes with forward slashes
        remoteFolderPath = remoteFolderPath.Replace("\\", "/");

        // Split the path into parts
        var folders = remoteFolderPath.Split('/');
        string currentPath = remoteFolderPath.StartsWith("/") ? "/" : "";

        foreach (var folder in folders)
        {
            if (string.IsNullOrWhiteSpace(folder))
                continue;

            // Build the path incrementally
            currentPath = currentPath == "/" ? $"/{folder}" : $"{currentPath}/{folder}";

            if (!sftp.Exists(currentPath))
            {
                Console.WriteLine($"Creating folder: {currentPath}");
                sftp.CreateDirectory(currentPath);
            }
        }
    }

    public static async Task<bool> DownloadFolderAsync(string remoteFolderPath, string localFolderPath)
    {
        var remote = Common.Settings.RemoteMachine;
        if (!await IsRemoteMachineAvailableAsync())
        {
            Console.WriteLine("Remote machine is unreachable.");
            return false;
        }

        using (var sftp = new SftpClient(remote.Address, remote.Port, remote.User, remote.Password))
        {
            try
            {
                await Task.Run(() => sftp.Connect());

                // Ensure the local folder exists
                if (!Directory.Exists(localFolderPath))
                {
                    Directory.CreateDirectory(localFolderPath);
                }

                await DownloadDirectoryRecursiveAsync(sftp, remoteFolderPath, localFolderPath);

                Console.WriteLine("Folder download complete.");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    EventPopup stoppedRunning = new EventPopup($"Download successful!");
                });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                sftp.Disconnect();
            }
        }
    }


    private static async Task DownloadDirectoryRecursiveAsync(SftpClient sftp, string remoteFolderPath,
        string localFolderPath)
    {
        var entries = await Task.Run(() => sftp.ListDirectory(remoteFolderPath));

        foreach (var entry in entries)
        {
            // Skip current and parent directory entries
            if (entry.Name == "." || entry.Name == "..")
                continue;

            string localPath = Path.Combine(localFolderPath, entry.Name);
            string remotePath = $"{remoteFolderPath}/{entry.Name}".Replace("\\", "/");

            if (entry.IsDirectory)
            {
                Console.WriteLine($"Creating local folder: {localPath}");
                Directory.CreateDirectory(localPath);

                // Recursive call for subdirectories
                await DownloadDirectoryRecursiveAsync(sftp, remotePath, localPath);
            }
            else if (entry.IsRegularFile)
            {
                Console.WriteLine($"Downloading file: {remotePath} to {localPath}");
                using (var fileStream = File.Create(localPath))
                {
                    await Task.Run(() => sftp.DownloadFile(remotePath, fileStream));
                }
            }
        }
    }


    public static async Task<List<string>> ListGameSubfoldersAsync(string remotePath, string gameName,
        bool oldestFirst = true)
    {
        List<string> filesList = new List<string>();
        // Combine the remote path with the game name
        string gameFolderPath = Path.Combine(remotePath, gameName).Replace("\\", "/");

        var remote = Common.Settings.RemoteMachine;
        // if (!await IsRemoteMachineAvailableAsync(remote.Address, remote.Port))
        // {
        //     Console.WriteLine("Remote machine is unreachable.");
        //     return filesList;
        // }

        using (var sftp = new SftpClient(remote.Address, remote.Port, remote.User, remote.Password))
        {
            // Create a CancellationTokenSource for timeout
            var cts = new CancellationTokenSource();
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(delayDuration), cts.Token);

            // Create a task to connect asynchronously, with timeout
            var connectTask = Task.Run(() => sftp.Connect());

            // Wait for either the connection to complete or timeout
            if (await Task.WhenAny(connectTask, timeoutTask) == timeoutTask)
            {
                Console.WriteLine("Connection timed out.");
                return filesList; // Return empty list if the connection times out
            }

            // Proceed only if the connection was successful
            try
            {
                // Ensure the game folder exists on the remote server with a timeout
                var existsTask = Task.Run(() => sftp.Exists(gameFolderPath));
                if (await Task.WhenAny(existsTask, timeoutTask) == timeoutTask)
                {
                    Console.WriteLine("Operation timed out while checking folder existence.");
                    return filesList; // Return empty list if the operation times out
                }

                // List directories asynchronously with timeout
                var foldersTask = Task.Run(() => sftp.ListDirectory(gameFolderPath));
                if (await Task.WhenAny(foldersTask, timeoutTask) == timeoutTask)
                {
                    Console.WriteLine("Operation timed out while listing directories.");
                    return filesList; // Return empty list if the operation times out
                }

                var folders = foldersTask.Result;
                Console.WriteLine($"Subfolders in {gameFolderPath}:");
                foreach (var file in folders)
                {
                    if (file.IsDirectory && file.Name != "." && file.Name != "..")
                    {
                        Console.WriteLine($"- {file.FullName}");
                        filesList.Add(file.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                sftp.Disconnect();
                cts.Cancel(); // Cancel the timeout task if everything completed successfully
            }
        }


        if (oldestFirst) filesList = filesList.OrderByDescending(x => x).ToList();
        return filesList;
    }


    public static string GetPathWithLatestName(string remotePath)
    {
        var remote = Common.Settings.RemoteMachine;
        using (var sftp = new SftpClient(remote.Address, remote.Port, remote.User,
                   remote.Password))
        {
            sftp.Connect();

            // Ensure the game folder exists on the remote server
            if (sftp.Exists(remotePath))
            {
                var entries = sftp.ListDirectory(remotePath);

                // List of valid subfolder paths
                List<string> subfolders = new List<string>();

                // Iterate through the entries and add subfolders
                foreach (var entry in entries)
                {
                    if (entry.IsDirectory && entry.Name != "." && entry.Name != "..")
                    {
                        subfolders.Add(entry.FullName);
                    }
                }

                if (subfolders.Count > 0)
                {
                    // Find the subfolder with the latest date
                    string latestSubfolder = subfolders
                        .OrderByDescending(subfolder => DateTime.ParseExact(
                            subfolder.Split('/').Last(), "yyyy-MM-dd-HH-mm-ss", null))
                        .First();

                    Console.WriteLine($"The latest subfolder is: {latestSubfolder}");
                    return latestSubfolder;
                }
                else
                {
                    Console.WriteLine("No subfolders found.");
                }
            }
            else
            {
                Console.WriteLine($"The specified folder does not exist: {remotePath}");
            }

            sftp.Disconnect();
        }

        return null;
    }

    public static async Task DeleteSavesOlderThanDays(string remoteSaves, int days)
    {
        var remote = Common.Settings.RemoteMachine;

        if (!await IsRemoteMachineAvailableAsync())
        {
            Console.WriteLine("Remote machine is unreachable.");
            return;
        }

        using (var sftp = new SftpClient(remote.Address, remote.Port, remote.User, remote.Password))
        {
            try
            {
                await Task.Run(() => sftp.Connect());
                Console.WriteLine($"Connected to {remote.Address}, checking {remoteSaves}");

                // Get all folders in the remote directory
                var folders = await Task.Run(() => sftp.ListDirectory(remoteSaves)
                    .Where(f => f.IsDirectory && f.Name != "." && f.Name != ".."));

                DateTime thresholdDate = DateTime.UtcNow.AddDays(-days);

                foreach (var folder in folders)
                {
                    if (folder.LastWriteTimeUtc < thresholdDate)
                    {
                        Console.WriteLine($"Deleting folder: {folder.FullName}");
                        await Task.Run(() => DeleteDirectoryRecursive(sftp, folder.FullName));
                    }
                }

                Console.WriteLine("Cleanup complete.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                sftp.Disconnect();
            }
        }
    }

    private static void DeleteDirectoryRecursive(SftpClient sftp, string directory)
    {
        var filesAndDirs = sftp.ListDirectory(directory);

        foreach (var item in filesAndDirs)
        {
            if (item.Name == "." || item.Name == "..")
                continue;

            if (item.IsDirectory)
            {
                DeleteDirectoryRecursive(sftp, item.FullName); // Recursively delete subdirectories
            }
            else
            {
                sftp.DeleteFile(item.FullName); // Delete files
            }
        }

        sftp.DeleteDirectory(directory); // Finally, delete the directory itself
    }
}