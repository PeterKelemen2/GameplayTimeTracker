using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameplayTimeTracker.UI.Menu.Content;

public class RemoteContent : UserControl
{
    StackPanel savesStackPanel;
    private Entry _entry;
    private List<string> filesList;
    private double scrollWidth = 300;
    private double scrollHeight = 200;

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

        LoadSubfoldersAsync();
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

        // Add new content
        foreach (var file in files)
        {
            string saveName = file.Split("/").LastOrDefault();
            TextBlock textBlock = new TextBlock
            {
                Text = saveName,
                FontSize = Common.TitleFontSize,
                Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"])),
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new Thickness(5),
                Margin = new Thickness(5),
            };

            savesStackPanel.Children.Add(textBlock);
            Console.WriteLine(file);
        }
    }
}