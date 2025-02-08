using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;

namespace GameplayTimeTracker;

public static class RemoteController
{
    public static void UploadFolder(string localFolderPath, string remoteFolderPath)
    {
        if (!Directory.Exists(localFolderPath))
        {
            Console.WriteLine("Local folder doesn't exist");
            return;
        }

        var remote = Common.Settings.RemoteMachine;
        using (var sftp = new SftpClient(remote.Address, remote.Port, remote.User,
                   remote.Password))
        {
            try
            {
                sftp.Connect();
                Console.WriteLine($"Trying to upload to {remoteFolderPath}");
                EnsureRemoteFolderExists(sftp, remoteFolderPath);

                UploadDirectoryRecursive(sftp, localFolderPath, remoteFolderPath);

                Console.WriteLine("Folder upload complete.");
                sftp.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private static void UploadDirectoryRecursive(SftpClient sftp, string localFolderPath, string remoteFolderPath)
    {
        var files = Directory.GetFiles(localFolderPath);
        var directories = Directory.GetDirectories(localFolderPath);

        // Upload all files in the current directory
        foreach (var file in files)
        {
            using (var fileStream = File.OpenRead(file))
            {
                string remoteFilePath = Path.Combine(remoteFolderPath, Path.GetFileName(file)).Replace("\\", "/");
                sftp.UploadFile(fileStream, remoteFilePath);
                Console.WriteLine($"Uploaded file: {remoteFilePath}");
            }
        }

        // Recursively upload all subdirectories
        foreach (var directory in directories)
        {
            string folderName = Path.GetFileName(directory);
            string remoteSubFolderPath = Path.Combine(remoteFolderPath, folderName).Replace("\\", "/");

            EnsureRemoteFolderExists(sftp, remoteSubFolderPath);
            UploadDirectoryRecursive(sftp, directory, remoteSubFolderPath);
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

    public static void DownloadFolder(string remoteFolderPath, string localFolderPath)
    {
        var remote = Common.Settings.RemoteMachine;
        using (var sftp = new SftpClient(remote.Address, remote.Port, remote.User,
                   remote.Password))
        {
            try
            {
                sftp.Connect();

                // Ensure the local folder exists
                if (!Directory.Exists(localFolderPath))
                {
                    Directory.CreateDirectory(localFolderPath);
                }

                DownloadDirectoryRecursive(sftp, remoteFolderPath, localFolderPath);

                Console.WriteLine("Folder download complete.");
                sftp.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private static void DownloadDirectoryRecursive(SftpClient sftp, string remoteFolderPath, string localFolderPath)
    {
        var entries = sftp.ListDirectory(remoteFolderPath);

        foreach (var entry in entries)
        {
            // Skip the current directory and parent directory entries
            if (entry.Name == "." || entry.Name == "..")
                continue;

            string localPath = Path.Combine(localFolderPath, entry.Name);
            string remotePath = $"{remoteFolderPath}/{entry.Name}".Replace("\\", "/");

            if (entry.IsDirectory)
            {
                Console.WriteLine($"Creating local folder: {localPath}");
                Directory.CreateDirectory(localPath);

                // Recursive call for subdirectories
                DownloadDirectoryRecursive(sftp, remotePath, localPath);
            }
            else if (entry.IsRegularFile)
            {
                Console.WriteLine($"Downloading file: {remotePath} to {localPath}");
                using (var fileStream = File.Create(localPath))
                {
                    sftp.DownloadFile(remotePath, fileStream);
                }
            }
        }
    }

    public static async Task<List<string>> ListGameSubfoldersAsync(string remotePath, string gameName)
    {
        List<string> filesList = new List<string>();
        // Combine the remote path with the game name
        string gameFolderPath = Path.Combine(remotePath, gameName).Replace("\\", "/");

        var remote = Common.Settings.RemoteMachine;
        using (var sftp = new SftpClient(remote.Address, remote.Port, remote.User, remote.Password))
        {
            // Create a CancellationTokenSource for timeout
            var cts = new CancellationTokenSource();
            var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10), cts.Token); // Set a timeout of 10 seconds

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

        return filesList;
    }


    public static string GetPathWithLatestName(string remotePath)
    {
        // Combine the remote path with the game name
        // string gameFolderPath = remotePath;

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
}