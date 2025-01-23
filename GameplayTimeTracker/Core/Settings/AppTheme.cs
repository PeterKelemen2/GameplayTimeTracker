using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameplayTimeTracker.Settings;

public class AppTheme
{
    [JsonPropertyName("Theme Name")] public string ThemeName { get; set; } = "Default";

    [JsonPropertyName("Colors")] public Dictionary<string, string> Colors { get; set; } = AppColors.GetColorsDict();
}