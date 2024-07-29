namespace GameplayTimeTracker;

public class Params
{
    // Parameterless constructor for serialization
    public Params()
    {
    }

    public Params(string tileGameName, double tileTotalTime, double tileLastPlaytime, string iconImagePath,
        string tileExePath)
    {
        gameName = tileGameName;
        totalTime = tileTotalTime;
        lastPlayedTime = tileLastPlaytime;
        iconPath = iconImagePath;
        exePath = tileExePath;
    }

    public string gameName { get; set; }
    public double totalTime { get; set; }
    public double lastPlayedTime { get; set; }
    public string iconPath { get; set; }
    public string exePath { get; set; }
}