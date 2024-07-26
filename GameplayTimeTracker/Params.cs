namespace GameplayTimeTracker;

public class Params
{
    public string gameName { get; set; }
    public double totalTime { get; set; }
    public double lastPlayedTime { get; set; }

    public string GetInfo()
    {
        return $"{gameName}, {totalTime}, {lastPlayedTime}";
    }
}