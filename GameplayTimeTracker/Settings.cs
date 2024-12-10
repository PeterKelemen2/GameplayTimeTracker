using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameplayTimeTracker;

public class Settings
{
    [JsonPropertyName("startWithSystem")] public bool StartWithSystem { get; set; }

    [JsonPropertyName("horizontalTileGradient")] public bool HorizontalTileGradient { get; set; }

    [JsonPropertyName("horizontalEditGradient")] public bool HorizontalEditGradient { get; set; }

    [JsonPropertyName("bigBgImages")] public bool BigBgImages { get; set; }

    [JsonPropertyName("selectedTheme")] public string SelectedTheme { get; set; }

    [JsonPropertyName("themeList")] public List<Theme> ThemeList { get; set; }
}