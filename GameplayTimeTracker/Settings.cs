using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameplayTimeTracker;

public class Settings
{
    
    [JsonPropertyName("startWithSystem")] public bool StartWithSystem { get; set; }

    [JsonPropertyName("themeList")] public List<Theme> ThemeList { get; set; }
}