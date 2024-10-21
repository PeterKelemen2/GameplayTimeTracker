using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace GameplayTimeTracker;

public class Utils
{
    public static Color DarkColor = (Color)ColorConverter.ConvertFromString("#1E2030");
    public static Color LightColor = (Color)ColorConverter.ConvertFromString("#2E324A");
    public static Color FontColor = (Color)ColorConverter.ConvertFromString("#DAE4FF");
    public static Color RunningColor = (Color)ColorConverter.ConvertFromString("#C3E88D");
    public static Color LeftColor = (Color)ColorConverter.ConvertFromString("#89ACF2");
    public static Color RightColor = (Color)ColorConverter.ConvertFromString("#B7BDF8");
    public static Color TileColor1 = (Color)ColorConverter.ConvertFromString("#414769");
    public static Color TileColor2 = (Color)ColorConverter.ConvertFromString("#2E324A");


    public static BlurEffect blurEffect = new BlurEffect
    {
        Radius = 10
    };

    public static DropShadowEffect dropShadowText = new DropShadowEffect
    {
        BlurRadius = 8,
        ShadowDepth = 0,
        Color = Colors.Black,
        Opacity = 1,
        Direction = 200,
    };
    public static DropShadowEffect dropShadowTextEdit = new DropShadowEffect
    {
        BlurRadius = 5,
        ShadowDepth = 0,
        Color = Colors.LightBlue,
        Opacity = 1,
        Direction = 200,
    };

    public static DropShadowEffect dropShadowIcon = new DropShadowEffect
    {
        BlurRadius = 10,
        ShadowDepth = 0
    };

    public static DropShadowEffect dropShadowRectangle = new DropShadowEffect
    {
        BlurRadius = 20,
        ShadowDepth = 0
    };

    public static TextBlock CloneTextBlock(TextBlock original, bool isBold = true)
    {
        return new TextBlock
        {
            Text = original.Text,
            FontWeight = isBold ? FontWeights.Bold : FontWeights.Regular,
            FontSize = original.FontSize,
            Foreground = original.Foreground.Clone(), // Clone the brush if needed
            HorizontalAlignment = original.HorizontalAlignment,
            VerticalAlignment = original.VerticalAlignment,
            Margin = original.Margin,
            Effect = original.Effect,
            // Add other properties that you need to copy as well...
        };
    }
}