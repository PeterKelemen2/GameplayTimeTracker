﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GameplayTimeTracker;

public class ThemeMenu : UserControl
{
    public List<Theme> Themes { get; set; }
    public StackPanel Panel { get; set; }
    public ComboBox comboBox { get; set; }
    public String SelectedThemeName { get; set; }
    public SettingsMenu SettingsMenu { get; set; }

    public Button SwitchTileColors { get; set; }
    public Button SwitchEditColors { get; set; }

    public ThemeMenu(SettingsMenu settingsMenu, StackPanel stackPanel, List<Theme> themes, String selectedThemeName)
    {
        SettingsMenu = settingsMenu;
        Panel = stackPanel;
        Themes = themes;
        SelectedThemeName = selectedThemeName;

        SwitchTileColors = new Button
        {
            Content = "Switch Tile Colors",
            Style = (Style)Application.Current.FindResource("RoundedButton"),
            Height = 30,
            Width = 150,
        };
        SwitchEditColors = new Button
        {
            Content = "Switch Edit Colors",
            Style = (Style)Application.Current.FindResource("RoundedButton"),
            Height = 30,
            Width = 150,
            Margin = new Thickness(0, 5, 0, 5),
        };
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
        Panel.Children.Add(SwitchTileColors);
        Panel.Children.Add(SwitchEditColors);

        AddColorEntries(); // Populate initial color entries based on the default selection
    }

    private void AddColorEntries(bool switchTileColors = true, bool switchEditColors = false)
    {
        // Remove all children except the first one (the ComboBox)
        for (int i = Panel.Children.Count - 1; i >= 0; i--)
        {
            var child = Panel.Children[i];
            if (child is ColorEntry)
            {
                Console.WriteLine(child); // Log the child that is being removed
                Panel.Children.RemoveAt(i);
            }
        }

        // Add color entries for the selected theme
        foreach (var theme in Themes)
        {
            if (theme.ThemeName.Equals(SelectedThemeName))
            {
                Console.WriteLine($"You selected: {theme.ThemeName}");
                SettingsMenu.SetBlurImage();

                Color c1 = switchTileColors
                    ? (Color)ColorConverter.ConvertFromString(theme.Colors["tileColor2"])
                    : (Color)ColorConverter.ConvertFromString(theme.Colors["tileColor1"]);
                
                Color c2 = switchTileColors
                    ? (Color)ColorConverter.ConvertFromString(theme.Colors["tileColor1"])
                    : (Color)ColorConverter.ConvertFromString(theme.Colors["tileColor2"]);
                
                // Color c1 = (Color)ColorConverter.ConvertFromString(theme.Colors["tileColor1"]);
                // Color c2 = (Color)ColorConverter.ConvertFromString(theme.Colors["tileColor2"]);
                Color sFont = (Color)ColorConverter.ConvertFromString(theme.Colors["fontColor"]);
                Color sBg = (Color)ColorConverter.ConvertFromString(theme.Colors["bgColor"]);
                SettingsMenu.SetColors(sFont, sBg);

                foreach (var color in theme.Colors)
                {   
                    ColorEntry newColorEntry = new ColorEntry(color.Key, color.Value, c1, c2);
                    newColorEntry.colorPicker.SelectedColorChanged += (sender, e) =>
                    {
                        ColorPicker_SelectedColorChanged(sender, e, newColorEntry);
                    };
                    Panel.Children.Add(newColorEntry);
                    Console.WriteLine($"Name: {color.Key} - Value: {color.Value}");
                }

                Utils.SetColors(theme.Colors);
                SettingsMenu.MainUpdateMethod();
            }
        }
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

    private void SwitchColors(ColorEntry ce1, ColorEntry ce2)
    {
        ColorEntry temp = ce1;
        ce1.ColorName = ce2.ColorName;
        ce1.ColorValue = ce2.ColorValue;
        ce1.colorPicker.Background = ce2.colorPicker.Background;
        ce1.colorPicker.Foreground = ce2.colorPicker.Foreground;
        ce1.colorPicker.SelectedColor = ce2.colorPicker.SelectedColor;
        
        ce2.ColorName = temp.ColorName;
        ce2.ColorValue = temp.ColorValue;
        ce2.colorPicker.Background = temp.colorPicker.Background;
        ce2.colorPicker.Foreground = temp.colorPicker.Foreground;
        ce2.colorPicker.SelectedColor = temp.colorPicker.SelectedColor;
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
            SettingsMenu.MainUpdateMethod();
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