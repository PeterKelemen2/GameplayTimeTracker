using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Pango;
using Color = System.Windows.Media.Color;

namespace GameplayTimeTracker;

public class ThemeMenu : UserControl
{
    public Grid container { get; set; }
    public List<Theme> Themes { get; set; }
    public StackPanel Panel { get; set; }
    public ComboBox comboBox { get; set; }
    public String SelectedThemeName { get; set; }
    public SettingsMenu SettingsMenu { get; set; }

    public ThemeMenu(SettingsMenu settingsMenu, StackPanel stackPanel, List<Theme> themes, String selectedThemeName)
    {
        SettingsMenu = settingsMenu;
        Panel = stackPanel;
        Themes = themes;
        SelectedThemeName = selectedThemeName;
    }

    private void InitSelected()
    {
        if (comboBox.Items.Count > 0)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (comboBox.Items[i].ToString().Equals(SelectedThemeName))
                {
                    comboBox.SelectedIndex = i;
                    break;
                }
            }
        }
    }

    public void CreateDropdown()
    {
        Panel.Children.Clear(); // Clear all children first
        comboBox = new ComboBox
        {
            Width = 150,
            Height = 30,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10)
        };

        ThemeSecurity();

        comboBox.SelectionChanged += (sender, e) =>
        {
            SelectedThemeName = comboBox.SelectedItem.ToString();
            JsonHandler jsonHandler = new JsonHandler();
            jsonHandler.WriteSelectedThemeToFile(comboBox.SelectedItem.ToString());
            AddColorEntries();
            SettingsMenu.SetBlurImage();
        };

        foreach (var theme in Themes)
        {
            comboBox.Items.Add(theme.ThemeName);
        }

        InitSelected();
        Panel.Children.Add(comboBox);

        AddColorEntries(); // Populate initial color entries based on the default selection
    }

    private void AddColorEntries()
    {
        // Remove all non-ComboBox children in a single pass
        Panel.Children
            .OfType<UIElement>()
            .Where(child => child is not ComboBox)
            .ToList()
            .ForEach(child => Panel.Children.Remove(child));

        // Find the selected theme quickly using LINQ
        var selectedTheme = Themes.FirstOrDefault(theme => theme.ThemeName.Equals(SelectedThemeName));

        if (selectedTheme == null)
        {
            Console.WriteLine($"Theme {SelectedThemeName} not found.");
            return; // Exit if no matching theme is found
        }

        Console.WriteLine($"You selected: {selectedTheme.ThemeName}");

        // Perform actions related to the selected theme
        SettingsMenu.MainUpdateMethod();

        Color tileColor1 = GetColorFromTheme(selectedTheme.Colors, "tileColor1");
        Color tileColor2 = GetColorFromTheme(selectedTheme.Colors, "tileColor2");
        Color fontColor = GetColorFromTheme(selectedTheme.Colors, "fontColor");
        Color bgColor = GetColorFromTheme(selectedTheme.Colors, "bgColor");

        SettingsMenu.SetColors(fontColor, bgColor);

        // Add color entries dynamically
        foreach (var (key, value) in selectedTheme.Colors)
        {
            var newColorEntry = new ColorEntry(key, value, tileColor1, tileColor2);

            newColorEntry.colorPicker.SelectedColorChanged += (sender, e) =>
                ColorPicker_SelectedColorChanged(sender, e, newColorEntry);

            Panel.Children.Add(newColorEntry);
            Console.WriteLine($"Name: {key} - Value: {value}");
        }
    }

// Helper method to get a color from a theme dictionary
    private Color GetColorFromTheme(Dictionary<string, string> colors, string key)
    {
        return colors.TryGetValue(key, out var colorString)
            ? (Color)ColorConverter.ConvertFromString(colorString)
            : Colors.Transparent; // Fallback to transparent if the key is missing
    }

    private void ThemeSecurity()
    {
        if (SelectedThemeName != null)
        {
            bool found = false;
            foreach (var theme in Themes)
            {
                if (SelectedThemeName.Equals(theme.ThemeName))
                {
                    found = true;
                }
            }

            if (!found)
            {
                Theme defaultTheme = new Theme();
                defaultTheme.ThemeName = SelectedThemeName;
                defaultTheme.Colors = Utils.GetDefaultColors();
                Themes.Add(defaultTheme);
            }
        }
    }

    private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e,
        ColorEntry colorEntry)
    {
        var selectedColor = e.NewValue;

        if (selectedColor.HasValue)
        {
            // Update the specific ColorEntry UI
            Color color = selectedColor.Value;
            colorEntry.colorPicker.Background = new SolidColorBrush(color);
            colorEntry.valueBlock.Text = color.ToString();
            colorEntry.ColorValue = color.ToString();

            SaveChangedColor(comboBox.SelectedItem.ToString(), colorEntry.ColorName, colorEntry.ColorValue);
            Utils.SetColors(GetColorDictionary(comboBox.SelectedItem.ToString()));

            Console.WriteLine(
                $"Updated ColorEntry {colorEntry.ColorName}: {colorEntry.ColorValue} | Theme: {comboBox.SelectedItem}");
        }
        else
        {
            // Handle no selection
            colorEntry.colorPicker.Background = new SolidColorBrush(Colors.Transparent);
            Console.WriteLine($"No color selected for {colorEntry.Name}.");
        }
    }

    private void SaveChangedColor(String tName, String cKey, String cValue)
    {
        foreach (var theme in Themes)
        {
            if (theme.ThemeName.Equals(tName))
            {
                theme.Colors[cKey] = cValue;
                JsonHandler jsonHandler = new JsonHandler();
                jsonHandler.WriteThemesToFile(Themes);
            }
        }
    }

    private Dictionary<string, string> GetColorDictionary(String tName)
    {
        Dictionary<string, string> colors = new Dictionary<string, string>();
        foreach (var theme in Themes)
        {
            if (theme.ThemeName.Equals(tName))
            {
                foreach (var color in theme.Colors)
                {
                    colors.Add(color.Key, color.Value);
                }
            }
        }

        return colors;
    }

    public void CreateMenu(object sender, MouseButtonEventArgs e)
    {
        CreateDropdown();
    }
}