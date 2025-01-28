using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameplayTimeTracker;

public class CustomComboBox : UserControl
{
    public double cornerRadius = 5;
    public double W { get; set; }
    public double H { get; set; }

    private StackPanel mainStackPanel;
    private Grid selectedGrid;
    private StackPanel optionsStackPanel;
    private Border selectedBorder;
    List<string> themeNames = new();

    public CustomComboBox(double w = 120, double h = 30)
    {
        W = w;
        H = h;
        themeNames = new();
        foreach (var theme in Common.Settings.ThemesList)
        {
            themeNames.Add(theme.ThemeName);
        }

        mainStackPanel = new StackPanel { Width = W };
        optionsStackPanel = new StackPanel { Width = W };

        CreateSelectedGrid();
        selectedGrid.MouseEnter += OnMouseEnter;
        selectedGrid.MouseLeave += OnMouseLeave;
        selectedGrid.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        selectedGrid.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        mainStackPanel.Children.Add(selectedGrid);

        ShowOptions(themeNames);

        Content = mainStackPanel;
    }

    public void ShowOptions(List<string> themes)
    {
        optionsStackPanel.Children.Clear();

        foreach (var text in themes)
        {
            TextBlock textBlock = new TextBlock { Text = text, Padding = new Thickness(5) };
            optionsStackPanel.Children.Add(textBlock);
        }

        mainStackPanel.Children.Add(optionsStackPanel);
    }

    public void CreateSelectedGrid()
    {
        if (selectedBorder != null)
        {
            selectedGrid.Children.Clear();
        }
        else
        {
            selectedGrid = new Grid
            {
                Width = W,
                Height = H,
            };
        }

        TextBlock selectedTextBlock = new TextBlock
        {
            Text = Common.Settings.CurrentTheme.ThemeName,
            FontSize = Common.TextFontSize,
            Foreground =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"])),
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new Thickness(10),
        };
        selectedBorder = new Border
        {
            Child = selectedTextBlock,
            // Width = W,
            // Height = H,
            Background =
                new SolidColorBrush(ColorHelper.AdjustBrightness(
                    (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]), 1.5)),
            CornerRadius = new CornerRadius(cornerRadius),
            BorderThickness = new Thickness(1),
            BorderBrush = new SolidColorBrush(ColorHelper.AdjustBrightness(
                (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]), 0.5))
        };
        selectedGrid.Children.Add(selectedBorder);

        Image downArrow = new Image
        {
            Width = H / 2,
            Height = H / 2,
            Source = new BitmapImage(new Uri(AppFiles.ArrowIcon, UriKind.Relative)),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(3),
        };
        selectedGrid.Children.Add(downArrow);
    }

    private void OnMouseEnter(object sender, MouseEventArgs e)
    {
        selectedBorder.Background =
            new SolidColorBrush(ColorHelper.AdjustBrightness(
                (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]), 1.2));
    }

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
        selectedBorder.Background =
            new SolidColorBrush(ColorHelper.AdjustBrightness(
                (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]), 1.5));
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ShowOptions(themeNames);
        e.Handled = true;
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        // ButtonBase.Fill = new SolidColorBrush(ButtonColor); // Revert color
        RaiseEvent(new RoutedEventArgs(ClickEvent)); // Raise the Click event
        e.Handled = true;
    }

    public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
        "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomComboBox));
}