using System.Text.Json.Serialization;

namespace GameplayTimeTracker.Settings;

public class RemoteMachine
{
    [JsonPropertyName("address")] public string Address { get; set; }
    [JsonPropertyName("port")] public int Port { get; set; } = 22;
    [JsonPropertyName("user")] public string User { get; set; }
    [JsonPropertyName("password")] public string Password { get; set; }
    [JsonPropertyName("remoteFolder")] public string RemoteFolder { get; set; }
}