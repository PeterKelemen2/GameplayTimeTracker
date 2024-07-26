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

            tileContainer.AddTile(new Tile(tileContainer, "Gameasd", 3241, 1233));
            tileContainer.ListTiles();
            // WriteToJson(tileContainer, "data.json");
            // tileContainer.WriteContentToFile(jsonFilePath);
            handler.WriteContentToFile(tileContainer, jsonFilePath);
        }

        public void WriteToJson(TileContainer container, string filePath)
        {
            // File.WriteAllText(filePath, JsonSerializer.Serialize(container.GetParamsOfTiles()));
            container.GetParamsOfTiles();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // tileContainer.AddTile(new Tile(tileContainer, "Game1", 3241, 1233));
            // tileContainer.AddTile(new Tile(tileContainer, "Game1", 120, 40));
            // tileContainer.AddTile(new Tile(tileContainer, "Game2", 300, 20));
            // tileContainer.AddTile(new Tile(tileContainer, "Game3", 50, 25));
            // tileContainer.AddTile(new Tile(tileContainer, "Game4", 60, 15));
            // tileContainer.AddTile(new Tile(tileContainer, "Game5", 60, 15));
            // tileContainer.ListTiles();
            //
            // tileContainer.RemoveTileById(2);
            // tileContainer.ListTiles();
            //
            // tileContainer.AddTile(new Tile(tileContainer, "GameX"));
            //
            // tileContainer.ListTiles();
            //
            // tileContainer.UpdateTileById(1, "Text", "New Value");
            // tileContainer.ListTiles();
            // ShowTilesOnCanvas();
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