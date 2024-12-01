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
        Panel.Children.Clear(); // Clear all children first
        comboBox = new ComboBox
        {
            Width = 150,
            Height = 30,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10)
        };

        comboBox.SelectionChanged += (sender, e) =>
        {
            AddColorEntries(); // Call the method to add color entries
        };

        foreach (var theme in Themes)
        {
            comboBox.Items.Add(theme.ThemeName);
        }

        comboBox.SelectedIndex = 0; // Set the default selection
        Panel.Children.Add(comboBox);

        AddColorEntries(); // Populate initial color entries based on the default selection
    }

    private void AddColorEntries()
    {
        // Remove all children except the first one (the ComboBox)
        for (int i = Panel.Children.Count - 1; i >= 0; i--)
        {
            var child = Panel.Children[i];
            if (child is not ComboBox) // If it's not a ComboBox, remove it
            {
                Console.WriteLine(child); // Log the child that is being removed
                Panel.Children.RemoveAt(i);
            }
        }

        // Add color entries for the selected theme
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