using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace GameplayTimeTracker;

public class JsonHandler
{
    private const string SampleImagePath = "assets/no_icon.png";

    private string CheckForFile(string filePath)
    {
        return File.Exists(filePath) ? filePath : SampleImagePath;
    }

    public void InitializeContainer(TileContainer container, string filePath)
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }

        string jsonString = File.ReadAllText(filePath);
        Console.WriteLine(jsonString);

        List<Params> paramsList = JsonSerializer.Deserialize<List<Params>>(jsonString);
        if (paramsList != null && paramsList.Count > 0)
        {
            foreach (var param in paramsList)
            {
                container.AddTile(new Tile(
                    container,
                    param.gameName,
                    param.totalTime,
                    param.lastPlayedTime,
                    CheckForFile(param.iconPath),
                    param.exePath));
            }
        }
    }

    public void WriteContentToFile(TileContainer container, string filePath)
    {
        List<Params> paramsList = new List<Params>();

        foreach (var tile in container.GetTiles())
        {
            paramsList.Add(new Params(tile.GameName, tile.TotalPlaytime, tile.LastPlaytime, tile.IconImagePath,
                tile.ExePath));
        }

        string jsonString = JsonSerializer.Serialize(paramsList, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, jsonString);
        Console.WriteLine("---- Wrote content");
    }
}