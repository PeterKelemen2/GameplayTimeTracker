using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using GameplayTimeTracker.Settings;
using GameplayTimeTracker.SGDB;

namespace GameplayTimeTracker.Menu;

public class EditMenu : EntryConfigMenu
{
    public EditMenu(Entry entry,
        double width = 350, bool performanceMode = true)
        : base(entry, width, performanceMode)
    {
        TitleTextBlock.FontWeight = FontWeights.Regular;
        TitleTextBlock.Text = "Editing ";
        TitleTextBlock.Inlines.Add(new Run { Text = entry.Name, FontWeight = FontWeights.Bold });
        ConfirmButton.Click += (_, _) => { Close(); };

        var RefreshImagesButton =
            new CustomButton(w: 120, h: 40, text: "SGDB Refresh", effect: AppEffects.dropShadowIcon);
        RefreshImagesButton.Margin = new Thickness(0, 0, 0, 20);
        RefreshImagesButton.Click += (_, __) => { entry.RefreshImagesFromSGDB(); };
        stackPanel.Children.Add(RefreshImagesButton);

        var ShowStatsButton =
            new CustomButton(w: 120, h: 40, text: "Show Stats", effect: AppEffects.dropShadowIcon);
        ShowStatsButton.Margin = new Thickness(0, 0, 0, 20);
        ShowStatsButton.Click += (_, __) =>
        {
            StatsGraph statsGraph = new StatsGraph(entry);
            statsGraph.Open();
        };
        stackPanel.Children.Add(ShowStatsButton);
    }
}