using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;

namespace GameplayTimeTracker;

public class ImageHelper
{
    static int numImages = 20;
    static double scaleMax = 1.1;
    static double scaleMin = 1.1;
    static float brightness = 0.8f;
    static float shadowOffset = 20;
    static float shadowOpacity = 0.4f;
    private static int step;

    private static float scatterY(int x, int scale, int hMod = 0)
    {
        float y = (float)Math.Cos(x);
        // float y = (float)(Math.Sin(x + 2) / (0.2 * (x + 2)));
        Console.WriteLine($"{x} | {y}");
        return y * scale + hMod;
    }

    private static double scaleModifier(int i)
    {
        return scaleMax - (i / (double)(numImages - 1) * (scaleMax - scaleMin));
    }

    public static void ScatterImage(string inputPath, string outputPath)
    {
        Random random = new Random();
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Size canvasSize = Common.HeroSize;
        Size targetSize = new Size(256, 256);
        step = Common.HeroSize.Width / numImages;

        using (Image originalImg = Image.FromFile(inputPath))
        using (Bitmap resizedImg = new Bitmap(targetSize.Width, targetSize.Height))
        using (Graphics resizeGraphics = Graphics.FromImage(resizedImg))
        using (Bitmap canvas = new Bitmap(canvasSize.Width, canvasSize.Height))
        using (Graphics g = Graphics.FromImage(canvas))
        {
            resizeGraphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            resizeGraphics.DrawImage(originalImg, new Rectangle(0, 0, targetSize.Width, targetSize.Height));
            // g.Clear(Color.Transparent);

            for (int i = numImages; i > 0; i--)
            {
                int newWidth = (int)(resizedImg.Width * scaleModifier(i));
                int newHeight = (int)(resizedImg.Height * scaleModifier(i));
                var dy = scatterY(i, 200);
                float rotation = (float)random.NextDouble() * 360;

                // Shadow
                Bitmap shadowImage = AdjustBrightness(resizedImg, shadowOpacity);
                GraphicsState state = g.Save();
                g.TranslateTransform(step * i + shadowOffset, dy + newHeight / 2 + shadowOffset);
                g.RotateTransform(rotation);
                g.DrawImage(shadowImage, -newWidth / 2, -newHeight / 2, newWidth, newHeight);
                g.Restore(state);

                // Image
                Bitmap adjustedImg = AdjustBrightness(resizedImg, brightness);
                state = g.Save();
                g.TranslateTransform(step * i, dy + newHeight / 2);
                // Random rotation to fill up space with shadow version, use rotation var to have actual shadows
                g.RotateTransform((float)random.NextDouble() * 360);
                g.DrawImage(adjustedImg, -newWidth / 2, -newHeight / 2, newWidth, newHeight);
                g.Restore(state);
            }

            canvas.Save(outputPath, ImageFormat.Png);
            Console.WriteLine($"Scattered and blurred image saved to {outputPath}");
        }

        stopwatch.Stop();
        Console.WriteLine($"Generating image took {stopwatch.Elapsed.TotalMilliseconds:F2}ms");
    }

    private static Bitmap AdjustBrightness(Bitmap image, float brightnessFactor)
    {
        Bitmap adjustedImage = new Bitmap(image);

        // Lock the bitmap's bits for fast access
        Rectangle rect = new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height);
        BitmapData data = adjustedImage.LockBits(rect, ImageLockMode.ReadWrite, adjustedImage.PixelFormat);

        int bytesPerPixel = Bitmap.GetPixelFormatSize(adjustedImage.PixelFormat) / 8;
        int byteCount = data.Stride * adjustedImage.Height;
        byte[] pixels = new byte[byteCount];

        // Copy the pixel data into the byte array
        Marshal.Copy(data.Scan0, pixels, 0, byteCount);

        // Adjust brightness in the pixel array
        for (int i = 0; i < pixels.Length; i += bytesPerPixel)
        {
            // The pixel data is arranged in BGRA format for most formats
            byte blue = pixels[i];
            byte green = pixels[i + 1];
            byte red = pixels[i + 2];

            // Adjust the brightness
            red = (byte)Math.Min(255, Math.Max(0, red * brightnessFactor));
            green = (byte)Math.Min(255, Math.Max(0, green * brightnessFactor));
            blue = (byte)Math.Min(255, Math.Max(0, blue * brightnessFactor));

            // Set the new pixel values
            pixels[i] = blue;
            pixels[i + 1] = green;
            pixels[i + 2] = red;
        }

        // Copy the modified byte array back into the bitmap
        Marshal.Copy(pixels, 0, data.Scan0, byteCount);

        // Unlock the bits to apply changes
        adjustedImage.UnlockBits(data);

        return adjustedImage;
    }
}