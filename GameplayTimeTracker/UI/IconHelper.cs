using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;

namespace GameplayTimeTracker;

public class IconHelper
{
    private int[] pX =
        { 476, 570, 648, -76, 54, 753, 24, 444, -233, 352, 207, 365, -212, 75, -37, 338, 752, -8, 426, 266 };

    private int[] pY = { 4, 101, 8, 67, 40, 31, 6, 17, 119, 0, 0, 3, 92, 0, 32, 33, 19, 0, 3, 0 };

    private double[] s =
    {
        1.14, 0.79, 0.85, 0.94, 0.97, 0.65, 1.18, 1.05, 0.62, 1.22, 1.39, 0.83, 0.74, 1.22, 1.04, 0.87, 0.71, 1.23,
        1.18, 1.24
    };

    private float[] a =
        { 139, 89, 45, 135, 250, 36, 319, 108, 321, 79, 346, 24, 227, 113, 275, 107, 233, 226, 166, 356 };

    private double[] b =
    {
        0.3000, 0.3263, 0.3526, 0.3789, 0.4053, 0.4316, 0.4579, 0.4842, 0.5105, 0.5368, 0.5632, 0.5895, 0.6158,
        0.6421, 0.6684, 0.6947, 0.7211, 0.7474, 0.7737, 0.8000
    };

    private void ScatterImage(string inputPath, string outputPath, Size canvasSize, int numImages, Size targetSize)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        using (System.Drawing.Image originalImg = System.Drawing.Image.FromFile(inputPath))
        using (Bitmap resizedImg = new Bitmap(targetSize.Width, targetSize.Height))
        using (Graphics resizeGraphics = Graphics.FromImage(resizedImg))
        using (Bitmap canvas = new Bitmap(canvasSize.Width, canvasSize.Height))
        using (Graphics g = Graphics.FromImage(canvas))
        {
            resizeGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resizeGraphics.DrawImage(originalImg, new Rectangle(0, 0, targetSize.Width, targetSize.Height));

            g.Clear(Color.Transparent); // Transparent background

            for (int i = 0; i < numImages; i++)
            {
                int newWidth = (int)(resizedImg.Width * s[i]);
                int newHeight = (int)(resizedImg.Height * s[i]);

                Bitmap adjustedImg = AdjustBrightness(resizedImg, (float)b[i]);

                GraphicsState state = g.Save();
                g.TranslateTransform(pX[i] + newWidth / 2, pY[i] + newHeight / 2); // Move to image center
                g.RotateTransform(a[i]); // Apply rotation
                g.DrawImage(adjustedImg, -newWidth / 2, -newHeight / 2, newWidth, newHeight); // Draw rotated image
                g.Restore(state);
            }

            canvas.Save(outputPath, ImageFormat.Png); 
            Console.WriteLine($"Scattered and blurred image saved to {outputPath}");
        }

        stopwatch.Stop();
        Console.WriteLine($"Cycle took {stopwatch.Elapsed.TotalMilliseconds:F2}ms");
    }

    private Bitmap AdjustBrightness(Bitmap image, float brightnessFactor)
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