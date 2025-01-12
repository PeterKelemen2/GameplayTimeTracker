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
    public static Color DefButtonHover => AdjustBrightness(DefButton, 1.2);
    public static Color DefButtonPress => AdjustBrightness(DefButton, 0.8);
    
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
        // Convert RGB to HSL
        double hue, saturation, lightness;
        RgbToHsl(color, out hue, out saturation, out lightness);

        // Adjust lightness
        lightness = Math.Clamp(lightness * factor, 0, 1);

        // Convert back to RGB
        return HslToRgb(hue, saturation, lightness);
    }

    private static void RgbToHsl(Color color, out double hue, out double saturation, out double lightness)
    {
        double r = color.R / 255.0;
        double g = color.G / 255.0;
        double b = color.B / 255.0;

        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));
        double delta = max - min;

        hue = 0;
        if (delta != 0)
        {
            if (max == r) hue = (g - b) / delta;
            else if (max == g) hue = 2 + (b - r) / delta;
            else if (max == b) hue = 4 + (r - g) / delta;
            hue = (hue * 60 + 360) % 360;
        }

        lightness = (max + min) / 2;

        saturation = (delta == 0) ? 0 : delta / (1 - Math.Abs(2 * lightness - 1));
    }

    private static Color HslToRgb(double hue, double saturation, double lightness)
    {
        double c = (1 - Math.Abs(2 * lightness - 1)) * saturation;
        double x = c * (1 - Math.Abs((hue / 60) % 2 - 1));
        double m = lightness - c / 2;

        double r = 0, g = 0, b = 0;
        if (0 <= hue && hue < 60) { r = c; g = x; }
        else if (60 <= hue && hue < 120) { r = x; g = c; }
        else if (120 <= hue && hue < 180) { g = c; b = x; }
        else if (180 <= hue && hue < 240) { g = x; b = c; }
        else if (240 <= hue && hue < 300) { r = x; b = c; }
        else if (300 <= hue && hue < 360) { r = c; b = x; }

        return Color.FromRgb(
            (byte)Math.Clamp((r + m) * 255, 0, 255),
            (byte)Math.Clamp((g + m) * 255, 0, 255),
            (byte)Math.Clamp((b + m) * 255, 0, 255)
        );
    }
}