using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace GameplayTimeTracker.UI.Menu.Content;

public class RemoteContent : UserControl
{
    StackPanel savesStackPanel;
    private Entry _entry;
    private List<string> filesList;
    private double scrollWidth = 300;
    private double scrollHeight = 200;
    private string currentSelectedPath = "";

    public RemoteContent(Entry entry)
    {
        _entry = entry;
        filesList = new List<string>();

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

        Content = savesBorder;

        AddLoadingIndicator();
        LoadSubfoldersAsync();
    }

    private void AddLoadingIndicator()
    {
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

    private void ShowSaves(List<string> files)
    {
        // Clear previous content
        savesStackPanel.Children.Clear();
        Console.WriteLine(files.Count);

        if (files.Count == 0)
        {
            Console.WriteLine("No saves were found!");
            TextBlock textBlock = new TextBlock
            {
                Text = "No saves were found!",
                FontSize = 21,
                Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"])),
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new Thickness(5),
            };
            savesStackPanel.Children.Add(textBlock);
            return;
        }

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