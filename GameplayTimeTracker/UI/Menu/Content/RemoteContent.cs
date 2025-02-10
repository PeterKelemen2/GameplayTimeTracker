using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace GameplayTimeTracker.UI.Menu.Content;

public class RemoteContent : UserControl
{
    StackPanel savesStackPanel;
    StackPanel container;
    private Entry _entry;
    private List<string> filesList;
    private double scrollWidth = 300;
    private double scrollHeight = 200;
    private string currentSelectedPath = "";

    public RemoteContent()
    {
    }

    public RemoteContent(Entry entry)
    {
        _entry = entry;
        filesList = new List<string>();

        container = new StackPanel();

        ScrollViewer savesScrollViewer = new ScrollViewer
            { Height = scrollHeight, Width = scrollWidth, VerticalScrollBarVisibility = ScrollBarVisibility.Hidden };
        savesStackPanel = new StackPanel();

        Border savesBorder = new Border
        {
            Height = scrollHeight, Width = scrollWidth,
            CornerRadius = new CornerRadius(Common.BorderRadius),
            Background = new SolidColorBrush(ColorHelper.AdjustBrightness(
                (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]), 1.2)),
            Margin = new Thickness(10),
            Child = savesScrollViewer,
            Effect = AppEffects.DropShadowIcon
        };

        savesScrollViewer.Content = savesStackPanel;

        container.Children.Add(savesBorder);
        Content = container;
        CreateButtons();
        // AddLoadingIndicator();
        // LoadSubfoldersAsync();
        LoadData();
    }

    private void LoadData()
    {
        AddLoadingIndicator();
        LoadSubfoldersAsync();
    }

    private void CreateButtons()
    {
        StackPanel buttonsStackPanel = new StackPanel
            { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
        var uploadButton = new CustomButton(w: 120, h: 40, text: "Upload", effect: AppEffects.DropShadowIcon,
            hA: HorizontalAlignment.Center);
        uploadButton.Margin = new Thickness(5);
        uploadButton.Click += UploadButton_Click;
        var downloadButton = new CustomButton(w: 120, h: 40, text: "Download", effect: AppEffects.DropShadowIcon,
            hA: HorizontalAlignment.Center);
        downloadButton.Margin = new Thickness(5);
        downloadButton.Click += DownloadButton_Click;

        buttonsStackPanel.Children.Add(uploadButton);
        buttonsStackPanel.Children.Add(downloadButton);
        container.Children.Add(buttonsStackPanel);
    }

    private void DownloadButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(currentSelectedPath)) return;
        Console.WriteLine($"Downloading selected save from {currentSelectedPath}");

        string remoteGameFolder = Path.Combine(Common.Settings.RemoteMachine.RemoteFolder, _entry.Name)
            .Replace("\\", "/");
        RemoteController.DownloadFolder(RemoteController.GetPathWithLatestName(remoteGameFolder), _entry.LocalSavePath);
    }

    private void UploadButton_Click(object sender, RoutedEventArgs e)
    {
        if (!Directory.Exists(_entry.LocalSavePath)) return;
        Console.WriteLine($"Uploading local save to {_entry.LocalSavePath}");

        string uploadPath =
            $"{Common.Settings.RemoteMachine.RemoteFolder.TrimEnd('/')}/{_entry.Name}/{DateTime.Now:yyyy-MM-dd-HH-mm-ss}";
        RemoteController.UploadFolder(_entry.LocalSavePath, uploadPath);

        LoadData();
    }

    private async void AddLoadingIndicator()
    {
        await FadeOutElement(savesStackPanel);
        savesStackPanel.Children.Clear();
        Image loading = new Image
        {
            Width = 64, Height = 64,
            Margin = new Thickness(0, scrollHeight / 2 - 32, 0, 0),
            Source = new BitmapImage(new Uri(AppFiles.LoadingImage, UriKind.RelativeOrAbsolute)),
            RenderTransformOrigin = new Point(0.5, 0.5)
        };

        RotateTransform rotateTransform = new RotateTransform();
        loading.RenderTransform = rotateTransform;
        rotateTransform.BeginAnimation(RotateTransform.AngleProperty, AppAnimations.RotationAnimation);

        savesStackPanel.Children.Add(loading);
        await FadeInElement(savesStackPanel);
    }

    private async void LoadSubfoldersAsync()
    {
        try
        {
            var remoteContent = await RemoteController
                .ListGameSubfoldersAsync(Common.Settings.RemoteMachine.RemoteFolder, _entry.Name).ConfigureAwait(false);

            Dispatcher.Invoke(() => { ShowSaves(remoteContent); });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async void ShowSaves(List<string> files)
    {
        // Fade out before clearing content
        await FadeOutElement(savesStackPanel);

        savesStackPanel.Children.Clear();

        if (files.Count == 0)
        {
            Console.WriteLine("No saves were found!");
            TextBlock textBlock = new TextBlock
            {
                Text = "No saves were found!",
                FontSize = 21,
                FontWeight = FontWeights.Bold,
                Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"])),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, scrollHeight / 2 - 21, 0, 0),
                Padding = new Thickness(5),
            };

            savesStackPanel.Children.Add(textBlock);
        }
        else
        {
            // Add new content
            foreach (var file in files)
            {
                string saveName = file.Split("/").LastOrDefault();
                Border textBorder = new Border { CornerRadius = new CornerRadius(8.5), Margin = new Thickness(5) };
                TextBlock textBlock = new TextBlock
                {
                    Text = saveName,
                    FontSize = Common.TitleFontSize,
                    Foreground =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"])),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Padding = new Thickness(5),
                };
                textBorder.Child = textBlock;

                textBorder.MouseDown += (s, e) =>
                {
                    currentSelectedPath = Path.Combine(AppFiles.BackupDataFolder, file);
                    HighlightSelected(textBorder);
                    Console.WriteLine(currentSelectedPath);
                };

                savesStackPanel.Children.Add(textBorder);
                Console.WriteLine(file);
            }
        }

        // Fade in after everything is added
        await FadeInElement(savesStackPanel);
    }


    private Task FadeOutElement(UIElement element)
    {
        var tcs = new TaskCompletionSource<bool>();
        DoubleAnimation fadeOut = new DoubleAnimation(0, TimeSpan.FromSeconds(AppAnimations.fastFadeAnimDuration));
        fadeOut.Completed += (s, e) => tcs.SetResult(true);
        element.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        return tcs.Task;
    }

    private Task FadeInElement(UIElement element)
    {
        var tcs = new TaskCompletionSource<bool>();
        DoubleAnimation fadeIn = new DoubleAnimation(1, TimeSpan.FromSeconds(AppAnimations.fastFadeAnimDuration));
        fadeIn.Completed += (s, e) => tcs.SetResult(true);
        element.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        return tcs.Task;
    }

    private void HighlightSelected(Border border)
    {
        foreach (UIElement element in savesStackPanel.Children)
        {
            if (element is Border b)
            {
                b.Background = b == border
                    ? new SolidColorBrush(ColorHelper.AdjustBrightness(
                        (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]),
                        0.8))
                    : new SolidColorBrush(Colors.Transparent);
            }
        }
    }
}