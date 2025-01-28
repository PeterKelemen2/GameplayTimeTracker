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
    public double borderSize = 2;
    public double W { get; set; }
    public double H { get; set; }
    public bool IsOpen { get; set; }

    private StackPanel mainStackPanel;
    private Grid selectedGrid;
    private StackPanel optionsStackPanel;
    private Border selectedBorder;
    private Border optionsBorder;
    private TextBlock selectedTextBlock;
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
        optionsBorder = new Border
        {
            Child = optionsStackPanel,
            Background =
                new SolidColorBrush(ColorHelper.AdjustBrightness(
                    (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]), 1.5)),
            CornerRadius = new CornerRadius(cornerRadius),
            Margin = Margin = new Thickness(0, 5, 0, 5),
            BorderBrush = Brushes.Gray,
        };

        CreateSelectedGrid();
        mainStackPanel.Children.Add(selectedGrid);
        mainStackPanel.Children.Add(optionsBorder);

        IsOpen = false;
        selectedGrid.MouseEnter += OnMouseEnter;
        selectedGrid.MouseLeave += OnMouseLeave;
        selectedGrid.MouseLeftButtonDown += OnMouseLeftButtonDown;
        selectedGrid.MouseLeftButtonUp += OnMouseLeftButtonUp;

        // ShowOptions();

        Content = mainStackPanel;
    }

    public void ToggleOptions()
    {
        optionsStackPanel.Children.Clear();

        if (IsOpen)
        {
            optionsBorder.BorderThickness = new Thickness(0);
            optionsBorder.Visibility = Visibility.Collapsed;
        }
        else
        {
            Console.WriteLine("Showing...");
            foreach (var text in themeNames)
            {
                Console.WriteLine(text);
                TextBlock textBlock = CreateTextBlock(text);
                Border textBorder = new Border
                {
                    Child = textBlock,
                    CornerRadius = new CornerRadius(cornerRadius),
                    Padding = new Thickness(5),
                    Margin = new Thickness(5),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };

                textBorder.MouseLeftButtonDown += (_, _) => { HighlightSelected(textBlock); };
                optionsStackPanel.Children.Add(textBorder);
            }

            optionsBorder.BorderThickness = new Thickness(borderSize);
            optionsBorder.Visibility = Visibility.Visible;
        }

        IsOpen = !IsOpen;
    }

    private void HighlightSelected(TextBlock selected)
    {
        foreach (UIElement child in optionsStackPanel.Children)
        {
            if (child is Border border && border.Child is TextBlock textBlock)
            {
                if (textBlock.Text.Equals(selected.Text))
                {
                    border.Background = new SolidColorBrush(ColorHelper.AdjustBrightness(
                        (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]),
                        0.9));
                }
                else
                {
                    border.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }
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

        selectedTextBlock = CreateTextBlock(Common.Settings.CurrentTheme.ThemeName);

        selectedBorder = new Border
        {
            Child = selectedTextBlock,
            Background =
                new SolidColorBrush(ColorHelper.AdjustBrightness(
                    (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]), 1.5)),
            CornerRadius = new CornerRadius(cornerRadius),
            BorderThickness = new Thickness(borderSize),
            Padding = new Thickness(5),
            // BorderBrush = new SolidColorBrush(ColorHelper.AdjustBrightness(
            //     (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]), 0.5))
            BorderBrush = Brushes.Gray,
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

    private TextBlock CreateTextBlock(string text)
    {
        var textBlock = new TextBlock
        {
            Text = text,
            FontSize = Common.TextFontSize,
            Foreground =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"])),
            VerticalAlignment = VerticalAlignment.Center,
            // Padding = new Thickness(10),
        };

        return textBlock;
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
        ToggleOptions();
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