using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using GameplayTimeTracker.Menu.Content;
using GameplayTimeTracker.UI.Menu.Content;

namespace GameplayTimeTracker.Menu;

public class SettingsMenu : CustomMenu
{
    private StackPanel HeaderPanel = new();
    private ScrollViewer ContentScrollViewer = new();
    private Type currentMenuType;
    public TextBlock PrefBlock;
    public TextBlock ThemeBlock;
    public TextBlock BackupBlock;
    public TextBlock RemoteBlock;

    public SettingsMenu(double width = 400, bool toScale = true)
        : base(width, toScale)
    {
        HeaderPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        var blockMargin = new Thickness(10);
        PrefBlock = UIHelper.CreateTextBlock("Preferences", isBold: false, fontSize: Common.TextFontSize + 2);
        PrefBlock.Padding = blockMargin;
        BindingHelper.SetColorBinding(PrefBlock, ForegroundProperty, "Font");
        PrefBlock.MouseDown += (_, _) => { SetMenu<PrefMenu>(PrefBlock); };

        ThemeBlock = UIHelper.CreateTextBlock("Themes", isBold: false, fontSize: Common.TextFontSize + 2);
        ThemeBlock.Padding = blockMargin;
        BindingHelper.SetColorBinding(ThemeBlock, ForegroundProperty, "Font");
        ThemeBlock.MouseDown += (_, _) => { SetMenu<ThemeMenu>(ThemeBlock); };

        BackupBlock = UIHelper.CreateTextBlock("Backup", isBold: false, fontSize: Common.TextFontSize + 2);
        BackupBlock.Padding = blockMargin;
        BindingHelper.SetColorBinding(BackupBlock, ForegroundProperty, "Font");
        BackupBlock.MouseDown += (_, _) => { SetMenu<BackupMenu>(BackupBlock); };

        RemoteBlock = UIHelper.CreateTextBlock("Remote", isBold: false, fontSize: Common.TextFontSize + 2);
        RemoteBlock.Padding = blockMargin;
        BindingHelper.SetColorBinding(RemoteBlock, ForegroundProperty, "Font");
        RemoteBlock.MouseDown += (_, _) => { SetMenu<RemoteMenu>(RemoteBlock); };

        HeaderPanel.Children.Add(PrefBlock);
        HeaderPanel.Children.Add(ThemeBlock);
        HeaderPanel.Children.Add(BackupBlock);
        HeaderPanel.Children.Add(RemoteBlock);
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

    public void SetMenu<T>(TextBlock selectedTextBlock) where T : new()
    {
        if (currentMenuType == typeof(T))
            return;

        var menuInstance = Activator.CreateInstance(typeof(T));
        double previousHeight = 0.0;

        if (menuInstance is not null && menuInstance is MenuContent newMenu)
        {
            if (MenuContentPanel.Children.Contains(ContentScrollViewer))
            {
                previousHeight = ContentScrollViewer.ActualHeight;
                MenuContentPanel.Children.Remove(ContentScrollViewer);
            }

            ContentScrollViewer = newMenu._scrollViewer;

            // Ensure the new menu is measured before animation
            MenuContentPanel.Children.Add(ContentScrollViewer);

            ContentScrollViewer.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            double newHeight = ContentScrollViewer.DesiredSize.Height;

            AnimateHeightTransition(ContentScrollViewer, previousHeight, newHeight);
            currentMenuType = typeof(T);
            HighlightCurrentTextBlock(selectedTextBlock);
        }
    }

    private void AnimateHeightTransition(ScrollViewer target, double fromHeight, double toHeight)
    {
        var heightAnimation = new DoubleAnimation
        {
            From = fromHeight,
            To = toHeight,
            Duration = TimeSpan.FromMilliseconds(200),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        target.BeginAnimation(FrameworkElement.HeightProperty, heightAnimation);
    }
}