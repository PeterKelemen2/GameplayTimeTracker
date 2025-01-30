using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;

namespace GameplayTimeTracker;

public class ImageHelper
{
    static int numImages = 20;
    static double scaleMax = 1.2;
    static double scaleMin = 0.9;
    static double brightnessMax = 1.0;
    static double brightnessMin = 0.3;

    private static int step;

    private static float scatterY(int x, int scale, int hMod)
    {
        float y = (float)(Math.Sin(x + 2) / (0.2 * (x + 2)));
        Console.WriteLine($"{x} | {y}");
        return y * scale + hMod;
    }

    private static float posX(int x)
    {
        return (float)(x / Common.HeroSize.Width);
    }

    private static double scaleModifier(int i)
    {
        return scaleMax - (i / (double)(numImages - 1) * (scaleMax - scaleMin));
    }

    private static double brightnessModifier(int i)
    {
        return brightnessMax - (i / (double)(numImages - 1) * (brightnessMax - brightnessMin));
    }

    public static void ScatterImage(string inputPath, string outputPath)
    {
        Random random = new Random();
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Size canvasSize = Common.HeroSize;
        Size targetSize = new Size(256, 256);
        step = Common.HeroSize.Width / numImages;

        using (System.Drawing.Image originalImg = System.Drawing.Image.FromFile(inputPath))
        using (Bitmap resizedImg = new Bitmap(targetSize.Width, targetSize.Height))
        using (Graphics resizeGraphics = Graphics.FromImage(resizedImg))
        using (Bitmap canvas = new Bitmap(canvasSize.Width, canvasSize.Height))
        using (Graphics g = Graphics.FromImage(canvas))
        {
            resizeGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resizeGraphics.DrawImage(originalImg, new Rectangle(0, 0, targetSize.Width, targetSize.Height));

            g.Clear(Color.Transparent); // Transparent background

            for (int i = numImages; i > 0; i--)
            {
                int newWidth = (int)(resizedImg.Width * scaleModifier(i));
                int newHeight = (int)(resizedImg.Height * scaleModifier(i));

                Bitmap adjustedImg = AdjustBrightness(resizedImg, (float)brightnessModifier(i));

                GraphicsState state = g.Save();
                g.TranslateTransform(step * i, //+ newWidth / 2,
                    scatterY(i, 200, 50) + newHeight / 2); // Move to image center
                g.RotateTransform((float)random.NextDouble() * 360); // Apply rotation
                g.DrawImage(adjustedImg, -newWidth / 2, -newHeight / 2, newWidth, newHeight); // Draw rotated image
                g.Restore(state);
            }

            canvas.Save(outputPath, ImageFormat.Png);
            Console.WriteLine($"Scattered and blurred image saved to {outputPath}");
        }

        stopwatch.Stop();
        Console.WriteLine($"Cycle took {stopwatch.Elapsed.TotalMilliseconds:F2}ms");
    }

    private static Bitmap AdjustBrightness(Bitmap image, float brightnessFactor)
    {
        Bitmap adjustedImage = new Bitmap(image);

        for (int y = 0; y < adjustedImage.Height; y++)
        {
            for (int x = 0; x < adjustedImage.Width; x++)
            {
                Color pixelColor = adjustedImage.GetPixel(x, y);

                int r = (int)(pixelColor.R * brightnessFactor);
                int g = (int)(pixelColor.G * brightnessFactor);
                int b = (int)(pixelColor.B * brightnessFactor);

                r = Math.Min(255, Math.Max(0, r));
                g = Math.Min(255, Math.Max(0, g));
                b = Math.Min(255, Math.Max(0, b));

                adjustedImage.SetPixel(x, y, Color.FromArgb(pixelColor.A, r, g, b));
            }
        }

        return adjustedImage;
    }
}