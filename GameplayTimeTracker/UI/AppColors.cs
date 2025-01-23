using System.Collections.Generic;
using System.Windows.Media;

namespace GameplayTimeTracker;

public static class AppColors
{
    public static Color Background = (Color)ColorConverter.ConvertFromString("#1E2030");
    public static Color Footer = (Color)ColorConverter.ConvertFromString("#6A6F99");
    public static Color Font = (Color)ColorConverter.ConvertFromString("#DAE4FF");
    public static Color Running = (Color)ColorConverter.ConvertFromString("#C3E88D");
    public static Color DefButton = (Color)ColorConverter.ConvertFromString("#3BC9E3");
    public static Color PositiveButton = (Color)ColorConverter.ConvertFromString("#90EE90");
    public static Color NegativeButton = (Color)ColorConverter.ConvertFromString("#ED0C0C");
    public static Color ProgressBar1 = (Color)ColorConverter.ConvertFromString("#89ACF2");
    public static Color ProgressBar2 = (Color)ColorConverter.ConvertFromString("#B7BDF8");
    public static Color CardColor1 = (Color)ColorConverter.ConvertFromString("#414769");
    public static Color CardColor2 = (Color)ColorConverter.ConvertFromString("#2E324A");
    public static Color Shadow = (Color)ColorConverter.ConvertFromString("#151515");

    public static Dictionary<string, string> GetColorsDict()
    {
        Dictionary<string, string> colors = new Dictionary<string, string>
        {
            { "Footer", "#6A6F99" },
            { "Background ", "#1E2030" },
            { "Card 1", "#414769" },
            { "Card 2", "#2E324A" },
            { "Progress Bar 1", "#89ACF2" },
            { "Progress Bar 2", "#B7BDF8" },
            { "Font", "#DAE4FF" },
            { "Running", "#C3E88D" },
            { "Button", "#3BC9E3" },
            { "Positive Button", "#90EE90" },
            { "Negative Button", "#ED0C0C" },
            { "Shadow", "#151515" },
        };
        return colors;
    }
}