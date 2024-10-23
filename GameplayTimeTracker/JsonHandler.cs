using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using Gtk;

namespace GameplayTimeTracker;

public class JsonHandler
{
    private const string SampleImagePath = "assets/no_icon.png";
    private const string SettingsPath = "settings.json";

    private string CheckForFile(string filePath)
    {
        return File.Exists(filePath) ? filePath : SampleImagePath;
    }

    public void InitializeSettings()
    {
        if (!File.Exists(SettingsPath))
        {
            File.WriteAllText(SettingsPath,
                "[\n  {\n    \"themeName\": \"default\",\n    \"colors\": {\n      \"darkColor\": \"#1E2030\",\n      \"lightColor\": \"#2E324A\",\n      \"fontColor\": \"#DAE4FF\",\n      \"runningColor\": \"#C3E88D\",\n      \"leftColor\": \"#89ACF2\",\n      \"rightColor\": \"#B7BDF8\",\n      \"tileColor1\": \"#414769\",\n      \"tileColor2\": \"#2E324A\",\n      \"shadowColor\": \"#151515\",\n      \"editColor1\": \"#7DD6EB\",\n      \"editColor2\": \"#7DD6EB\"\n    }\n  }\n]");
        }
    }

    public List<Theme> GetThemesFromFile()
    {
        string json = File.ReadAllText(SettingsPath);
        List<Theme> themes = JsonSerializer.Deserialize<List<Theme>>(json);

        if (themes != null)
        {
            foreach (var theme in themes)
            {
                Console.WriteLine($"Theme name: {theme.ThemeName}");
                foreach (var color in theme.Colors)
                {
                    Console.WriteLine($"{color.Key}: {color.Value}");
                }
            }
            Console.WriteLine($"Theme(s) found: {themes.Count}");
            return themes;
        }
        
        return null;
    }

    public void InitializeContainer(TileContainer container, string filePath)
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }

        string jsonString = File.ReadAllText(filePath);
        // Console.WriteLine(jsonString);

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
        Console.WriteLine($"!! Saved data to {filePath} !!");
    }
}