namespace GameplayTimeTracker;

public class Params
{
    // Parameterless constructor for serialization
    public Params()
    {
    }

    public Params(string tileGameName, double tileTotalTime, double tileLastPlaytime)
    {
        gameName = tileGameName;
        totalTime = tileTotalTime;
        lastPlayedTime = tileLastPlaytime;
    }

    public string gameName { get; set; }
    public double totalTime { get; set; }
    public double lastPlayedTime { get; set; }
}