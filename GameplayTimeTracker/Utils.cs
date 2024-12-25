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

// using HorizontalAlignment = System.Windows.HorizontalAlignment;
// using MessageBox = System.Windows.MessageBox;
// using TextBox = System.Windows.Controls.TextBox;

namespace GameplayTimeTracker;

public class Utils
{
    public static string DocumentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        Assembly.GetExecutingAssembly().GetName().Name);

    private const string SampleImagePath = "assets/no_icon.png";
    public const string EditIcon = "assets/edit.png";
    public const string RemIcon = "assets/remove.png";
    public const string SaveIcon = "assets/save.png";
    public const string FolderIcon = "assets/folder.png";
    public const string AddIcon = "assets/add.png";

    // public const string AppIconPath = "assets/GameplayTimeTracker.ico";
    private const string SettingsFileName = "settings.json";
    private const string DataFileName = "data.json";
    private const string BackupDataFileName = "backup_data.json";
    private const string SavedIconsFolderName = "Saved Icons";
    private const string BackupFolderName = "Backup Data";
    public static string DataFilePath = Path.Combine(DocumentsPath, DataFileName);
    public static string BackupDataFilePath = Path.Combine(DocumentsPath, BackupDataFileName);
    public static string SettingsFilePath = Path.Combine(DocumentsPath, SettingsFileName);
    public static string SavedIconsPath = Path.Combine(DocumentsPath, SavedIconsFolderName);
    public static string BackupDataFolder = Path.Combine(DocumentsPath, BackupFolderName);

    public static Color BgColor = (Color)ColorConverter.ConvertFromString("#1E2030");
    public static Color FooterColor = (Color)ColorConverter.ConvertFromString("#1E2030");
    public static Color DarkColor = (Color)ColorConverter.ConvertFromString("#1E2030");
    public static Color LightColor = (Color)ColorConverter.ConvertFromString("#2E324A");
    public static Color FontColor = (Color)ColorConverter.ConvertFromString("#DAE4FF");
    public static Color RunningColor = (Color)ColorConverter.ConvertFromString("#C3E88D");

    public static Color DefButtonColor = (Color)ColorConverter.ConvertFromString("#3BC9E3");
    public static Color DefButtonColorHover = (Color)ColorConverter.ConvertFromString("#ADD8E6");
    public static Color DefButtonColorPress = (Color)ColorConverter.ConvertFromString("#199BB3");


    public static Color PositiveButtonColor = (Color)ColorConverter.ConvertFromString("#90EE90");
    public static Color PositiveButtonColorHover = (Color)ColorConverter.ConvertFromString("#B5FFB5");
    public static Color PositiveButtonColorPress = (Color)ColorConverter.ConvertFromString("#489447");

    public static Color NegativeButtonColor = (Color)ColorConverter.ConvertFromString("#ED0C0C");
    public static Color NegativeButtonColorHover = (Color)ColorConverter.ConvertFromString("#E33B3B");
    public static Color NegativeButtonColorPress = (Color)ColorConverter.ConvertFromString("#A30202");

    public static Color LeftColor = (Color)ColorConverter.ConvertFromString("#89ACF2");
    public static Color RightColor = (Color)ColorConverter.ConvertFromString("#B7BDF8");
    public static Color TileColor1 = (Color)ColorConverter.ConvertFromString("#414769");
    public static Color TileColor2 = (Color)ColorConverter.ConvertFromString("#2E324A");
    public static Color ShadowColor = (Color)ColorConverter.ConvertFromString("#151515");
    public static Color EditColor1 = (Color)ColorConverter.ConvertFromString("#7DD6EB");
    public static Color EditColor2 = (Color)ColorConverter.ConvertFromString("#7EAFE0");

    public static Color ButtonColor = (Color)ColorConverter.ConvertFromString("#3BC9E3");

    public static bool toUpdate = false;

    public static System.Windows.Window mainWindow = Application.Current.MainWindow;

    public static Window GetMainWindow()
    {
        return mainWindow;
    }

    public static Dictionary<String, String> GetDefaultColors()
    {
        Dictionary<String, String> colors = new Dictionary<string, string>
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
            { "button", $"{DefButtonColor.ToString()}" },
            { "buttonHover", $"{DefButtonColorHover.ToString()}" },
            { "positiveButton", $"{PositiveButtonColor.ToString()}" },
            { "positiveButtonHover", $"{PositiveButtonColorHover.ToString()}" },
            { "negativeButton", $"{NegativeButtonColor.ToString()}" },
            { "negativeButtonHover", $"{NegativeButtonColorHover.ToString()}" },
        };
        return colors;
    }

    public static Dictionary<String, String> GetPinkColors()
    {
        Dictionary<String, String> colors = new Dictionary<string, string>
        {
            { "bgColor", "#45092b" },
            { "tileColor1", "#db1484" },
            { "tileColor2", "#7d2055" },
            { "leftColor", "#89ACF2" },
            { "rightColor", "#B7BDF8" },
            { "editColor1", "#7DD6EB" },
            { "editColor2", "#7DD6EB" },
            { "shadowColor", "#151515" },
            { "fontColor", "#DAE4FF" },
            { "runningColor", "#C3E88D" },
            { "footerColor", "#90EE90" },
            { "button", $"{DefButtonColor.ToString()}" },
            { "buttonHover", $"{DefButtonColorHover.ToString()}" },
            { "positiveButton", $"{PositiveButtonColor.ToString()}" },
            { "positiveButtonHover", $"{PositiveButtonColorHover.ToString()}" },
            { "negativeButton", $"{NegativeButtonColor.ToString()}" },
            { "negativeButtonHover", $"{NegativeButtonColorHover.ToString()}" },
        };

        return colors;
    }

    public static Dictionary<String, String> GetCustomColors()
    {
        Dictionary<String, String> colors = new Dictionary<string, string>
        {
            { "bgColor", "#FF8B008B" },
            { "tileColor1", "#FFD958FF" },
            { "tileColor2", "#FF00DDE0" },
            { "leftColor", "#FF7FFFD4" },
            { "rightColor", "#B7BDF8" },
            { "editColor1", "#7DD6EB" },
            { "editColor2", "#FF6A5ACD" },
            { "shadowColor", "#151515" },
            { "fontColor", "#FFF0F8FF" },
            { "runningColor", "#86DBFF" },
            { "footerColor", "#FF7B5EFF" },
            { "button", $"{DefButtonColor.ToString()}" },
            { "buttonHover", $"{DefButtonColorHover.ToString()}" },
            { "positiveButton", $"{PositiveButtonColor.ToString()}" },
            { "positiveButtonHover", $"{PositiveButtonColorHover.ToString()}" },
            { "negativeButton", $"{NegativeButtonColor.ToString()}" },
            { "negativeButtonHover", $"{NegativeButtonColorHover.ToString()}" },
        };

        return colors;
    }

    public static Dictionary<String, String> CheckThemeIntegrity(Dictionary<String, String> theme)
    {
        var defaultColors = GetDefaultColors();
        foreach (var key in defaultColors.Keys)
        {
            if (!theme.ContainsKey(key)) theme.Add(key, defaultColors[key]);
        }

        return theme;
    }

    public static LinearGradientBrush createLinGradBrushVer(Color c1, Color c2)
    {
        LinearGradientBrush brush = new LinearGradientBrush();
        brush.StartPoint = new Point(0, 0);
        brush.EndPoint = new Point(0, 1);
        brush.GradientStops.Add(new GradientStop(c1, 0.0));
        brush.GradientStops.Add(new GradientStop(c2, 1.0));
        return brush;
    }

    public static LinearGradientBrush createLinGradBrushHor(Color c1, Color c2)
    {
        LinearGradientBrush brush = new LinearGradientBrush();
        brush.StartPoint = new Point(0, 0);
        brush.EndPoint = new Point(1, 0);
        brush.GradientStops.Add(new GradientStop(c1, 0.0));
        brush.GradientStops.Add(new GradientStop(c2, 1.0));
        return brush;
    }

    public static void SetColors(Dictionary<string, string> colors)
    {
        try
        {
            BgColor = (Color)ColorConverter.ConvertFromString(colors["bgColor"]);
            FooterColor = (Color)ColorConverter.ConvertFromString(colors["footerColor"]);
            FontColor = (Color)ColorConverter.ConvertFromString(colors["fontColor"]);
            RunningColor = (Color)ColorConverter.ConvertFromString(colors["runningColor"]);
            LeftColor = (Color)ColorConverter.ConvertFromString(colors["leftColor"]);
            RightColor = (Color)ColorConverter.ConvertFromString(colors["rightColor"]);
            TileColor1 = (Color)ColorConverter.ConvertFromString(colors["tileColor1"]);
            TileColor2 = (Color)ColorConverter.ConvertFromString(colors["tileColor2"]);
            ShadowColor = (Color)ColorConverter.ConvertFromString(colors["shadowColor"]);
            EditColor1 = (Color)ColorConverter.ConvertFromString(colors["editColor1"]);
            EditColor2 = (Color)ColorConverter.ConvertFromString(colors["editColor2"]);
            DefButtonColor = (Color)ColorConverter.ConvertFromString(colors["button"]);
            DefButtonColorHover = (Color)ColorConverter.ConvertFromString(colors["buttonHover"]);
            PositiveButtonColor = (Color)ColorConverter.ConvertFromString(colors["positiveButton"]);
            PositiveButtonColorHover = (Color)ColorConverter.ConvertFromString(colors["positiveButtonHover"]);
            NegativeButtonColor = (Color)ColorConverter.ConvertFromString(colors["negativeButton"]);
            NegativeButtonColorHover = (Color)ColorConverter.ConvertFromString(colors["negativeButtonHover"]);

            // { "button", $"{DefButtonColor.ToString()}" },
            // { "buttonHover", $"{DefButtonColorHover.ToString()}" },
            // { "positiveButton", $"{PositiveButtonColor.ToString()}" },
            // { "positiveButtonHover", $"{PositiveButtonColorHover.ToString()}" },
            // { "negativeButton", $"{NegativeButtonColor.ToString()}" },
            // { "negativeButtonHover", $"{NegativeButtonColorHover.ToString()}"},
            toUpdate = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            BgColor = (Color)ColorConverter.ConvertFromString("#45092b");
            FooterColor = (Color)ColorConverter.ConvertFromString("#90EE90");
            FontColor = (Color)ColorConverter.ConvertFromString("#DAE4FF");
            RunningColor = (Color)ColorConverter.ConvertFromString("#C3E88D");
            LeftColor = (Color)ColorConverter.ConvertFromString("#89ACF2");
            RightColor = (Color)ColorConverter.ConvertFromString("#B7BDF8");
            TileColor1 = (Color)ColorConverter.ConvertFromString("#414769");
            TileColor2 = (Color)ColorConverter.ConvertFromString("#2E324A");
            ShadowColor = (Color)ColorConverter.ConvertFromString("#151515");
            EditColor1 = (Color)ColorConverter.ConvertFromString("#7DD6EB");
            EditColor2 = (Color)ColorConverter.ConvertFromString("#7EAFE0");
            DefButtonColor = (Color)ColorConverter.ConvertFromString(colors["button"]);
            DefButtonColorHover = (Color)ColorConverter.ConvertFromString(colors["buttonHover"]);
            PositiveButtonColor = (Color)ColorConverter.ConvertFromString(colors["positiveButton"]);
            PositiveButtonColorHover = (Color)ColorConverter.ConvertFromString(colors["positiveButtonHover"]);
            NegativeButtonColor = (Color)ColorConverter.ConvertFromString(colors["negativeButton"]);
            NegativeButtonColorHover = (Color)ColorConverter.ConvertFromString(colors["negativeButtonHover"]);
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

    // Effects for global usage
    public static BlurEffect fakeShadow = new BlurEffect
    {
        Radius = 8,
    };

    public static BlurEffect blurEffect = new BlurEffect
    {
        Radius = 10,
        RenderingBias = RenderingBias.Performance
    };

    public static BlurEffect bigBlurEffect = new BlurEffect
    {
        Radius = 30,
        RenderingBias = RenderingBias.Performance
    };

    public static OuterGlowBitmapEffect outerGlowEffect = new OuterGlowBitmapEffect
    {
        GlowSize = 10
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

    public static DoubleAnimation dragFadeInAnimation = new DoubleAnimation
    {
        From = 0,
        To = 1,
        Duration = new Duration(TimeSpan.FromSeconds(0.2)),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };

    public static DoubleAnimation dragFadeOutAnimation = new DoubleAnimation
    {
        From = 1,
        To = 0,
        Duration = new Duration(TimeSpan.FromSeconds(0.2)),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };

    // public static void SaveBitmapSourceToFile(BitmapSource bitmapSource, string filePath)
    // {
    //     // Create an encoder for the format you want to save (e.g., PNG)
    //     PngBitmapEncoder encoder = new PngBitmapEncoder();
    //     encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
    //
    //     // Save the bitmap to the specified file
    //     using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
    //     {
    //         encoder.Save(fileStream);
    //     }
    // }

    public static String GetPrettyVarName(string variableName)
    {
        // Use Regex to split on uppercase letters
        string result = Regex.Replace(variableName, "([a-z])([A-Z])", "$1 $2");

        // Capitalize the first letter of the entire result
        result = char.ToUpper(result[0]) + result.Substring(1);

        // Replace 'Bg' with 'Background'
        result = result.Replace("Bg", "Background");

        return result;
    }

    public static ThicknessAnimation GetMarginTopBottomAnimation(FrameworkElement uiElement, double duration)
    {
        Thickness m = uiElement.Margin;
        return new ThicknessAnimation
        {
            From = new Thickness(m.Left, m.Top, m.Right, m.Bottom),
            To = new Thickness(m.Left, 0, m.Right, 0),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut },
            Duration = TimeSpan.FromSeconds(duration)
        };
    }

    public static BitmapSource CropTransparentAreas(BitmapSource bitmap)
    {
        int width = bitmap.PixelWidth;
        int height = bitmap.PixelHeight;
        int bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;

        // Create a buffer to hold the pixels
        byte[] pixels = new byte[width * height * bytesPerPixel];
        bitmap.CopyPixels(pixels, width * bytesPerPixel, 0);

        // Find the bounds of the non-transparent pixels
        int minX = width, minY = height, maxX = 0, maxY = 0;
        bool hasNonTransparent = false;

        // Scan the pixels to find non-transparent bounds
        Parallel.For(0, height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * width + x) * bytesPerPixel;
                byte alpha = pixels[index + 3]; // Assuming the format has alpha channel (e.g., BGRA)

                if (alpha > 0) // Check if the pixel is not transparent
                {
                    hasNonTransparent = true;
                    lock (pixels) // Use lock for shared state
                    {
                        if (x < minX) minX = x;
                        if (y < minY) minY = y;
                        if (x > maxX) maxX = x;
                        if (y > maxY) maxY = y;
                    }
                }
            }
        });

        // Early exit if the image is fully transparent
        if (!hasNonTransparent)
        {
            return bitmap;
        }

        // Calculate new width and height
        int croppedWidth = maxX - minX + 1;
        int croppedHeight = maxY - minY + 1;

        // Create a new pixel buffer for the cropped image
        byte[] croppedPixels = new byte[croppedWidth * croppedHeight * bytesPerPixel];

        // Copy the non-transparent pixels to the new buffer
        for (int y = 0; y < croppedHeight; y++)
        {
            for (int x = 0; x < croppedWidth; x++)
            {
                int originalIndex = ((minY + y) * width + (minX + x)) * bytesPerPixel;
                int croppedIndex = (y * croppedWidth + x) * bytesPerPixel;

                Array.Copy(pixels, originalIndex, croppedPixels, croppedIndex, bytesPerPixel);
            }
        }

        // Create the cropped BitmapSource
        BitmapSource croppedBitmap = BitmapSource.Create(croppedWidth, croppedHeight, bitmap.DpiX, bitmap.DpiY,
            bitmap.Format, null, croppedPixels, croppedWidth * bytesPerPixel);

        return croppedBitmap;
    }

    // Returns a render of the main window
    public static BitmapSource CaptureCurrentWindow()
    {
        // Define the width and height of the bitmap, matching the element's size
        int width = (int)mainWindow.ActualWidth;
        int height = (int)mainWindow.ActualHeight;

        // Create a RenderTargetBitmap with the dimensions of the target element
        RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
            width, height, 96, 96, PixelFormats.Pbgra32);

        // Render the target element onto the bitmap
        renderBitmap.Render(mainWindow);

        return renderBitmap;
    }

    // Returns a render of the container grid
    public static BitmapSource CaptureContainerGrid(double scaleFactor = 0.35)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Restart();
        var contentGrid = (Grid)mainWindow.FindName("ContainerGrid");
        contentGrid = (Grid)contentGrid.FindName("Grid");

        int renderWidth = (int)(mainWindow.ActualWidth * scaleFactor);
        int renderHeight = (int)(mainWindow.ActualHeight * scaleFactor);

        RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
            renderWidth, renderHeight, 96 * scaleFactor, 96 * scaleFactor, PixelFormats.Pbgra32);
        renderBitmap.Render(contentGrid);

        stopwatch.Stop();
        Console.WriteLine($"Rendering content grid: {stopwatch.Elapsed.TotalMilliseconds.ToString("F2")}ms");
        return renderBitmap;
    }

    public static String GetPrettyTime(double totalHours)
    {
        (double h, double m, double s) = ConvertDoubleToTime(totalHours);
        String prettyTime = $"{h}h {m}m {s}s";
        return prettyTime;
    }

    public static (int Hours, int Minutes, int Seconds) ConvertDoubleToTime(double totalHours)
    {
        int hours = (int)totalHours; // Extract the whole number part for hours
        double totalMinutes = (totalHours - hours) * 60; // Convert remaining fraction to minutes
        int minutes = (int)totalMinutes; // Extract the whole number part for minutes
        int seconds = (int)((totalMinutes - minutes) * 60); // Convert remaining fraction to seconds

        return (hours, minutes, seconds);
    }
    
    public static (int Hours, int Minutes, int Seconds) ConvertMinutesToTime(double totalMinutes)
    {
        int hours = (int)(totalMinutes / 60); // Extract the whole number part for hours
        double remainingMinutes = totalMinutes % 60; // Get the remaining minutes
        int minutes = (int)remainingMinutes; // Extract the whole number part for minutes
        int seconds = (int)((remainingMinutes - minutes) * 60); // Convert fractional minutes to seconds

        return (hours, minutes, seconds);
    }
    
    // Regex handling of the edited time textfield
    public static (double, double, double) DecodeTimeString(string timeString, double prevH, double prevM, double prevS)
    {
        if (string.IsNullOrWhiteSpace(timeString))
        {
            return (prevH, prevM, prevS);
        }

        // Updated pattern to capture hours, minutes, and seconds in various formats:
        // Examples: "23-56-30", "23h 56m 30s", "23:56:30", or "23h56m30s"
        string pattern = @"(?:(\d+)[h\-:]?)?\s*(?:(\d+)[m\-:]?)?\s*(?:(\d+)[s]?)?";

        // Match the input string against the pattern
        Match match = Regex.Match(timeString, pattern);

        double h = prevH;
        double m = prevM;
        double s = prevS;

        if (match.Success)
        {
            // Extract the matched numbers and convert them to double
            h = match.Groups[1].Success ? double.Parse(match.Groups[1].Value) : prevH;
            m = match.Groups[2].Success ? double.Parse(match.Groups[2].Value) : prevM;
            s = match.Groups[3].Success ? double.Parse(match.Groups[3].Value) : prevS;

            // Normalize seconds and minutes if necessary
            if (s > 59)
            {
                m += Math.Floor(s / 60);
                s %= 60;
            }

            if (m > 59)
            {
                h += Math.Floor(m / 60);
                m %= 60;
            }

            Console.WriteLine($"{timeString} Decoded as: {h}h {m}m {s}s");
            return (h, m, s);
        }
        else
        {
            throw new FormatException("Input does not contain valid numbers in the expected format.");
        }
    }


    // Check if an image is available for usage
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

    /*
     * If the output image path doesn't exist decide whether the filepath if an executable of image file.
     * This is used for both extracting images from the exe and updating bg image later on.
     */
    public static void PrepIcon(string filePath, string? outputImagePath)
    {
        try
        {
            if (!File.Exists(outputImagePath)) //  && IsValidImage(outputImagePath)
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

    // Template     
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
    
    public static double GetTextHeight(TextBlock textBlock)
    {
        // Create a FormattedText object
        var formattedText = new FormattedText(
            textBlock.Text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
            textBlock.FontSize,
            textBlock.Foreground,
            VisualTreeHelper.GetDpi(textBlock).PixelsPerDip);

        // Apply the TextBlock's width for wrapping
        formattedText.MaxTextWidth = textBlock.Width;

        // Calculate the line count
        return formattedText.MaxTextHeight;
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