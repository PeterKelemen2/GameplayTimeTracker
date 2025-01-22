using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SkiaSharp;

namespace GameplayTimeTracker.SGDB
{
    public class SGDBDownloader
    {
        public static async Task DownloadImageAsync(string imageUrl, string outputFilePath, double scaleModifier = 0.5,
            int[] sizeLimits = null)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(imageUrl);
                    response.EnsureSuccessStatusCode();
                    byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        string extension = GetImageExtension(imageBytes);

                        if (extension == ".webp")
                        {
                            ProcessWebPImage(ms, outputFilePath, sizeLimits);
                        }
                        else
                        {
                            using (Image originalImage = Image.FromStream(ms))
                            {
                                if (sizeLimits != null && (originalImage.Width > sizeLimits[0] ||
                                                           originalImage.Height > sizeLimits[1]))
                                {
                                    Console.WriteLine(
                                        $"Image size limit exceeded - {originalImage.Width}x{originalImage.Height}");
                                    Image resizedImage = ResizeImage(originalImage, sizeLimits[0], sizeLimits[1]);
                                    resizedImage.Save(outputFilePath, ImageFormat.Png);
                                    Console.WriteLine($"Image successfully downloaded and resized to {outputFilePath}");
                                }
                                else
                                {
                                    originalImage.Save(outputFilePath, ImageFormat.Png);
                                    Console.WriteLine($"Image successfully downloaded to {outputFilePath}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while downloading or processing the image: {ex}");
            }
        }

        private static void ProcessWebPImage(Stream inputStream, string outputFilePath, int[] sizeLimits)
        {
            inputStream.Position = 0; // Reset stream position
            using (SKBitmap bitmap = SKBitmap.Decode(inputStream))
            {
                if (bitmap == null)
                {
                    Console.WriteLine("Failed to decode WebP image.");
                    return;
                }

                int width = bitmap.Width;
                int height = bitmap.Height;

                if (sizeLimits != null && (width > sizeLimits[0] || height > sizeLimits[1]))
                {
                    float scale = Math.Min((float)sizeLimits[0] / width, (float)sizeLimits[1] / height);
                    width = (int)(bitmap.Width * scale);
                    height = (int)(bitmap.Height * scale);
                }

                using (SKBitmap resizedBitmap = bitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.High))
                using (SKImage image = SKImage.FromBitmap(resizedBitmap ?? bitmap))
                using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    File.WriteAllBytes(outputFilePath, data.ToArray());
                    Console.WriteLine($"WebP image processed and saved to {outputFilePath}");
                }
            }
        }

        private static string GetImageExtension(byte[] imageBytes)
        {
            if (imageBytes.Length < 12) return string.Empty;

            // WebP magic number "RIFF....WEBP"
            if (imageBytes[0] == 'R' && imageBytes[1] == 'I' && imageBytes[2] == 'F' && imageBytes[3] == 'F' &&
                imageBytes[8] == 'W' && imageBytes[9] == 'E' && imageBytes[10] == 'B' && imageBytes[11] == 'P')
            {
                return ".webp";
            }

            return ".png"; // Default to PNG if unrecognized
        }

        private static Image ResizeImage(Image originalImage, int newWidth, int newHeight)
        {
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

                // Process the ICO file to extract the highest resolution frame
                ExtractHighestResolutionFrame(outputFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void ExtractHighestResolutionFrame(string icoFilePath)
        {
            try
            {
                using (FileStream stream = new FileStream(icoFilePath, FileMode.Open, FileAccess.Read))
                using (Icon icon = new Icon(stream))
                {
                    string outputDir = Path.GetDirectoryName(icoFilePath);
                    Bitmap highestResBitmap = null;
                    int maxResolution = 0;

                    // Extract each frame and check for the highest resolution
                    using (MemoryStream ms = new MemoryStream())
                    {
                        icon.Save(ms);
                        ms.Position = 0;

                        while (ms.Position < ms.Length)
                        {
                            try
                            {
                                Icon frameIcon = new Icon(ms);
                                Bitmap bitmap = frameIcon.ToBitmap();

                                int resolution = bitmap.Width * bitmap.Height;
                                if (resolution > maxResolution)
                                {
                                    highestResBitmap?.Dispose();
                                    highestResBitmap = new Bitmap(bitmap);
                                    maxResolution = resolution;
                                }

                                bitmap.Dispose(); // Dispose the current bitmap
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error processing icon frame: {ex.Message}");
                                break;
                            }
                        }
                    }

                    // Save the highest resolution frame
                    if (highestResBitmap != null)
                    {
                        string highestResOutputPath = Path.Combine(outputDir,
                            $"{Path.GetFileNameWithoutExtension(icoFilePath)}_highest.png");
                        highestResBitmap.Save(highestResOutputPath, ImageFormat.Png);
                        highestResBitmap.Dispose();

                        Console.WriteLine($"Highest resolution frame saved to {highestResOutputPath}");
                    }
                    else
                    {
                        Console.WriteLine("No valid frames found in the ICO file.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing ICO file: {ex.Message}");
            }
        }
    }
}