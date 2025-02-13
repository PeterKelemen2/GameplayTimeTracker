using System.Text.Json.Serialization;

namespace GameplayTimeTracker;

public class EntryProxy
{
    [JsonPropertyName("gameName")] public string Name { get; set; }
    [JsonPropertyName("TotalPlay")] public int[] TotalPlay { get; set; }
    [JsonPropertyName("totalTime")] public double TotalPlayOld { get; set; }

    public string GetPrettyTime()
    {
        return Common.GetPrettyTimeFromArray(TotalPlay);
    }
}