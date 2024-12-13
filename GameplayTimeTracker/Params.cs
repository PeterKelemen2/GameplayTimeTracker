namespace GameplayTimeTracker;

public class Params
{
    // Parameterless constructor for serialization
    public Params()
    {
    }

    // Parameters needed for creating a tile instance
    public Params(string tileGameName, double tileTotalTime, double tileLastPlaytime, string? iconImagePath,
        string tileExePath, string args)
    {
        gameName = tileGameName;
        totalTime = tileTotalTime;
        lastPlayedTime = tileLastPlaytime;
        iconPath = iconImagePath;
        exePath = tileExePath;
        arguments = args;
    }

    public string gameName { get; set; }
    public double totalTime { get; set; }
    public double lastPlayedTime { get; set; }
    public string? iconPath { get; set; }
    public string exePath { get; set; }
    public string arguments { get; set; }
}