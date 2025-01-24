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
        TextBlock titleTextBlock = UIHelper.CreateTextBlock(text: $"Last week's playtime for ",
            isBold: false, fontSize: Common.EditTitleFontSize, hA: HorizontalAlignment.Center);
        var gameNameRun = new Run { Text = entry.Name, FontWeight = FontWeights.Bold };
        titleTextBlock.Inlines.Add(gameNameRun);
        MenuContentPanel.Children.Add(titleTextBlock);

        StackPanel statsStackPanel = new StackPanel
        {
            Width = graphWidth,
            Height = graphHeight,
            Background = new SolidColorBrush(ColorHelper.AdjustBrightness(
                (Color)ColorConverter.ConvertFromString(appSettings.CurrentTheme.Colors["Background"]), 1.2)),
            Orientation = Orientation.Horizontal,
        };
        Border statsGridBorder = new Border
        {
            Width = graphWidth,
            Height = graphHeight,
            BorderBrush = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(appSettings.CurrentTheme.Colors["Font"])),
            BorderThickness = new Thickness(2, 0, 0, 2),
            Margin = new Thickness(20),
            Child = statsStackPanel,
        };
        MenuContentPanel.Children.Add(statsGridBorder);

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

        maxTime++;

        double rectMargin = (totalSpace / count) * 0.5;
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
                // Fill = new SolidColorBrush(Colors.Black),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(rectMargin, 0, rectMargin, 0),
            };
            BindingHelper.SetGradientColorBinding(bar, Shape.FillProperty, appSettings, "Progress Bar 1",
                "Progress Bar 2", true);
            statsStackPanel.Children.Add(bar);
        }
    }

    private double ConvertTimeArrayToDouble(int[] arr)
    {
        return Math.Round(arr[0] + arr[1] / 60.0 + arr[2] / 3600.0, 2);
    }
}