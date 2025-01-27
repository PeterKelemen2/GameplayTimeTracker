using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GameplayTimeTracker.Menu.Content;
using GameplayTimeTracker.Settings;
using GameplayTimeTracker.UI.Menu.Content;

namespace GameplayTimeTracker.Menu;

public class SettingsMenu : CustomMenu
{
    StackPanel SettingsContentPanel = new();
    StackPanel HeaderPanel = new();
    // private AppSettings _settings;
    // public MainWindow _mainWindow;

    public SettingsMenu(double width = 400, bool performanceMode = true) 
        : base( width, performanceMode)
    {
        // _settings = appSettings;
        SettingsContentPanel = new StackPanel();
        HeaderPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        var blockMargin = new Thickness(10);
        var PrefBlock = UIHelper.CreateTextBlock("Preferences", margin: blockMargin, isBold: false,
            fontSize: Common.TextFontSize + 2);
        BindingHelper.SetColorBinding(PrefBlock, ForegroundProperty, "Font");
        PrefBlock.MouseDown += (_, _) => { SetPrefMenu(PrefBlock); };

        var Themes = UIHelper.CreateTextBlock("Themes", margin: blockMargin, isBold: false,
            fontSize: Common.TextFontSize + 2);
        BindingHelper.SetColorBinding(Themes, ForegroundProperty, "Font");
        Themes.MouseDown += (_, _) => { SetThemeMenu(Themes); };

        var Backup = UIHelper.CreateTextBlock("Backup", margin: blockMargin, isBold: false,
            fontSize: Common.TextFontSize + 2);
        BindingHelper.SetColorBinding(Backup, ForegroundProperty, "Font");
        Backup.MouseDown += (_, _) => { SetBackupMenu(Backup); };

        HeaderPanel.Children.Add(PrefBlock);
        HeaderPanel.Children.Add(Themes);
        HeaderPanel.Children.Add(Backup);
        Border headerBorder = new Border
        {
            BorderThickness = new Thickness(0, 0, 0, 1),
            BorderBrush = Brushes.Gray,
            Child = HeaderPanel,
        };
        MenuContentPanel.Children.Add(headerBorder);
        MenuContentPanel.Children.Add(SettingsContentPanel);

        SetPrefMenu(PrefBlock);

        // PrefEntry pref1 = new PrefEntry("Pref 1", false);
        // SettingsContentPanel.Children.Add(pref1);
    }

    private void HighlightCurrentTextBlock(TextBlock selectedTextBlock)
    {
        Console.WriteLine($"Clicked on TextBlock: {selectedTextBlock.Text}");
        foreach (UIElement element in HeaderPanel.Children)
        {
            if (element is TextBlock tb)
            {
                tb.FontWeight = tb == selectedTextBlock ? FontWeights.Bold : FontWeights.Regular;
            }
        }
    }

    private void SetPrefMenu(TextBlock selectedTextBlock)
    {
        var prefMenu = new PrefMenu();
        if (MenuContentPanel.Children.Contains(SettingsContentPanel))
        {
            MenuContentPanel.Children.Remove(SettingsContentPanel);
        }

        SettingsContentPanel = prefMenu.Panel;
        MenuContentPanel.Children.Add(SettingsContentPanel);

        HighlightCurrentTextBlock(selectedTextBlock);
    }

    private void SetThemeMenu(TextBlock selectedTextBlock)
    {
        var themeMenu = new ThemeMenu();
        if (MenuContentPanel.Children.Contains(SettingsContentPanel))
        {
            MenuContentPanel.Children.Remove(SettingsContentPanel);
        }

        SettingsContentPanel = themeMenu.Panel;
        MenuContentPanel.Children.Add(SettingsContentPanel);

        HighlightCurrentTextBlock(selectedTextBlock);
    }

    private void SetBackupMenu(TextBlock selectedTextBlock)
    {
        var backupMenu = new BackupMenu(Common.Settings);
        if (MenuContentPanel.Children.Contains(SettingsContentPanel))
        {
            MenuContentPanel.Children.Remove(SettingsContentPanel);
        }

        SettingsContentPanel = backupMenu.Panel;
        MenuContentPanel.Children.Add(SettingsContentPanel);

        HighlightCurrentTextBlock(selectedTextBlock);
    }
}