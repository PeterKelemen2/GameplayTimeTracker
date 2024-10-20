using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Windows;

namespace GameplayTimeTracker;

public class TileContainer
{
    private List<Tile> tilesList = new();
    private JsonHandler handler = new JsonHandler();
    private const string jsonFilePath = "data.json";


    public TileContainer()
    {
    }

    public List<Tile> GetTiles()
    {
        return tilesList;
    }

    public void OverwriteTiles(List<Tile> newTiles)
    {
        tilesList = newTiles;
    }
    
    public void InitSave()
    {
        handler.WriteContentToFile(this, jsonFilePath);
        Console.WriteLine(" ==== Saved! ====");
    }

    public void SortByProperty(string propertyName = "", bool ascending = true)
    {
        // Check if the property name is valid
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            MessageBox.Show("Property name is required.");
            return;
        }

        // Get the property info using reflection
        var propertyInfo = typeof(Tile).GetProperty(propertyName);

        if (propertyInfo == null)
        {
            MessageBox.Show("Invalid property name.");
            return;
        }

        // Sort in ascending or descending order based on the flag
        tilesList = ascending
            ? tilesList.OrderBy(item => propertyInfo.GetValue(item, null)).ToList()
            : tilesList.OrderByDescending(item => propertyInfo.GetValue(item, null)).ToList();

        // Build the sorted string
        // string order = ascending ? "ascending" : "descending";
        // string sortedString = $"Sorted by {propertyName} in {order} order:\n\n";
        // foreach (var tile in sortedItems)
        // {
        //     var propertyValue = propertyInfo.GetValue(tile, null);
        //     if (propertyName.Equals("IsRunning"))
        //     {
        //         sortedString += $"{tile.GameName} - {tile.IsRunning}.\n";
        //     }
        //     else
        //     {
        //         sortedString += propertyValue + "\n";
        //     }
        // }

        // Display the sorted results
        // MessageBox.Show(sortedString);
        // return sortedItems;
    }



    public List<String> GetExecutableNames()
    {
        List<String> executableNames = new();
        foreach (Tile tile in tilesList)
        {
            if (tile.ExePath is not "")
            {
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(tile.ExePath);
                executableNames.Add(Path.GetFileName(tile.ExePath));
                Console.WriteLine(fileInfo.FileDescription);
                Console.WriteLine(executableNames[executableNames.Count - 1]);
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

            tilesList.Add(newTile);


            // This accounts for change in percentages when adding new tile
            double auxPlaytime = CalculateTotalPlaytime();
            foreach (var tile in tilesList)
            {
                tile.TotalPlaytimePercent = Math.Round(tile.TotalPlaytime / auxPlaytime, 2);
                tile.InitializeTile();
            }

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

    public double CalculateTotalPlaytime()
    {
        double playtime = 0;
        foreach (var tile in tilesList)
        {
            playtime += tile.TotalPlaytime;
        }

        return playtime;
    }

    public void UpdatePlaytimeBars()
    {
        double globalTotalPlaytime = CalculateTotalPlaytime();
        foreach (var tile in tilesList)
        {
            tile.totalTimeGradientBar.Percent = Math.Round(tile.TotalPlaytime / globalTotalPlaytime, 2);
            Console.WriteLine(Math.Round(tile.LastPlaytime / tile.TotalPlaytime, 2));
            tile.lastTimeGradientBar.Percent = Math.Round(tile.LastPlaytime / tile.TotalPlaytime, 2);
        }

        foreach (var tile in tilesList)
        {
            tile.totalTimeGradientBar.InitializeBar();
            tile.lastTimeGradientBar.InitializeBar();
        }
    }
}