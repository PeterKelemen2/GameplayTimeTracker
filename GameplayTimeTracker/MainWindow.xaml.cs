using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace GameplayTimeTracker
{
    public partial class MainWindow : Window
    {
        private const double Offset = 8;
        private const string jsonFilePath = "data.json";
        TileContainer tileContainer = new();

        public MainWindow()
        {
            JsonHandler handler = new();
            handler.InitializeContainer(tileContainer, jsonFilePath);
            // tileContainer.InitializeContainer(jsonFilePath);
            InitializeComponent();
            // Loaded += MainWindow_Loaded;
            Loaded += ShowTilesOnCanvas;

            tileContainer.AddTile(new Tile(tileContainer, "Gameasd", 3241, 1233, "C:\\Users\\Peti\\Pictures\\focus.jpg"));
            tileContainer.AddTile(new Tile(tileContainer, "Gameasd", 3241, 1233));
            tileContainer.ListTiles();
            // WriteToJson(tileContainer, "data.json");
            // tileContainer.WriteContentToFile(jsonFilePath);
            handler.WriteContentToFile(tileContainer, jsonFilePath);
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
            if (e.VerticalOffset > 0)
            {
                OverlayTop.Visibility = Visibility.Visible;
            }
            else
            {
                OverlayTop.Visibility = Visibility.Collapsed;
            }

            if (e.VerticalOffset < ScrollViewer.ScrollableHeight)
            {
                OverlayBottom.Visibility = Visibility.Visible;
            }
            else
            {
                OverlayBottom.Visibility = Visibility.Collapsed;
            }
        }
    }
}