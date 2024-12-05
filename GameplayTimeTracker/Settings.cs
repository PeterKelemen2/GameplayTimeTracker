using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameplayTimeTracker;

public class Settings
{
    
    [JsonPropertyName("startWithSystem")] public bool StartWithSystem { get; set; }
    
    // TODO: Add option for usage of horizontal or vertical gradient for Tiles and Edit menus

    [JsonPropertyName("selectedTheme")] public string SelectedTheme { get; set; }
    
    [JsonPropertyName("themeList")] public List<Theme> ThemeList { get; set; }
}