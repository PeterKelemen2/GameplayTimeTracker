using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GameplayTimeTracker.Settings;

namespace GameplayTimeTracker.Menu.Content;

public class ThemeMenu : UserControl
{
    public StackPanel Panel = new StackPanel();

    public ThemeMenu(AppSettings settings)
    {
        Panel = new StackPanel();

        StackPanel colorEntryPanel = new StackPanel();
        ScrollViewer colorEntryScrollViewer = new ScrollViewer
        {
            Height = 400,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
            Padding = new Thickness(5),
        };

        // foreach (var theme in settings.ThemesList)
        // {
        //     Console.WriteLine($"Theme: {theme.ThemeName}");
        //     foreach (var color in theme.Colors)
        //     {
        //         ColorEntry colorEntry =
        //             new ColorEntry(color.Key, color.Value, AppColors.CardColor1, AppColors.CardColor2);
        //         colorEntry.colorPicker.SelectedColorChanged += (s, e) =>
        //         {
        //             ColorPicker_SelectedColorChanged(s, e, colorEntry, theme);
        //         };
        //         colorEntryPanel.Children.Add(colorEntry);
        //     }
        // }

        Console.WriteLine($"Theme: {settings.CurrentTheme.ThemeName}");
        foreach (var color in settings.CurrentTheme.Colors)
        {
            ColorEntry colorEntry =
                new ColorEntry(color.Key, color.Value, AppColors.CardColor1, AppColors.CardColor2);
            colorEntry.colorPicker.SelectedColorChanged += (s, e) =>
            {
                ColorPicker_SelectedColorChanged(s, e, colorEntry, settings.CurrentTheme);
                // DataHandler.WriteSettingsToFile(settings);
            };
            colorEntryPanel.Children.Add(colorEntry);
        }

        colorEntryScrollViewer.Content = colorEntryPanel;
        Panel.Children.Add(colorEntryScrollViewer);
    }

    private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e,
        ColorEntry colorEntry, AppTheme theme)
    {
        var selectedColor = e.NewValue;

        if (selectedColor.HasValue)
        {
            // Update the specific ColorEntry
            Color color = selectedColor.Value;
            colorEntry.colorPicker.Background = new SolidColorBrush(color);
            colorEntry.valueBlock.Text = color.ToString();
            colorEntry.ColorValue = color.ToString();

            // Update the theme's color dictionary
            theme.UpdateColor(colorEntry.ColorName, colorEntry.ColorValue);
        }
        else
        {
            // Handle no selection
            colorEntry.colorPicker.Background = new SolidColorBrush(Colors.Transparent);
            Console.WriteLine($"No color selected for {colorEntry.Name}.");
        }
    }
}