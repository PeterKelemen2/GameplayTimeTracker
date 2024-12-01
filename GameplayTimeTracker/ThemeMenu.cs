using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Pango;

namespace GameplayTimeTracker;

public class ThemeMenu : UserControl
{
    public Grid container { get; set; }
    public List<Theme> Themes { get; set; }
    public StackPanel Panel { get; set; }
    public ComboBox comboBox { get; set; }

    public ThemeMenu(StackPanel stackPanel, List<Theme> themes)
    {
        Panel = stackPanel;
        Themes = themes;

        CreateDropdown();
    }

    private void CreateDropdown()
    {
        Panel.Children.Clear();
        comboBox = new();
        comboBox.Width = 150;
        comboBox.Height = 30;
        comboBox.HorizontalContentAlignment = HorizontalAlignment.Center;
        comboBox.VerticalContentAlignment = VerticalAlignment.Center;
        comboBox.Margin = new Thickness(10);

        comboBox.SelectionChanged += (sender, e) =>
        {
            string selectedItem = comboBox.SelectedItem.ToString();
            Console.WriteLine($"You selected: {selectedItem}");

            AddColorEntries();
        };

        foreach (var theme in Themes)
        {
            comboBox.Items.Add(theme.ThemeName);
        }

        comboBox.SelectedIndex = 0;
        Panel.Children.Add(comboBox);

        AddColorEntries();
    }
    
    private void AddColorEntries()
    {
        foreach (var theme in Themes)
        {
            if (theme.ThemeName.Equals(comboBox.SelectedItem.ToString()))
            {
                Console.WriteLine($"You selected: {theme.ThemeName}");
                foreach (var color in theme.Colors)
                {
                    Panel.Children.Add(new ColorEntry(color.Key, color.Value));
                    Console.WriteLine($"Name: {color.Key} - Value: {color.Value}");
                }
            }
        }
    }
}