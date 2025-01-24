using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using GameplayTimeTracker.Menu;
using GameplayTimeTracker.Settings;

namespace GameplayTimeTracker;

public class StatsGraph : CustomMenu
{
    public StatsGraph(Entry entry, AppSettings appSettings, double width = 300, bool performanceMode = true)
        : base(appSettings, width, performanceMode)
    {
        TextBlock titleTextBlock = UIHelper.CreateTextBlock(text: $"Last week's playtime for ",
            isBold: false, fontSize: Common.EditTitleFontSize);
        var gameNameRun = new Run { Text = entry.Name, FontWeight = FontWeights.Bold };
        titleTextBlock.Inlines.Add(gameNameRun);

        MenuContentPanel.Children.Add(titleTextBlock);
    }
}