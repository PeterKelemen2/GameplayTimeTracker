using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Toolbelt.Drawing;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace GameplayTimeTracker
{
    public partial class MainWindow : System.Windows.Window
    {
        private const double Offset = 8;
        private const string jsonFilePath = "data.json";
        private const string? SampleImagePath = "assets/no_icon.png";
        private const string? AppIcon = "assets/MyAppIcon.ico";
        TileContainer tileContainer = new();
        public JsonHandler handler = new();
        ProcessTracker tracker = new();

        private System.Windows.Forms.NotifyIcon m_notifyIcon;

        public MainWindow()
        {
            handler.InitializeContainer(tileContainer, jsonFilePath);
            InitializeComponent();

            Loaded += ShowTilesOnCanvas;

            tileContainer.ListTiles();
            tileContainer.GetExecutableNames();
            // WriteToJson(tileContainer, "data.json");
            // tileContainer.WriteContentToFile(jsonFilePath);
            handler.WriteContentToFile(tileContainer, jsonFilePath);
            tracker.InitializeProcessTracker(tileContainer);

            // tileContainer.UpdateBgImages();

            this.Closing += MainWindow_Closing;

            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "The app has been minimised. Click the tray icon to show.";
            m_notifyIcon.BalloonTipTitle = "Gameplay Time Tracker";
            m_notifyIcon.Text = "Gameplay Time Tracker";
            //TODO: Change this
            m_notifyIcon.Icon =
                new System.Drawing.Icon(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppIcon));
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);
        }

        // ==== Tray ====
        void OnCloseNotify(object sender, CancelEventArgs args)
        {
            // Only dispose if you are exiting
            if (!args.Cancel)
            {
                m_notifyIcon.Dispose();
                m_notifyIcon = null;
            }
        }

        private WindowState m_storedWindowState = WindowState.Normal;

        void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                if (m_notifyIcon != null)
                    m_notifyIcon.ShowBalloonTip(2000);
            }
            else
                m_storedWindowState = WindowState;
        }

        void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (m_notifyIcon != null)
                m_notifyIcon.Visible = !IsVisible;
        }

        void m_notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = m_storedWindowState;
        }

        void ShowTrayIcon(bool show)
        {
            if (m_notifyIcon != null)
                m_notifyIcon.Visible = show;
        }

        // ==== Tray ====

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
                iconPath = IsValidImage(iconPath) ? iconPath : SampleImagePath;

                Tile newTile = new Tile(tileContainer, fileName, 0, 0, iconPath, exePath: filePath);
                newTile.Margin = new Thickness(Offset, 5, 0, 5);

                if (!(Path.GetFileName(filePath).Equals("GameplayTimeTracker.exe") ||
                      Path.GetFileName(filePath).Equals("Gameplay Time Tracker.exe")))
                {
                    tileContainer.AddTile(newTile, newlyAdded: true);
                    tileContainer.ListTiles();

                    MessageBox.Show($"Selected file: {fileName}");

                    var tilesList = tileContainer.GetTiles();
                    mainStackPanel.Children.Add(tilesList.Last());
                }
                else
                {
                    MessageBox.Show("Sorry, I can't keep tabs on myself.", "Existential crisis", MessageBoxButton.OK);
                }
            }

            handler.WriteContentToFile(tileContainer, jsonFilePath);
        }

        private void PrepIcon(string filePath, string? outputImagePath)
        {
            var source = filePath;
            using var s = File.Create(outputImagePath);
            try
            {
                IconExtractor.Extract1stIconTo(source, s);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButton.YesNo) ==
                    MessageBoxResult.No)
                {
                    e.Cancel = true; // Cancel closing
                    // Reinitialize NotifyIcon if it's null
                    if (m_notifyIcon == null)
                    {
                        InitializeNotifyIcon(); // Custom method to reinitialize the NotifyIcon
                    }

                    return; // Exit the method
                }

                // If the user confirmed the exit, proceed with the save logic
                tileContainer?.InitSave();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
                e.Cancel = true; // Cancel closing in case of an error
            }
        }

        private void InitializeNotifyIcon()
        {
            m_notifyIcon = new System.Windows.Forms.NotifyIcon();
            m_notifyIcon.BalloonTipText = "The app has been minimized. Click the tray icon to show.";
            m_notifyIcon.BalloonTipTitle = "Gameplay Time Tracker";
            m_notifyIcon.Text = "Gameplay Time Tracker";
            m_notifyIcon.Icon =
                new System.Drawing.Icon(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppIcon));
            m_notifyIcon.Click += new EventHandler(m_notifyIcon_Click);
            m_notifyIcon.Visible = true; // Make sure it's visible
        }

        bool IsValidImage(string imagePath)
        {
            try
            {
                // Create a BitmapImage object and load the image
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Load the entire image at once
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
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
    }
}