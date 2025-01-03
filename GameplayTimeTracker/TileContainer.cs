﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace GameplayTimeTracker;

public class TileContainer
{
    private JsonHandler handler = new JsonHandler();
    public List<Tile> tilesList { get; set; } = new();
    public Run TotalTimeRun { get; set; }
    MainWindow _mainWindow;

    public TileContainer()
    {
    }

    public List<string> GetTilesExePath()
    {
        return tilesList.Select(x => x.ExePath).ToList();
    }

    public string GetTileNameByExePath(string exePath)
    {
        return tilesList.FirstOrDefault(tile => tile.ExePath.Equals(exePath))?.GameName ?? string.Empty;
    }

    public bool IsExePathPresent(string exePathToCheck)
    {
        // Get the list of ExePaths and check if the specific exePath is present
        return tilesList.Any(x => x.ExePath.Equals(exePathToCheck, StringComparison.OrdinalIgnoreCase));
    }

    public List<string?> GetExecutableNames()
    {
        return tilesList
            .Where(x => !string.IsNullOrEmpty(x.ExePath) && File.Exists(x.ExePath))
            .Select(x => FileVersionInfo.GetVersionInfo(x.ExePath).FileDescription)
            .ToList();
    }

    public void InitSave()
    {
        handler.WriteContentToFile(this, Utils.DataFilePath);
        Console.WriteLine(" ==== Saved! ====");
    }

    // Sorts content of the tile list by a certain property in ascending or descending order.
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

    /*
     * Adds a new tile to the tile list with a unique ID.
     * If it's a newly added entry, it gets the game from the exe file description.
     * Updates all bars for tiles.
     */
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
                if (File.Exists(newTile.ExePath))
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(newTile.ExePath);
                    if (fvi.FileDescription != null)
                    {
                        newTile.GameName = fvi.FileDescription;
                    }
                }
            }

            newTile.Index = tilesList.Count;
            tilesList.Add(newTile);
            newTile.InitializeTile();

            // This accounts for change in percentages when adding new tile
            UpdatePlaytimeBars();

            newTile.ToggleBgImageColor(newTile.IsRunning);
            
            Run Total = Utils.mainWindow.FindName("GameCountRun") as Run;
            Total.Text = $"{tilesList.Count}";
            Console.WriteLine($"Tile added to TileContainer!");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Something went wrong adding new Tile to container! ({e})");
        }
    }

    // Prints content of tilesList to the console
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

    // Looks for the tile with a specific ID and removes it from the list
    public void RemoveTileById(int id)
    {
        bool isRemoved = false;
        try
        {
            
            foreach (var tile in tilesList.ToList())
            {
                if (tile.Id.Equals(id))
                {
                    // tile.ToggleEdit();
                    tilesList.Remove(tile);
                    isRemoved = true;

                    Run Total = Utils.mainWindow.FindName("GameCountRun") as Run;
                    Total.Text = $"{tilesList.Count}";
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

    // Returns a tile with a certain ID
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

    // Return the total number of minutes played across all entries in tilesList
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
        var tileToUpdate = GetTileById(tileId);
        tileToUpdate.lastTimeGradientBar.Percent =
            Math.Round(tileToUpdate.LastPlaytime / tileToUpdate.TotalPlaytime, 2);

        tileToUpdate.lastTimeGradientBar.UpdateBar();
    }

    // Updates the width of all tiles based on the parameter
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
            if (tile.TileEditMenu.IsOpen) tile.TileEditMenu.CloseMenu();
        }
    }

    public void CloseAllPopups()
    {
        foreach (var tile in tilesList)
        {
            if (tile.deleteMenu != null)
            {
                if (tile.deleteMenu.IsToggled)
                {
                    tile.deleteMenu.CloseMenuMethod();
                }
            }
        }
    }

    public void UpdateTilesColors()
    {
        foreach (var tile in tilesList)
        {
            tile.UpdateTileColors();
            tile.ToggleBgImageColor(tile.IsRunning);
        }
    }

    public void UpdateTileBgImages(bool value)
    {
        foreach (var tile in tilesList)
        {
            // tile.iconContainerGrid = new Grid();
            // tile.iconContainerGrid = tile.GetBgImagesInGrid(value);
            tile.SetBgImageSizes(value);
        }
    }

    public void UpdateTilesGradients(bool tileG, bool editG)
    {
        foreach (var tile in tilesList)
        {
            tile.HorizontalTileG = tileG;
            tile.HorizontalEditG = editG;
            tile.SetGradients();
        }
    }

    public double GetTLTotalTimeDouble()
    {
        double globalTotalPlaytime = 0;
        foreach (Tile tile in tilesList)
        {
            globalTotalPlaytime += tile.GetTotalPlaytimeAsDouble();
        }

        return globalTotalPlaytime;
    }

    public void UpdatePlaytimeBars()
    {
        double totalH = GetTLTotalTimeDouble();
        foreach (Tile tile in tilesList)
        {
            tile.totalTimeGradientBar.Percent = tile.GetTotalPlaytimeAsDouble() / totalH;
            tile.lastTimeGradientBar.Percent = tile.GetLastPlaytimeAsDouble() / tile.GetTotalPlaytimeAsDouble();
            if (!tile.totalTimeGradientBar.WasInitialized)
            {
                tile.totalTimeGradientBar.InitializeBar();
            }
            else
            {
                tile.totalTimeGradientBar.UpdateBar();
            }

            tile.lastTimeGradientBar.UpdateBar();
        }
    }

    // public void UpdateLegacyTime(SettingsMenu setMenu)
    public void UpdateLegacyTime()
    {
        foreach (var tile in tilesList)
        {
            (tile.TotalH, tile.TotalM, tile.TotalS) = Utils.ConvertMinutesToTime(tile.TotalPlaytime);
            (tile.LastH, tile.LastM, tile.LastS) = Utils.ConvertMinutesToTime(tile.LastPlaytime);
            tile.TotalPlaytime = tile.GetTotalPlaytimeAsDouble();
            tile.LastPlaytime = tile.GetLastPlaytimeAsDouble();
            tile.UpdatePlaytimeText();
        }

        UpdatePlaytimeBars();
        TotalTimeRun.Text = $"{Utils.GetPrettyTime(GetTLTotalTimeDouble())}";
        InitSave();
        Console.WriteLine("Legacy data updated!");
    }

    public void RestoreBackup()
    {
        JsonHandler handler = new JsonHandler();
        handler.RestoreBackupDataFile();
        AddRestoredEntries(handler);
    }

    public void AddRestoredEntries(JsonHandler handler)
    {
        List<Params> paramsList = handler.GetDataFromFile();
        Settings settings = handler.GetSettingsFromFile();
        foreach (var entry in paramsList)
        {
            var matchingTile = tilesList.FirstOrDefault(tile => tile.ExePath == entry.exePath);
            Tile newTile = new Tile(
                this,
                entry.gameName,
                entry.lastPlayDate.Year < 2000 || entry.lastPlayDate == null
                    ? new DateTime(2, 1, 1)
                    : entry.lastPlayDate,
                settings.HorizontalTileGradient,
                settings.HorizontalEditGradient,
                settings.BigBgImages,
                entry.totalTime,
                entry.lastPlayedTime,
                entry.iconPath,
                entry.exePath,
                entry.arguments == null ? "" : entry.arguments);
            if (matchingTile != null)
            {
                tilesList.Remove(matchingTile);
            }

            AddTile(newTile, newlyAdded: true);
        }
        // InitSave();
    }
}