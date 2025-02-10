using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using GameplayTimeTracker.Menu;

namespace GameplayTimeTracker;

public class StatsGraph : CustomMenu
{
    private double graphWidth = 400;
    private double graphHeight = 200;
    private double barsWidth = 30;
    private double[] marginArray;

    public StatsGraph(Entry entry, double width = 500, bool toScale = false)
        : base(width, toScale)
    {
        ToScale = false;
        TextBlock titleTextBlock = UIHelper.CreateTextBlock(
            text: $"Last {entry.PlaytimeHistory.Count()} day's playtime for ",
            isBold: false, fontSize: Common.EditTitleFontSize, hA: HorizontalAlignment.Center);
        titleTextBlock.Padding = new Thickness(20);
        titleTextBlock.TextWrapping = TextWrapping.Wrap;
        titleTextBlock.TextAlignment = TextAlignment.Center;
        var gameNameRun = new Run { Text = entry.Name, FontWeight = FontWeights.Bold };
        var spacing = new Run { Text = "\n.\n", Foreground = Brushes.Transparent, FontSize = 10 };
        var gameTotalTimeRun1 = new Run { Text = "Total Playtime: " };
        var gameTotalTimeRun2 = new Run
            { Text = Common.GetPrettyTimeFromArray(GetTotalTime(entry)), FontWeight = FontWeights.Bold };
        titleTextBlock.Inlines.Add(gameNameRun);
        titleTextBlock.Inlines.Add(spacing);
        titleTextBlock.Inlines.Add(gameTotalTimeRun1);
        titleTextBlock.Inlines.Add(gameTotalTimeRun2);
        MenuContentPanel.Children.Add(titleTextBlock);

        Grid grid = new Grid { Width = width, ClipToBounds = false };

        StackPanel statStackPanel = new StackPanel
        {
            Width = graphWidth, Height = graphHeight,
            Orientation = Orientation.Horizontal,
        };
        Border statsPanelBorder = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Top,
            Background = new SolidColorBrush(ColorHelper.AdjustBrightness(
                (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]), 1.2)),
            BorderBrush = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"])),
            BorderThickness = new Thickness(2, 0, 0, 2),
            Margin = new Thickness(0, 10, 0, 0),
            CornerRadius = new CornerRadius(0, 5, 0, 0),
            Effect = AppEffects.DropShadowRectangle,
            Child = statStackPanel,
        };
        grid.Children.Add(statsPanelBorder);

        int count = entry.PlaytimeHistory.Count;
        double totalSpace = graphWidth - (count * barsWidth);
        double maxTime = entry.PlaytimeHistory.Max(stat => Common.GetDoubleTimeFromArray(stat.Value)) * 1.1;
        double rectMargin = (totalSpace / count) * 0.5;
        double baseTimeMarg = (width - graphWidth) * 0.5 + 35;

        for (int i = 0; i < count; i++)
        {
            double barRatio = Common.GetDoubleTimeFromArray(entry.PlaytimeHistory.ElementAt(i).Value) / maxTime;
            Border bar = new Border
            {
                Width = barsWidth, Height = barRatio * graphHeight,
                CornerRadius = new CornerRadius(5, 5, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(rectMargin, 0, rectMargin, 0),
            };
            BindingHelper.SetGradientColorBinding(bar, Border.BackgroundProperty, "Progress Bar 1",
                "Progress Bar 2", true);
            statStackPanel.Children.Add(bar);

            int[] currTimeArr = entry.PlaytimeHistory.ElementAt(i).Value;
            TextBlock time = new TextBlock
            {
                Text = $"{currTimeArr[0]}h {currTimeArr[1]}m",
                FontSize = Common.TitleFontSize,
                HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top,
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"])),
                Margin = new Thickness(baseTimeMarg + i * (rectMargin * 2 + barsWidth),
                    graphHeight + Common.TitleFontSize, 0, 80),
                Effect = titleTextBlock.Effect
            };
            RotateTransform rotateTransform = new RotateTransform(60);
            time.RenderTransform = rotateTransform;
            grid.Children.Add(time);
        }

        MenuContentPanel.Children.Add(grid);
    }

    private int[] GetTotalTime(Entry entry)
    {
        int[] totalTime = entry.PlaytimeHistory
            .Aggregate(new int[3], (sum, stat) =>
            {
                sum[0] += stat.Value[0];
                sum[1] += stat.Value[1];
                sum[2] += stat.Value[2];
                return sum;
            });

        totalTime = Common.NormalizeTime(totalTime);

        return totalTime;
    }
}