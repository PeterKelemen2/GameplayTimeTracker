using System.Text.Json.Serialization;

namespace GameplayTimeTracker.Settings;

public class RemoteMachine
{
    [JsonPropertyName("Address")] public string Address { get; set; }
    [JsonPropertyName("Port")] public int Port { get; set; } = 22;
    [JsonPropertyName("User")] public string User { get; set; }
    [JsonPropertyName("Password")] public string Password { get; set; }
    [JsonPropertyName("Remote Folder")] public string RemoteFolder { get; set; }
}