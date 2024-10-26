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


    public static (byte[] topRow, byte[] bottomRow, byte[] leftColumn, byte[] rightColumn) GetEdges(BitmapSource bitmap)
    {
        int width = bitmap.PixelWidth;
        int height = bitmap.PixelHeight;
        int bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8; // Bits per pixel rounded to bytes
        int rowStride = width * bytesPerPixel; // Byte width of a single row
        int columnStride = height * bytesPerPixel; // Byte height of a single column

        // Create buffers for top row, bottom row, left column, and right column
        byte[] topRow = new byte[rowStride];
        byte[] bottomRow = new byte[rowStride];
        byte[] leftColumn = new byte[columnStride];
        byte[] rightColumn = new byte[columnStride];

        // Define rectangles for each region
        Int32Rect topRowRect = new Int32Rect(0, 0, width, 1);
        Int32Rect bottomRowRect = new Int32Rect(0, height - 1, width, 1);
        Int32Rect leftColumnRect = new Int32Rect(0, 0, 1, height);
        Int32Rect rightColumnRect = new Int32Rect(width - 1, 0, 1, height);

        // Copy pixels for each region
        bitmap.CopyPixels(topRowRect, topRow, rowStride, 0);
        bitmap.CopyPixels(bottomRowRect, bottomRow, rowStride, 0);
        bitmap.CopyPixels(leftColumnRect, leftColumn, bytesPerPixel, 0);
        bitmap.CopyPixels(rightColumnRect, rightColumn, bytesPerPixel, 0);

        return (topRow, bottomRow, leftColumn, rightColumn);
    }

    public static BitmapSource ExtendEdgesAroundCenter(BitmapSource bitmap, int n)
    {
        BitmapSource cropped = CropTransparentAreas(bitmap);
        bitmap = cropped;

        int originalWidth = bitmap.PixelWidth;
        int originalHeight = bitmap.PixelHeight;
        int extendedWidth = originalWidth + 2 * n;
        int extendedHeight = originalHeight + 2 * n;
        int bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;

        // Ensure n is positive and within image bounds
        if (n < 0 || originalWidth < 1 || originalHeight < 1)
            throw new ArgumentException("Invalid dimensions or extension size.");

        // Get top, bottom, left, and right edges
        var (topRow, bottomRow, leftColumn, rightColumn) = GetEdges(bitmap);

        // Create a buffer for the extended image
        byte[] extendedPixels = new byte[extendedWidth * extendedHeight * bytesPerPixel];

        // Copy the original image into the center of the extended image
        bitmap.CopyPixels(new Int32Rect(0, 0, originalWidth, originalHeight),
            extendedPixels,
            extendedWidth * bytesPerPixel,
            n * extendedWidth * bytesPerPixel + n * bytesPerPixel);

        // Fill top edge
        for (int i = 0; i < n; i++)
        {
            Array.Copy(topRow, 0, extendedPixels, i * extendedWidth * bytesPerPixel + n * bytesPerPixel, topRow.Length);
        }

        // Fill bottom edge
        for (int i = 0; i < n; i++)
        {
            Array.Copy(bottomRow, 0, extendedPixels,
                (extendedHeight - n + i - 1) * extendedWidth * bytesPerPixel + n * bytesPerPixel, bottomRow.Length);
        }

        // Fill left and right edges
        for (int y = 0; y < originalHeight; y++)
        {
            for (int i = 0; i < n; i++)
            {
                // Left edge
                Array.Copy(leftColumn, y * bytesPerPixel, extendedPixels, ((y + n) * extendedWidth + i) * bytesPerPixel,
                    bytesPerPixel);
                // Right edge
                Array.Copy(rightColumn, y * bytesPerPixel, extendedPixels,
                    ((y + n) * extendedWidth + (originalWidth + n + i)) * bytesPerPixel, bytesPerPixel);
            }
        }

        BitmapSource newBitmapSource = BitmapSource.Create(extendedWidth, extendedHeight, bitmap.DpiX, bitmap.DpiY,
            bitmap.Format, null,
            extendedPixels, extendedWidth * bytesPerPixel);
        SaveBitmapSourceToFile(newBitmapSource, "assets/new_sok.png");
        // Return the new extended bitmap
        return BitmapSource.Create(extendedWidth, extendedHeight, bitmap.DpiX, bitmap.DpiY, bitmap.Format, null,
            extendedPixels, extendedWidth * bytesPerPixel);
    }

    public static BlurEffect fakeShadow = new BlurEffect
    {
        Radius = 8,
    };

    public static BlurEffect blurEffect = new BlurEffect
    {
        Radius = 10,
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

    public static void SaveBitmapSourceToFile(BitmapSource bitmapSource, string filePath)
    {
        // Create an encoder for the format you want to save (e.g., PNG)
        PngBitmapEncoder encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

        // Save the bitmap to the specified file
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            encoder.Save(fileStream);
        }
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
        int minX = width;
        int minY = height;
        int maxX = 0;
        int maxY = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * width + x) * bytesPerPixel;
                byte alpha = pixels[index + 3]; // Assuming the format has alpha channel (e.g., BGRA)

                if (alpha > 0) // Check if the pixel is not transparent
                {
                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                    if (x > maxX) maxX = x;
                    if (y > maxY) maxY = y;
                }
            }
        }

        // Calculate new width and height
        int croppedWidth = maxX - minX + 1;
        int croppedHeight = maxY - minY + 1;

        if (croppedWidth <= 0 || croppedHeight <= 0)
        {
            // No non-transparent pixels found; return the original bitmap
            return bitmap;
        }

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