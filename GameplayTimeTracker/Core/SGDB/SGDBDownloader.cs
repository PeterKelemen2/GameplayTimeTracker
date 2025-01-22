using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GameplayTimeTracker.SGDB;

public class SGDBDownloader
{
    public static async Task DownloadImageAsync(string imageUrl, string outputFilePath, double scaleModifier = 0.5,
        int[] sizeLimits = null)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                // Send a GET request to the image URL
                HttpResponseMessage response = await client.GetAsync(imageUrl);
                response.EnsureSuccessStatusCode(); // Ensure we received a successful response

                // Read the response as a byte array (image content)
                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                // Create an image object from the byte array
                using (MemoryStream ms = new MemoryStream(imageBytes))
                using (Image originalImage = Image.FromStream(ms))
                {
                    if (sizeLimits != null)
                    {
                        // Resize the image
                        if (originalImage.Width > sizeLimits[0] || originalImage.Height > sizeLimits[1])
                        {
                            Console.WriteLine(
                                $"Image size limit exceeded - {originalImage.Width}x{originalImage.Height}");
                            Image resizedImage = ResizeImage(originalImage, sizeLimits[0], sizeLimits[1]);
                            resizedImage.Save(outputFilePath, ImageFormat.Png);
                            Console.WriteLine($"Image successfully downloaded and resized to {outputFilePath}");
                        }
                        else
                        {
                            Image image = new Bitmap(originalImage);
                            image.Save(outputFilePath, ImageFormat.Png);
                            Console.WriteLine($"Image successfully downloaded to {outputFilePath}");
                        }
                    }
                    else
                    {
                        Image image = new Bitmap(originalImage);
                        image.Save(outputFilePath, ImageFormat.Png);
                        Console.WriteLine($"Image successfully downloaded to {outputFilePath}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while downloading or resizing the image: {ex.Message}");
        }
    }

    // Method to resize the image
    private static Image ResizeImage(Image originalImage, int newWidth, int newHeight)
    {
        // Create a new bitmap with the desired dimensions
        Bitmap resizedBitmap = new Bitmap(originalImage, newWidth, newHeight);
        return resizedBitmap;
    }

    public static async Task DownloadAndProcessIcoAsync(string imageUrl, string outputFilePath)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(imageUrl);
                response.EnsureSuccessStatusCode();

                byte[] iconBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(outputFilePath, iconBytes);
                Console.WriteLine($"ICO file downloaded to {outputFilePath}");
            }

            if (Path.GetExtension(outputFilePath).ToLower() == ".ico")
            {
                Console.WriteLine("Processing ICO file...");
                using (FileStream stream = new FileStream(outputFilePath, FileMode.Open, FileAccess.Read))
                using (Icon icon = new Icon(stream))
                {
                    SaveHighestResolutionFrame(icon, outputFilePath);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static void SaveHighestResolutionFrame(Icon icon, string outputFilePath)
    {
        string outputDir = Path.GetDirectoryName(outputFilePath);
        Bitmap highestResBitmap = null;
        int maxResolution = 0;

        using (MemoryStream ms = new MemoryStream())
        {
            icon.Save(ms);
            ms.Position = 0;

            while (ms.Position < ms.Length)
            {
                // Extract each frame in the ICO file
                try
                {
                    Icon frameIcon = new Icon(ms);
                    Bitmap bitmap = frameIcon.ToBitmap();

                    // Check if this frame has the highest resolution
                    int resolution = bitmap.Width * bitmap.Height;
                    if (resolution > maxResolution)
                    {
                        highestResBitmap?.Dispose(); // Dispose of the previous bitmap
                        highestResBitmap = new Bitmap(bitmap);
                        maxResolution = resolution;
                    }

                    bitmap.Dispose(); // Dispose the current bitmap after comparison
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Frame processing error: {ex.Message}");
                    break;
                }
            }
        }

        // Save the highest-resolution bitmap
        if (highestResBitmap != null)
        {
            string highestResOutputPath = Path.Combine(
                outputDir,
                $"{Path.GetFileNameWithoutExtension(outputFilePath)}_highest.png");

            highestResBitmap.Save(highestResOutputPath, ImageFormat.Png);
            Console.WriteLine($"Saved highest resolution frame to {highestResOutputPath}");
            highestResBitmap.Dispose();
        }
        else
        {
            Console.WriteLine("No valid frames found in the ICO file.");
        }
    }
}