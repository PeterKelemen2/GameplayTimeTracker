using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameplayTimeTracker.Settings;

public class AppSettings
{
    [JsonPropertyName("Start with System")]
    public bool StartWithSystem { get; set; } = true;

    [JsonPropertyName("SteamGridDB API key")]
    public string SGDBApiKey { get; set; } = string.Empty;

    [JsonPropertyName("Prefer Local App Image")]
    public bool PreferLocalAppImage { get; set; } = false;

    public override string ToString()
    {
        return $"SETTINGS: Start: {StartWithSystem}, API Key: {SGDBApiKey}, Prefer Local: {PreferLocalAppImage}";
    }
}