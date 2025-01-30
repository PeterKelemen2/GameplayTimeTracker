using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using GameplayTimeTracker.Settings;
using GameplayTimeTracker.SGDB;
using Size = System.Drawing.Size;

namespace GameplayTimeTracker.Menu;

public class EditMenu : EntryConfigMenu
{
    public EditMenu(Entry entry,
        double width = 350, bool toScale = true)
        : base(entry, width, toScale)
    {
        ToScale = toScale;
        TitleTextBlock.FontWeight = FontWeights.Regular;
        TitleTextBlock.Text = "Editing ";
        TitleTextBlock.Inlines.Add(new Run { Text = entry.Name, FontWeight = FontWeights.Bold });
        ConfirmButton.Click += (_, _) => { Close(); };

        Grid refreshGrid = new Grid();
        var RefreshSGDBButton =
            new CustomButton(w: 120, h: 40, text: "SGDB Refresh", effect: AppEffects.DropShadowIcon);
        RefreshSGDBButton.Margin = new Thickness(0, 20, 130, 30);
        RefreshSGDBButton.Click += (_, __) => { entry.RefreshImagesFromSGDB(); };
        refreshGrid.Children.Add(RefreshSGDBButton);

        var RefreshLocalHeroButton =
            new CustomButton(w: 120, h: 40, text: "Local Refresh", effect: AppEffects.DropShadowIcon);
        RefreshLocalHeroButton.Margin = new Thickness(130, 20, 0, 30);
        RefreshLocalHeroButton.Click += (_, __) =>
        {
            Guid guid = Guid.NewGuid();
            string newImagePath = Path.Combine(AppFiles.SavedImagesPath, $"_{guid}.png");
            ImageHelper.ScatterImage(entry.IconPath, newImagePath);
            entry.HeroPath = newImagePath;
        };
        refreshGrid.Children.Add(RefreshLocalHeroButton);

        stackPanel.Children.Add(refreshGrid);

        var ShowStatsButton =
            new CustomButton(w: 120, h: 40, text: "Show Stats", effect: AppEffects.DropShadowIcon);
        ShowStatsButton.Margin = new Thickness(0, 0, 0, 20);
        ShowStatsButton.Click += (_, __) =>
        {
            StatsGraph statsGraph = new StatsGraph(entry);
            statsGraph.Open();
        };
        stackPanel.Children.Add(ShowStatsButton);
    }
}