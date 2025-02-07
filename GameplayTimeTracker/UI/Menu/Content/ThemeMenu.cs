using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GameplayTimeTracker.Settings;
using GameplayTimeTracker.UI.Menu.Content;

namespace GameplayTimeTracker.Menu.Content;

public class ThemeMenu : MenuContent
{
    public ComboBox ThemeComboBox = new();

    private ScrollViewer colorEntryScrollViewer;
    private StackPanel colorEntryPanel;

    public ThemeMenu()
    {
        colorEntryScrollViewer = new ScrollViewer
        {
            Height = 400,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
            Padding = new Thickness(5),
        };
        CreateComboBox();
        CreateColorEntries();
        _stackPanel.Children.Add(colorEntryScrollViewer);
    }

    private void CreateColorEntries()
    {
        colorEntryPanel = new StackPanel();
        Console.WriteLine($"Theme: {Common.Settings.CurrentTheme.ThemeName}");
        foreach (var color in Common.Settings.CurrentTheme.Colors)
        {
            ColorEntry colorEntry =
                new ColorEntry(color.Key, color.Value, AppColors.CardColor1, AppColors.CardColor2);
            colorEntry.colorPicker.SelectedColorChanged += (s, e) =>
            {
                ColorPicker_SelectedColorChanged(s, e, colorEntry, Common.Settings.CurrentTheme);
                DataHandler.WriteSettingsToFile(Common.Settings);
            };
            BindingHelper.SetColorBinding(colorEntry.valueBlock, ForegroundProperty, "Font");
            BindingHelper.SetColorBinding(colorEntry.nameBlock, ForegroundProperty, "Font");
            BindingHelper.SetGradientColorBinding(colorEntry.bg, Shape.FillProperty,
                "Card 1", "Card 2", true);
            colorEntryPanel.Children.Add(colorEntry);
        }

        colorEntryScrollViewer.Content = colorEntryPanel;
        // Panel.Children.Add(colorEntryScrollViewer);
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

            // Update the theme's and source's color dictionary
            theme.UpdateColor(colorEntry.ColorName, colorEntry.ColorValue);
            foreach (var t in Common.Settings.ThemesList)
            {
                if (t.ThemeName == theme.ThemeName) t.UpdateColor(colorEntry.ColorName, colorEntry.ColorValue);
            }
        }
        else
        {
            // Handle no selection
            colorEntry.colorPicker.Background = new SolidColorBrush(Colors.Transparent);
            Console.WriteLine($"No color selected for {colorEntry.Name}.");
        }
    }

    private void CreateComboBox()
    {
        ThemeComboBox = new ComboBox
        {
            Width = 150,
            Height = 30,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10)
        };
        foreach (var theme in Common.Settings.ThemesList)
        {
            ThemeComboBox.Items.Add(theme.ThemeName);
        }

        ThemeComboBox.SelectionChanged += (s, e) =>
        {
            foreach (var theme in Common.Settings.ThemesList)
            {
                if (theme.ThemeName == ThemeComboBox.SelectedItem.ToString())
                {
                    ForceCurrentThemeDictUpdate(Common.Settings.CurrentTheme, theme);
                    CreateColorEntries();
                    DataHandler.WriteSettingsToFile(Common.Settings);
                    return;
                }
            }
        };
        _stackPanel.Children.Add(ThemeComboBox);

        if (ThemeComboBox.Items.Contains(Common.Settings.CurrentTheme.ThemeName))
        {
            ThemeComboBox.SelectedItem = Common.Settings.CurrentTheme.ThemeName;
        }
    }

    private void ForceCurrentThemeDictUpdate(AppTheme currentTheme, AppTheme newTheme)
    {
        var keys = new List<string>(currentTheme.Colors.Keys); // Create a copy of the keys

        foreach (var key in keys)
        {
            if (newTheme.Colors.ContainsKey(key))
            {
                currentTheme.UpdateColor(key, newTheme.Colors[key]);
            }
        }

        currentTheme.ThemeName = newTheme.ThemeName;
    }
}