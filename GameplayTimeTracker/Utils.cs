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
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using TextBox = System.Windows.Controls.TextBox;

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
    public static Color ShadowColor = (Color)ColorConverter.ConvertFromString("#292929");

    public const int TextMargin = 10;
    public const int TitleFontSize = 17;
    public const int TextFontSize = 14;
    public const double THeight = 150;
    public const double BorderRadius = 10;
    public const int MenuTopMargin = -20;
    public const int TextBoxHeight = 28;
    public const int EditTextMaxHeight = 30;
    public const int EditFirstRowTopMargin = 20;
    public const int EditSecondRowTopMargin = 60;

    public static BlurEffect fakeShadow = new BlurEffect
    {
        Radius = 8
    };

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
    
    public static DropShadowEffect dropShadowLightArea = new DropShadowEffect
    {
        BlurRadius = 5,
        ShadowDepth = 0
    };

    public static DropShadowEffect dropShadowRectangle = new DropShadowEffect
    {
        BlurRadius = 20,
        ShadowDepth = 0
    };


    public static TextBlock NewTextBlock()
    {
        var sampleTextBlock = new TextBlock
        {
            Text = "",
            FontWeight = FontWeights.Bold,
            FontSize = TextFontSize,
            Foreground = new SolidColorBrush(FontColor),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin =
                new Thickness(0, 0, 0, 0),
            Effect = dropShadowText,
        };

        return sampleTextBlock;
    }

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

    public static TextBox NewTextBoxEdit()
    {
        var sampleTextBoxEdit = new TextBox
        {
            Text = "Text",
            Width = 50,
            MaxHeight = 0,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            TextAlignment = TextAlignment.Left,
            HorizontalContentAlignment = HorizontalAlignment.Left, // Center-align content horizontally
            VerticalContentAlignment = VerticalAlignment.Center,
            Effect = Utils.dropShadowIcon
        };

        return sampleTextBoxEdit;
    }

    public static TextBox CloneTextBoxEdit(TextBox original)
    {
        return new TextBox
        {
            Text = original.Text,
            Width = original.Width,
            MaxHeight = original.MaxHeight,
            HorizontalAlignment = original.HorizontalAlignment,
            VerticalAlignment = original.VerticalAlignment,
            TextAlignment = original.TextAlignment,
            HorizontalContentAlignment = original.HorizontalContentAlignment,
            VerticalContentAlignment = original.VerticalContentAlignment,
            Effect = Utils.dropShadowIcon,
            Style = original.Style,
        };
    }

    public static BitmapSource ConvertToGrayscale(BitmapSource source)
    {
        var stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;
        var pixels = new byte[stride * source.PixelWidth];

        source.CopyPixels(pixels, stride, 0);

        for (int i = 0; i < pixels.Length; i += 4)
        {
            // this works for PixelFormats.Bgra32
            var blue = pixels[i];
            var green = pixels[i + 1];
            var red = pixels[i + 2];
            var gray = (byte)(0.2126 * red + 0.7152 * green + 0.0722 * blue);
            pixels[i] = gray;
            pixels[i + 1] = gray;
            pixels[i + 2] = gray;
        }

        return BitmapSource.Create(
            source.PixelWidth, source.PixelHeight,
            source.DpiX, source.DpiY,
            source.Format, null, pixels, stride);
    }
}