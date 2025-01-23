using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using Toolbelt.Drawing;

namespace GameplayTimeTracker;

public static class ColorHelper
{
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
        if (0 <= hue && hue < 60)
        {
            r = c;
            g = x;
        }
        else if (60 <= hue && hue < 120)
        {
            r = x;
            g = c;
        }
        else if (120 <= hue && hue < 180)
        {
            g = c;
            b = x;
        }
        else if (180 <= hue && hue < 240)
        {
            g = x;
            b = c;
        }
        else if (240 <= hue && hue < 300)
        {
            r = x;
            b = c;
        }
        else if (300 <= hue && hue < 360)
        {
            r = c;
            b = x;
        }

        return Color.FromRgb(
            (byte)Math.Clamp((r + m) * 255, 0, 255),
            (byte)Math.Clamp((g + m) * 255, 0, 255),
            (byte)Math.Clamp((b + m) * 255, 0, 255)
        );
    }

    public static LinearGradientBrush CreateLinGradBrushVer(Color c1, Color c2)
    {
        LinearGradientBrush brush = new LinearGradientBrush();
        brush.StartPoint = new Point(0, 0);
        brush.EndPoint = new Point(0, 1);
        brush.GradientStops.Add(new GradientStop(c1, 0.0));
        brush.GradientStops.Add(new GradientStop(c2, 1.0));
        return brush;
    }

    public static LinearGradientBrush CreateLinGradBrushHor(Color c1, Color c2)
    {
        LinearGradientBrush brush = new LinearGradientBrush();
        brush.StartPoint = new Point(0, 0);
        brush.EndPoint = new Point(1, 0);
        brush.GradientStops.Add(new GradientStop(c1, 0.0));
        brush.GradientStops.Add(new GradientStop(c2, 1.0));
        return brush;
    }
}