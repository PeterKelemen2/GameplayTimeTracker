using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using GameplayTimeTracker.Menu;
using GameplayTimeTracker.Settings;

namespace GameplayTimeTracker;

public class StatsGraph : CustomMenu
{
    private double graphWidth = 400;
    private double graphHeight = 200;
    private double barsWidth = 30;
    private double[] marginArray;

    public StatsGraph(Entry entry, AppSettings appSettings, double width = 500, bool performanceMode = true)
        : base(appSettings, width, performanceMode)
    {
        TextBlock titleTextBlock = UIHelper.CreateTextBlock(
            text: $"Last {entry.PlaytimeHistory.Count()} days playtime for ",
            isBold: false, fontSize: Common.EditTitleFontSize, hA: HorizontalAlignment.Center);
        titleTextBlock.Padding = new Thickness(20);
        titleTextBlock.TextWrapping = TextWrapping.Wrap;
        titleTextBlock.TextAlignment = TextAlignment.Center;
        // titleTextBlock.Margin = new Thickness(10, 20, 10, 10);
        var gameNameRun = new Run { Text = entry.Name, FontWeight = FontWeights.Bold };
        titleTextBlock.Inlines.Add(gameNameRun);
        var spacing = new Run { Text = "\n.\n", Foreground = Brushes.Transparent, FontSize = 10 };
        titleTextBlock.Inlines.Add(spacing);
        var gameTotalTimeRun1 = new Run { Text = "Total Playtime: " };
        var gameTotalTimeRun2 = new Run
            { Text = GetPrettyTime(GetTotalTime(entry)), FontWeight = FontWeights.Bold };
        titleTextBlock.Inlines.Add(gameTotalTimeRun1);
        titleTextBlock.Inlines.Add(gameTotalTimeRun2);
        MenuContentPanel.Children.Add(titleTextBlock);

        Grid grid = new Grid { Width = width, ClipToBounds = false };

        StackPanel statStackPanel = new StackPanel
        {
            Width = graphWidth,
            Height = graphHeight,
            Background = new SolidColorBrush(ColorHelper.AdjustBrightness(
                (Color)ColorConverter.ConvertFromString(appSettings.CurrentTheme.Colors["Background"]), 1.2)),
            Orientation = Orientation.Horizontal,
        };
        Border statsPanelBorder = new Border
        {
            Width = graphWidth,
            Height = graphHeight,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top,
            BorderBrush = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(appSettings.CurrentTheme.Colors["Font"])),
            BorderThickness = new Thickness(2, 0, 0, 2),
            Margin = new Thickness(0, 10, 0, 0),
            Effect = AppEffects.DropShadowRectangle,
            Child = statStackPanel,
        };
        grid.Children.Add(statsPanelBorder);

        int count = entry.PlaytimeHistory.Count;
        double totalSpace = graphWidth - (count * barsWidth);
        double maxTime = 0;
        foreach (var stat in entry.PlaytimeHistory)
        {
            if (stat.Value[0] > maxTime)
            {
                maxTime = stat.Value[0];
            }
        }

        maxTime += 2;

        double rectMargin = (totalSpace / count) * 0.5;
        double baseTimeMarg = (width - graphWidth) * 0.5 + 35;
        for (int i = 0; i < count; i++)
        {
            // Current Bar time value as double
            double tDouble = ConvertTimeArrayToDouble(entry.PlaytimeHistory.ElementAt(i).Value);
            double barRatio = tDouble / maxTime;
            Rectangle bar = new Rectangle
            {
                Width = barsWidth,
                Height = barRatio * graphHeight,
                RadiusX = 5,
                RadiusY = 5,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(rectMargin, 0, rectMargin, 0),
            };
            BindingHelper.SetGradientColorBinding(bar, Shape.FillProperty, appSettings, "Progress Bar 1",
                "Progress Bar 2", true);
            statStackPanel.Children.Add(bar);

            int[] currTimeArr = entry.PlaytimeHistory.ElementAt(i).Value;
            TextBlock time = new TextBlock
            {
                Text = $"{currTimeArr[0]}h {currTimeArr[1]}m",
                FontSize = Common.TitleFontSize,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(appSettings.CurrentTheme.Colors["Font"])),
                Margin =
                    new Thickness(baseTimeMarg + i * (rectMargin * 2 + barsWidth),
                        graphHeight + Common.TitleFontSize, 0, 80),
                Effect = titleTextBlock.Effect
            };
            RotateTransform rotateTransform = new RotateTransform(60);
            time.RenderTransform = rotateTransform;
            grid.Children.Add(time);
        }

        MenuContentPanel.Children.Add(grid);
    }

    private double ConvertTimeArrayToDouble(int[] arr)
    {
        return Math.Round(arr[0] + arr[1] / 60.0 + arr[2] / 3600.0, 2);
    }

    private int[] GetTotalTime(Entry entry)
    {
        int[] totalTime = new[] { 0, 0, 0 };
        foreach (var stat in entry.PlaytimeHistory)
        {
            totalTime[0] += stat.Value[0];
            totalTime[1] += stat.Value[1];
            totalTime[2] += stat.Value[2];
        }

        if (totalTime[2] > 60)
        {
            totalTime[1] += totalTime[2] / 60;
            totalTime[2] %= 60;
        }

        if (totalTime[1] > 60)
        {
            totalTime[0] += totalTime[1] / 60;
            totalTime[1] %= 60;
        }

        return totalTime;
    }

    private string GetPrettyTime(int[] arr)
    {
        return $"{arr[0]}h {arr[1]}m {arr[2]}s";
    }
}