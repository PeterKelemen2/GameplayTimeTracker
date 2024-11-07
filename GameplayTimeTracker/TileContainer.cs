using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GameplayTimeTracker;

public class TileContainer
{
    private JsonHandler handler = new JsonHandler();

    public double TileWidth { get; set; }

    public List<Theme> themes { get; set; } = new();

    public List<Tile> tilesList { get; set; } = new();

    // public List<Tile> toMoveList { get; set; } = new();
    public Theme currentTheme { get; set; } = new();

    public TextBlock Total { get; set; }

    MainWindow _mainWindow;

    public TileContainer()
    {
    }

    public List<string> GetTilesExePath()
    {
        return tilesList.Select(x => x.ExePath).ToList();
    }

    public void SetTilesList(List<Tile> newList)
    {
        tilesList = newList;
    }

    public void OverwriteTiles(List<Tile> newTiles)
    {
        tilesList = newTiles;
    }

    public void InitSave()
    {
        handler.WriteContentToFile(this);
        Console.WriteLine(" ==== Saved! ====");
    }


    public bool IsListEqual(List<Tile> newList)
    {
        if (tilesList.Count != newList.Count) return false;
        for (int i = 0; i < tilesList.Count; i++)
        {
            if (!tilesList[i].Equals(newList[i])) return false;
        }

        return true;
    }

    public List<Tile> SortedByProperty(string propertyName = "", bool ascending = true)
    {
        // Check if the property name is valid
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            MessageBox.Show("Property name is required.");
            return null;
        }

        // Get the property info using reflection
        var propertyInfo = typeof(Tile).GetProperty(propertyName);

        if (propertyInfo == null)
        {
            MessageBox.Show("Invalid property name.");
            return null;
        }

        // Sort in ascending or descending order based on the flag
        var sortedTilesList = ascending
            ? tilesList.OrderBy(item => propertyInfo.GetValue(item, null)).ToList()
            : tilesList.OrderByDescending(item => propertyInfo.GetValue(item, null)).ToList();

        // tilesList.RemoveAll(item => propertyInfo.GetValue(item, null) == null);
        // tilesList = newTilesList;
        return sortedTilesList;
    }

    public List<String> GetExecutableNames()
    {
        List<String> executableNames = new();
        foreach (Tile tile in tilesList)
        {
            if (tile.ExePath is not "")
            {
                if (Path.Exists(tile.ExePath))
                {
                    FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(tile.ExePath);
                    executableNames.Add(Path.GetFileName(tile.ExePath));
                    Console.WriteLine(fileInfo.FileDescription);
                    Console.WriteLine(executableNames[executableNames.Count - 1]);
                }
            }
        }

        return executableNames;
    }

    public void AddTile(Tile newTile, bool newlyAdded = false)
    {
        try
        {
            if (tilesList.Count == 0)
            {
                newTile.Id = 1;
            }
            else
            {
                newTile.Id = tilesList.ElementAt(tilesList.Count - 1).Id + 1;
            }

            if (newlyAdded)
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(newTile.ExePath);
                if (fvi.FileDescription != null)
                {
                    newTile.GameName = fvi.FileDescription;
                }
            }

            newTile.Index = tilesList.Count;
            tilesList.Add(newTile);
            newTile.InitializeTile();

            // This accounts for change in percentages when adding new tile
            UpdatePlaytimeBars();

            newTile.ToggleBgImageColor(newTile.IsRunning);
            Total.Text = $"Games managed: {tilesList.Count}";
            Console.WriteLine($"Tile added to TileContainer!");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Something went wrong adding new Tile to container! ({e})");
        }
    }

    public void ListTiles()
    {
        try
        {
            Console.WriteLine("\nTileContainer content:");
            Console.WriteLine($"Total Playtime: {CalculateTotalPlaytime()} min");
            foreach (var tile in tilesList)
            {
                Console.WriteLine(
                    $"Id: {tile.Id} | Name: {tile.GameName} | Total: {tile.TotalPlaytime} min |" +
                    $" Total%: {tile.TotalPlaytimePercent} | Last: {tile.LastPlaytime} | " +
                    $"Last%: {tile.LastPlaytimePercent} | Icon: {tile.IconImagePath} | Exe: {tile.ExePath}");
            }

            Console.WriteLine(GetTotalPlaytimePretty());
        }
        catch (Exception e)
        {
            Console.WriteLine($"Something went wrong listing TileContainer content!({e})");
        }
    }

    public void RemoveTileById(int id)
    {
        bool isRemoved = false;
        try
        {
            foreach (var tile in tilesList.ToList())
            {
                if (tile.Id.Equals(id))
                {
                    tilesList.Remove(tile);
                    isRemoved = true;
                    // TextBlock totalText = _mainWindow.FindName("GamesLoaded") as TextBlock;
                    Total.Text = $"Games managed: {tilesList.Count}";
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            String message = isRemoved ? $"Tile with ID {id} removed." : $"Couldn't find Tile with ID {id}";
            Console.WriteLine(message);

            InitSave();
        }
    }

    public void UpdateTileById(int id, string propertyName, object newValue)
    {
        foreach (var tile in tilesList)
        {
            if (tile.Id.Equals(id))
            {
                var property = tile.GetType().GetProperty(propertyName);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(tile, newValue);
                }
            }
        }
    }

    public void UpdateTileNameById(int id, string newValue)
    {
        foreach (var tile in tilesList)
        {
            if (tile.Id.Equals(id))
            {
                tile.GameName = newValue;
                tile.InitializeTile();
            }
        }
    }


    public Tile GetTileById(int id)
    {
        foreach (var tile in tilesList)
        {
            if (tile.Id.Equals(id))
            {
                return tile;
            }
        }

        return null;
    }

    public double CalculateTotalPlaytime()
    {
        return tilesList.Sum(tile => tile.TotalPlaytime);
    }

    public string GetTotalPlaytimePretty()
    {
        double playtime = CalculateTotalPlaytime();
        return $"{(int)(playtime / 60)}h {(int)(playtime % 60)}m";
    }

    public void UpdateLastPlaytimeBarOfTile(int tileId)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var tileToUpdate = GetTileById(tileId);
        tileToUpdate.lastTimeGradientBar.Percent =
            Math.Round(tileToUpdate.LastPlaytime / tileToUpdate.TotalPlaytime, 2);

        Console.WriteLine($"Updating last gradient bar {tileToUpdate.GameName}");
        tileToUpdate.lastTimeGradientBar.UpdateBar();
        // tileToUpdate.lastTimeGradientBar.InitializeBar();

        stopwatch.Stop();
        Console.WriteLine($"Updating LAST playtime bar took: {stopwatch.Elapsed}");
    }

    public void UpdateTilesWidth(double newWidth)
    {
        foreach (var tile in tilesList)
        {
            if (Math.Abs(newWidth - tile.TileWidth) > 0)
                tile.UpdateTileWidth(newWidth);
        }
    }

    public void CloseAllEditMenus()
    {
        foreach (var tile in tilesList)
        {
            if (tile.IsMenuToggled)
            {
                tile.ToggleEdit();
            }
        }
    }

    public void UpdatePlaytimeBars()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        double globalTotalPlaytime = 1 / CalculateTotalPlaytime();
        // if (tilesList.Count == 1)
        // {
        //     globalTotalPlaytime = 1;
        // }

        Console.WriteLine("Global: " + globalTotalPlaytime);
        foreach (var tile in tilesList)
        {
            tile.totalTimeGradientBar.Percent = Math.Round(tile.TotalPlaytime * globalTotalPlaytime, 2);

            // if (tilesList.Count == 1)
            // {
            //     tile.totalTimeGradientBar.Percent = 1;
            // }

            Console.WriteLine("Tile total percent:" + tile.totalTimeGradientBar.Percent);
            // Console.WriteLine(Math.Round(tile.LastPlaytime / tile.TotalPlaytime, 2));

            var newPercent = Math.Round(tile.LastPlaytime / tile.TotalPlaytime, 2);
            // switch (newPercent)
            // {
            //     case < 0:
            //         Console.WriteLine("Percent is smaller than 0");
            //         newPercent = 0;
            //         break;
            //     case > 1:
            //         Console.WriteLine("Percent is bigger than 1");
            //         newPercent = 1;
            //         break;
            // }

            tile.lastTimeGradientBar.Percent = newPercent;

            if (!tile.totalTimeGradientBar.WasInitialized)
            {
                tile.totalTimeGradientBar.InitializeBar();
            }
            else
            {
                Console.WriteLine($"Updating total time gradient bar for {tile.GameName}");
                tile.totalTimeGradientBar.UpdateBar();
            }

            Console.WriteLine($"Updating last time gradient bar for {tile.GameName}");
            tile.lastTimeGradientBar.UpdateBar();
        }

        stopwatch.Stop();
        Console.WriteLine($"Updating ALL playtime bars took: {stopwatch.Elapsed}");
    }
}