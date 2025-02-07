using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GameplayTimeTracker.Menu.Content;
using GameplayTimeTracker.UI.Menu.Content;

namespace GameplayTimeTracker.Menu;

public class SettingsMenu : CustomMenu
{
    StackPanel HeaderPanel = new();
    ScrollViewer ContentScrollViewer = new();

    public SettingsMenu(double width = 400, bool toScale = true)
        : base(width, toScale)
    {
        HeaderPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        var blockMargin = new Thickness(10);
        var PrefBlock = UIHelper.CreateTextBlock("Preferences", margin: blockMargin, isBold: false,
            fontSize: Common.TextFontSize + 2);
        BindingHelper.SetColorBinding(PrefBlock, ForegroundProperty, "Font");
        PrefBlock.MouseDown += (_, _) => { SetMenu<PrefMenu>(PrefBlock); };

        var Themes = UIHelper.CreateTextBlock("Themes", margin: blockMargin, isBold: false,
            fontSize: Common.TextFontSize + 2);
        BindingHelper.SetColorBinding(Themes, ForegroundProperty, "Font");
        Themes.MouseDown += (_, _) => { SetMenu<ThemeMenu>(Themes); };

        var Backup = UIHelper.CreateTextBlock("Backup", margin: blockMargin, isBold: false,
            fontSize: Common.TextFontSize + 2);
        BindingHelper.SetColorBinding(Backup, ForegroundProperty, "Font");
        Backup.MouseDown += (_, _) => { SetMenu<BackupMenu>(Backup); };

        var Remote = UIHelper.CreateTextBlock("Remote", margin: blockMargin, isBold: false,
            fontSize: Common.TextFontSize + 2);
        BindingHelper.SetColorBinding(Remote, ForegroundProperty, "Font");
        Remote.MouseDown += (_, _) => { SetMenu<RemoteMenu>(Remote); };

        HeaderPanel.Children.Add(PrefBlock);
        HeaderPanel.Children.Add(Themes);
        HeaderPanel.Children.Add(Backup);
        HeaderPanel.Children.Add(Remote);
        Border headerBorder = new Border
        {
            BorderThickness = new Thickness(0, 0, 0, 1),
            BorderBrush = Brushes.Gray,
            Child = HeaderPanel,
        };
        MenuContentPanel.Children.Add(headerBorder);
        MenuContentPanel.Children.Add(ContentScrollViewer);

        SetMenu<PrefMenu>(PrefBlock);
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

    private void SetMenu<T>(TextBlock selectedTextBlock) where T : new()
    {
        var menuInstance = Activator.CreateInstance(typeof(T));

        if (menuInstance is not null && menuInstance is MenuContent menu)
        {
            if (MenuContentPanel.Children.Contains(ContentScrollViewer))
            {
                MenuContentPanel.Children.Remove(ContentScrollViewer);
            }

            ContentScrollViewer = menu._scrollViewer;
            MenuContentPanel.Children.Add(ContentScrollViewer);

            HighlightCurrentTextBlock(selectedTextBlock);
        }
    }
}