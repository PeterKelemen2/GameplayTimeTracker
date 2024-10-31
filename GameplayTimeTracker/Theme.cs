using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameplayTimeTracker;

public class Theme
{
    [JsonPropertyName("themeName")] public string ThemeName { get; set; }
    [JsonPropertyName("colors")] public Dictionary<string, string> Colors { get; set; }
}