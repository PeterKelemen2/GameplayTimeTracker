using System.Text.Json.Serialization;

namespace GameplayTimeTracker.Settings;

public class RemoteMachine
{
    [JsonPropertyName("remoteAddress")] public string RemoteAddress { get; set; }
    [JsonPropertyName("remotePort")] public int RemotePort { get; set; } = 2222;
    [JsonPropertyName("remoteUser")] public string RemoteUser { get; set; }
    [JsonPropertyName("remotePassword")] public string RemotePassword { get; set; }
    [JsonPropertyName("remoteFolder")] public string RemoteFolder { get; set; }
}