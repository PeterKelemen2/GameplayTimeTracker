using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace GameplayTimeTracker;

public static class AppColors
{
    public static Color Background = (Color)ColorConverter.ConvertFromString("#1E2030");
    public static Color Footer = (Color)ColorConverter.ConvertFromString("#1E2030");
    public static Color Font = (Color)ColorConverter.ConvertFromString("#DAE4FF");
    public static Color Running = (Color)ColorConverter.ConvertFromString("#C3E88D");

    public static Color DefButton = (Color)ColorConverter.ConvertFromString("#3BC9E3");
    public static Color DefButtonHover => AdjustBrightness(DefButton, 1.2); // 20% lighter
    public static Color DefButtonPress => AdjustBrightness(DefButton, 0.8); // 20% darker
    public static Color PositiveButton = (Color)ColorConverter.ConvertFromString("#90EE90");
    public static Color PositiveButtonHover => AdjustBrightness(PositiveButton, 1.2);
    public static Color PositiveButtonPress => AdjustBrightness(PositiveButton, 0.8);
    public static Color NegativeButton = (Color)ColorConverter.ConvertFromString("#ED0C0C");
    public static Color NegativeButtonHover => AdjustBrightness(NegativeButton, 1.2);
    public static Color NegativeButtonPress=> AdjustBrightness(NegativeButton, 0.8);

    public static Color ProgressBar1 = (Color)ColorConverter.ConvertFromString("#89ACF2");
    public static Color ProgressBar2 = (Color)ColorConverter.ConvertFromString("#B7BDF8");
    public static Color TileColor1 = (Color)ColorConverter.ConvertFromString("#414769");
    public static Color TileColor2 = (Color)ColorConverter.ConvertFromString("#2E324A");
    public static Color EditColor1 = (Color)ColorConverter.ConvertFromString("#7DD6EB");
    public static Color EditColor2 = (Color)ColorConverter.ConvertFromString("#7EAFE0");
    
    public static Color OuterGlow = (Color)ColorConverter.ConvertFromString("#151515");
    
    public static Dictionary<string, string> GetDefaultColors()
    {
        Dictionary<string, string> colors = new Dictionary<string, string>
        {
            { "bgColor", "#1E2030" },
            { "tileColor1", "#414769" },
            { "tileColor2", "#2E324A" },
            { "leftColor", "#89ACF2" },
            { "rightColor", "#B7BDF8" },
            { "editColor1", "#7DD6EB" },
            { "editColor2", "#7DD6EB" },
            { "shadowColor", "#151515" },
            { "fontColor", "#DAE4FF" },
            { "runningColor", "#C3E88D" },
            { "footerColor", "#90EE90" },
            { "button", $"{DefButton.ToString()}" },
            { "positiveButton", $"{PositiveButton.ToString()}" },
            { "negativeButton", $"{NegativeButton.ToString()}" },
        };
        return colors;
    }
    
    public static Color AdjustBrightness(Color color, double factor)
    {
        factor = Math.Clamp(factor, 0, 1); // Ensure factor is in the range [0, 1]
        return Color.FromRgb(
            (byte)Math.Clamp(color.R * factor, 0, 255),
            (byte)Math.Clamp(color.G * factor, 0, 255),
            (byte)Math.Clamp(color.B * factor, 0, 255)
        );
    }
}