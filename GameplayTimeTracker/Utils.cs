using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using Toolbelt.Drawing;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace GameplayTimeTracker;

public class Utils
{
    public static Color BgColor = (Color)ColorConverter.ConvertFromString("#1E2030");
    public static Color FooterColor = (Color)ColorConverter.ConvertFromString("#1E2030");
    public static Color DarkColor = (Color)ColorConverter.ConvertFromString("#1E2030");
    public static Color LightColor = (Color)ColorConverter.ConvertFromString("#2E324A");
    public static Color FontColor = (Color)ColorConverter.ConvertFromString("#DAE4FF");
    public static Color RunningColor = (Color)ColorConverter.ConvertFromString("#C3E88D");
    public static Color LeftColor = (Color)ColorConverter.ConvertFromString("#89ACF2");
    public static Color RightColor = (Color)ColorConverter.ConvertFromString("#B7BDF8");
    public static Color TileColor1 = (Color)ColorConverter.ConvertFromString("#414769");
    public static Color TileColor2 = (Color)ColorConverter.ConvertFromString("#2E324A");
    public static Color ShadowColor = (Color)ColorConverter.ConvertFromString("#151515");
    public static Color EditColor1 = (Color)ColorConverter.ConvertFromString("#7DD6EB");

    public static Color EditColor2 = (Color)ColorConverter.ConvertFromString("#7EAFE0");
    // public static Color EditColor1 = (Color)ColorConverter.ConvertFromString("#328fa8");
    // public static Color EditColor2 = (Color)ColorConverter.ConvertFromString("#3279a8");

    public static System.Windows.Window mainWindow = Application.Current.MainWindow;

    public static void SetColors(Dictionary<string, string> colors)
    {
        try
        {
            BgColor = (Color)ColorConverter.ConvertFromString(colors["bgColor"]);
            BgColor = (Color)ColorConverter.ConvertFromString(colors["footerColor"]);
            DarkColor = (Color)ColorConverter.ConvertFromString(colors["darkColor"]);
            LightColor = (Color)ColorConverter.ConvertFromString(colors["lightColor"]);
            FontColor = (Color)ColorConverter.ConvertFromString(colors["fontColor"]);
            RunningColor = (Color)ColorConverter.ConvertFromString(colors["runningColor"]);
            LeftColor = (Color)ColorConverter.ConvertFromString(colors["leftColor"]);
            RightColor = (Color)ColorConverter.ConvertFromString(colors["rightColor"]);
            TileColor1 = (Color)ColorConverter.ConvertFromString(colors["tileColor1"]);
            TileColor2 = (Color)ColorConverter.ConvertFromString(colors["tileColor2"]);
            ShadowColor = (Color)ColorConverter.ConvertFromString(colors["shadowColor"]);
            EditColor1 = (Color)ColorConverter.ConvertFromString(colors["editColor1"]);
            EditColor2 = (Color)ColorConverter.ConvertFromString(colors["editColor2"]);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            BgColor = (Color)ColorConverter.ConvertFromString("#45092b");
            FooterColor = (Color)ColorConverter.ConvertFromString("#90EE90");
            DarkColor = (Color)ColorConverter.ConvertFromString("#1E2030");
            DarkColor = (Color)ColorConverter.ConvertFromString("#1E2030");
            LightColor = (Color)ColorConverter.ConvertFromString("#2E324A");
            FontColor = (Color)ColorConverter.ConvertFromString("#DAE4FF");
            RunningColor = (Color)ColorConverter.ConvertFromString("#C3E88D");
            LeftColor = (Color)ColorConverter.ConvertFromString("#89ACF2");
            RightColor = (Color)ColorConverter.ConvertFromString("#B7BDF8");
            TileColor1 = (Color)ColorConverter.ConvertFromString("#414769");
            TileColor2 = (Color)ColorConverter.ConvertFromString("#2E324A");
            ShadowColor = (Color)ColorConverter.ConvertFromString("#151515");
            EditColor1 = (Color)ColorConverter.ConvertFromString("#7DD6EB");
            EditColor2 = (Color)ColorConverter.ConvertFromString("#7EAFE0");
        }
    }


    public const int TextMargin = 10;
    public const int TileLeftMargin = 7;
    public const int TitleFontSize = 17;
    public const int TextFontSize = 14;
    public const double THeight = 150;
    public const double BorderRadius = 10;
    public const int MenuTopMargin = -20;
    public const int TextBoxHeight = 28;
    public const int EditTextMaxHeight = 30;
    public const int EditFirstRowTopMargin = 20;
    public const int EditSecondRowTopMargin = 60;
    public const int EditThirdRowTopMargin = 100;
    public const int EditFColLeft = 50;
    public const int EditSColLeft = 190;
    public const int EditColTop = 10;
    public const int SettingsRadius = 20;


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
        BlurRadius = 7,
        ShadowDepth = 0
    };

    public static DropShadowEffect dropShadowRectangle = new DropShadowEffect
    {
        BlurRadius = 20,
        ShadowDepth = 0
    };


    public static BitmapSource CaptureCurrentWindow()
    {
        // Define the width and height of the bitmap, matching the element's size
        int width = 800;
        int height = 650;

        // Create a RenderTargetBitmap with the dimensions of the target element
        RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
            width, height, 96, 96, PixelFormats.Pbgra32);

        // Render the target element onto the bitmap
        renderBitmap.Render(mainWindow);

        // Optionally, encode the bitmap to save as an image
        // Example: Save to PNG
        BitmapEncoder encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

        using (var fileStream = new System.IO.FileStream("CapturedWindow.png", System.IO.FileMode.Create))
        {
            encoder.Save(fileStream);
        }

        // Return the bitmap
        return renderBitmap;
    }

    public static (double, double) DecodeTimeString(string timeString, double prevH, double prevM)
    {
        if (string.IsNullOrWhiteSpace(timeString))
        {
            return (prevH, prevM);
        }

        // Regular expression to match the different formats: 
        // - "23-56", "23h 56m", "23h56m", or "23 56"
        string pattern = @"(\d+)?\D*(\d+)?";

        // string pattern = @"(\d+)[h\- ]?(\d+)[m]?";

        // Match the input string against the pattern
        Match match = Regex.Match(timeString, pattern);

        double h;
        double m;
        if (match.Success)
        {
            // Extract the matched numbers and convert them to double
            h = match.Groups[1].Success ? double.Parse(match.Groups[1].Value) : prevH;
            m = match.Groups[2].Success ? double.Parse(match.Groups[2].Value) : prevM;

            if (m > 59)
            {
                h += Math.Floor(m / 60);
                m %= 60;
            }

            Console.WriteLine($"{timeString} Decoded as: {h} {m}");
            return (h, m);
        }
        else
        {
            throw new FormatException("Input does not contain valid numbers in the expected format.");
        }
    }

    public static bool IsValidImage(string imagePath)
    {
        try
        {
            // Create a BitmapImage object and load the image
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad; // Load the entire image at once
            bitmap.UriSource = new Uri(imagePath, UriKind.Relative);
            bitmap.EndInit();

            // Ensure the image has valid pixel width and height
            if (bitmap.PixelWidth > 0 && bitmap.PixelHeight > 0)
            {
                return true; // The image is valid
            }
        }
        catch (Exception ex)
        {
            // Log or handle the exception if needed
            Console.WriteLine($"Invalid image: {ex.Message}");
        }

        return false; // If any exception occurs, or the image dimensions are invalid
    }

    private static bool IsExecutable(string filePath)
    {
        // Check if the file is an executable
        return Path.GetExtension(filePath)?.ToLower() == ".exe";
    }

    private static bool IsImageFile(string filePath)
    {
        // Check if the file is an image by its extension
        string extension = Path.GetExtension(filePath)?.ToLower();
        return extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".bmp" ||
               extension == ".gif";
    }

    public static void PrepIcon(string filePath, string? outputImagePath)
    {
        try
        {
            if (!(File.Exists(outputImagePath) && IsValidImage(outputImagePath)))
            {
                if (IsExecutable(filePath))
                {
                    using var s = File.Create(outputImagePath);
                    IconExtractor.Extract1stIconTo(filePath, s);
                }

                if (IsImageFile(filePath))
                {
                    File.Copy(filePath, outputImagePath);
                }
            }
        }
        catch (IOException ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

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
            Width = 180,
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