using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace GameplayTimeTracker;

public class JsonHandler
{
    public void InitializeContainer(TileContainer container, string filePath)
    {
        string jsonString = File.ReadAllText(filePath);
        Console.WriteLine(jsonString);

        List<Params> paramsList = JsonSerializer.Deserialize<List<Params>>(jsonString);

        foreach (var param in paramsList)
        {
            container.AddTile(new Tile(container, param.gameName, param.totalTime, param.lastPlayedTime));
        }
    }

    public void WriteContentToFile(TileContainer container, string filePath)
    {
        List<Params> paramsList = new List<Params>();

        foreach (var tile in container.GetTiles())
        {
            paramsList.Add(new Params(tile.GameName, tile.TotalPlaytime, tile.LastPlaytime));
        }

        string jsonString = JsonSerializer.Serialize(paramsList, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, jsonString);
    }
}