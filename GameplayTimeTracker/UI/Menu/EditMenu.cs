using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using WrapPanel = Xceed.Wpf.Toolkit.Panels.WrapPanel;

namespace GameplayTimeTracker.Menu;

public class EditMenu : EntryConfigMenu
{
    public EditMenu(Entry entry,
        double width = 350, bool toScale = true)
        : base(entry, width, toScale)
    {
        MenuContentPanel.Orientation = Orientation.Horizontal;
        MenuContentPanel.Width = Double.NaN;

        MenuContentPanel.Children.Remove(stackPanel);
        stackPanel.Width = width;

        StackPanel leftSide = stackPanel;
        StackPanel rightSide = new StackPanel { Width = width, Background = Brushes.Black };
        MenuContentPanel.Children.Add(leftSide);
        MenuContentPanel.Children.Add(rightSide);

        ToScale = toScale;
        TitleTextBlock.FontWeight = FontWeights.Regular;
        TitleTextBlock.Text = "Editing ";
        TitleTextBlock.Inlines.Add(new Run { Text = entry.Name, FontWeight = FontWeights.Bold });
        TitleTextBlock.TextTrimming = TextTrimming.CharacterEllipsis;
        TitleTextBlock.Padding = new Thickness(10, 0, 10, 0);
        ConfirmButton.Click += (_, _) => { Close(); };

        CreateTitleBlock("Refresh Images");

        Panel buttonContainer = new WrapPanel
            { HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 15) };
        var RefreshSGDBButton =
            new CustomButton(w: 120, h: 40, text: "Full SGDB", effect: AppEffects.DropShadowIcon);
        RefreshSGDBButton.Margin = new Thickness(5);
        RefreshSGDBButton.Click += (_, __) => { entry.RefreshImagesFromSGDB(); };
        if (Common.Settings.SGDBApiKey.Length == 0)
        {
            RefreshSGDBButton.Active = false;
        }

        buttonContainer.Children.Add(RefreshSGDBButton);

        var RefreshLocalHeroButton =
            new CustomButton(w: 120, h: 40, text: "Hero Local", effect: AppEffects.DropShadowIcon);
        RefreshLocalHeroButton.Margin = new Thickness(5);
        RefreshLocalHeroButton.Click += async (_, __) => { RefreshLocalHero(entry); };
        buttonContainer.Children.Add(RefreshLocalHeroButton);

        var RefreshLocalIconFromExeButton =
            new CustomButton(w: 120, h: 40, text: "Icon Local", effect: AppEffects.DropShadowIcon);
        RefreshLocalIconFromExeButton.Margin = new Thickness(5);
        RefreshLocalIconFromExeButton.Click += async (_, __) => { RefreshLocalIcon(entry); };
        Binding activeBinding = new Binding("IsLaunchable")
        {
            Source = entry,
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(RefreshLocalIconFromExeButton, CustomButton.ActiveProperty, activeBinding);
        buttonContainer.Children.Add(RefreshLocalIconFromExeButton);

        var ShowStatsButton =
            new CustomButton(w: 120, h: 40, text: "Show Stats", effect: AppEffects.DropShadowIcon,
                type: BType.Positive);
        ShowStatsButton.Margin = new Thickness(5);
        ShowStatsButton.Click += (_, __) =>
        {
            StatsGraph statsGraph = new StatsGraph(entry);
            statsGraph.Open();
        };
        buttonContainer.Children.Add(ShowStatsButton);

        stackPanel.Children.Add(buttonContainer);
    }

    private async void RefreshLocalHero(Entry entry)
    {
        await Task.Run(() =>
        {
            Guid guid = Guid.NewGuid();
            string newImagePath = Path.Combine(AppFiles.SavedImagesPath, $"_{guid}_hero.png");
            ImageHelper.ScatterImage(entry.IconPath, newImagePath);

            Dispatcher.Invoke(() => entry.HeroPath = newImagePath);
        });
    }

    private async void RefreshLocalIcon(Entry entry)
    {
        await Task.Run(() =>
        {
            Guid guid = Guid.NewGuid();
            string newImagePath = Path.Combine(AppFiles.SavedImagesPath, $"_{guid}_icon.png");
            string cloned = string.Copy(newImagePath);
            // bool success = ImageHelper.SaveIconFromExe(entry.ExePath, newImagePath);
            ImageHelper.SaveIconFromExe(entry.ExePath, newImagePath);
            Dispatcher.Invoke(() => entry.IconPath = newImagePath);
        });
    }
}