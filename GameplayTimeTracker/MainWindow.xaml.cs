using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Toolbelt.Drawing;

namespace GameplayTimeTracker
{
    public partial class MainWindow : Window
    {
        private const double Offset = 8;
        private const string jsonFilePath = "data.json";
        TileContainer tileContainer = new();
        public JsonHandler handler = new();
        ProcessTracker tracker = new();

        public MainWindow()
        {
            handler.InitializeContainer(tileContainer, jsonFilePath);
            // tileContainer.InitializeContainer(jsonFilePath);
            InitializeComponent();
            // Loaded += MainWindow_Loaded;
            Loaded += ShowTilesOnCanvas;

            // tileContainer.AddTile(
            //     new Tile(tileContainer, "Gameasd", 3241, 1233, "C:\\Users\\Peti\\Pictures\\focus.jpg"));
            // tileContainer.AddTile(new Tile(tileContainer, "Gameasd", 3241, 1233));
            tileContainer.ListTiles();
            tileContainer.GetExecutableNames();
            // WriteToJson(tileContainer, "data.json");
            // tileContainer.WriteContentToFile(jsonFilePath);
            handler.WriteContentToFile(tileContainer, jsonFilePath);
            tracker.InitializeProcessTracker(tileContainer);
        }

        private void AddExecButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                // Handle the selected file
                string filePath = openFileDialog.FileName;
                string fileName = Path.GetFileName(filePath);
                fileName = fileName.Substring(0, fileName.Length - 4);

                string uniqueFileName = Guid.NewGuid().ToString() + ".png";
                string? iconPath = $"assets/{uniqueFileName}";

                Console.WriteLine(iconPath);
                PrepIcon(filePath, iconPath);

                Tile newTile = new Tile(tileContainer, fileName, 0, 0, iconPath, exePath: filePath);
                newTile.Margin = new Thickness(Offset, 5, 0, 5);
                tileContainer.AddTile(newTile, newlyAdded: true);
                tileContainer.ListTiles();

                MessageBox.Show($"Selected file: {fileName}");

                var tilesList = tileContainer.GetTiles();
                mainStackPanel.Children.Add(tilesList.Last());
            }

            handler.WriteContentToFile(tileContainer, jsonFilePath);
        }

        private void PrepIcon(string filePath, string? outputImagePath)
        {
            var source = filePath;
            using var s = File.Create(outputImagePath);
            IconExtractor.Extract1stIconTo(source, s);
            // // GetLargestIcon(filePath);
            // // var icon = GetLargestIcon(filePath);
            // var icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
            // if (icon != null)
            // {
            //     string directoryPath = Path.GetDirectoryName(outputImagePath);
            //
            //     // Ensure the directory path is valid and writable
            //     if (string.IsNullOrWhiteSpace(directoryPath) || !Directory.Exists(directoryPath))
            //     {
            //         MessageBox.Show("Directory does not exist or is invalid.");
            //         return;
            //     }
            //
            //     using (MemoryStream iconStream = new MemoryStream())
            //     {
            //         // Save the icon to a MemoryStream
            //         icon.ToBitmap().Save(iconStream, System.Drawing.Imaging.ImageFormat.Png);
            //         iconStream.Seek(0, SeekOrigin.Begin);
            //
            //         // Save the MemoryStream to a file
            //         using (FileStream fileStream = new FileStream(outputImagePath, FileMode.Create, FileAccess.Write))
            //         {
            //             iconStream.WriteTo(fileStream);
            //         }
            //     }
            // }
        }


        private void ShowTilesOnCanvas(object sender, RoutedEventArgs e)
        {
            var tilesList = tileContainer.GetTiles();
            foreach (var tile in tilesList)
            {
                tile.Margin = new Thickness(Offset, 5, 0, 5);
                mainStackPanel.Children.Add(tile);
            }
        }

        public void ShowScrollViewerOverlay(object sender, ScrollChangedEventArgs e)
        {
            OverlayTop.Visibility = e.VerticalOffset > 0 ? Visibility.Visible : Visibility.Collapsed;
            OverlayBottom.Visibility = e.VerticalOffset < ScrollViewer.ScrollableHeight
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}