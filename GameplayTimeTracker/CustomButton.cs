using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameplayTimeTracker;

public class CustomButton : UserControl
{
    private bool isDisabled = false;
    public Rectangle ButtonBase;
    private string ButtonImagePath;
    public Image ButtonImage = new Image();
    public Grid Grid { get; private set; } // Make Grid a property with private set

    // Dependency Properties
    public static readonly DependencyProperty MarginProperty =
        DependencyProperty.Register("Margin", typeof(Thickness), typeof(CustomButton),
            new PropertyMetadata(new Thickness(0), OnMarginChanged));

    public static readonly DependencyProperty OpacityProperty =
        DependencyProperty.Register("Opacity", typeof(double), typeof(CustomButton),
            new PropertyMetadata(1.0, OnOpacityChanged));

    public static readonly DependencyProperty HeightProperty =
        DependencyProperty.Register("Height", typeof(double), typeof(CustomButton),
            new PropertyMetadata(40.0, OnHeightChanged));

    public CustomButton(Grid parent = null, double width = 100, double height = 30, double borderRadius = 7,
        string text = "", double fontSize = 16, bool isBold = true,
        string buttonImagePath = "", bool isDisabled = false)
    {
        Grid = new Grid
        {
            Width = width,
            Height = height
        };
        this.Content = Grid; // Set the inner Grid as the content of the UserControl

        ButtonBase = new Rectangle
        {
            Width = width,
            Height = 30,
            RadiusX = borderRadius,
            RadiusY = borderRadius,
            Fill = new SolidColorBrush(Utils.ButtonColor),
            Effect = Utils.dropShadowIcon
        };
        Grid.Children.Add(ButtonBase);

        TextBlock buttonTextBlock = new TextBlock
        {
            Text = text,
            Foreground = new SolidColorBrush(Colors.Black),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = fontSize,
            FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal,
            Margin = new Thickness(0, 0, 0, fontSize / 4),
        };
        if (!buttonTextBlock.Text.Equals(""))
        {
            Grid.Children.Add(buttonTextBlock);
        }

        if (!string.Equals(buttonImagePath, ""))
        {
            if (File.Exists(buttonImagePath))
            {
                ButtonImage.Source = new BitmapImage(new Uri(buttonImagePath, UriKind.RelativeOrAbsolute));
                ButtonImage.Width = height / 2;
                ButtonImage.Height = height / 2;
                ButtonImage.HorizontalAlignment = HorizontalAlignment.Center;

                if (!buttonTextBlock.Text.Equals(""))
                {
                    double padding = 5;
                    Grid combinedGrid = new();
                    combinedGrid.HorizontalAlignment = HorizontalAlignment.Center;
                    ButtonImage.HorizontalAlignment = HorizontalAlignment.Left;
                    ButtonImage.Margin = new Thickness(0, 0, ButtonImage.Width + padding, 0);
                    buttonTextBlock.Margin = new Thickness(ButtonImage.Width + padding, 0, 0, 0);
                    if (Grid.Children.Contains(buttonTextBlock))
                    {
                        Grid.Children.Remove(buttonTextBlock);
                    }

                    combinedGrid.Children.Add(buttonTextBlock);
                    combinedGrid.Children.Add(ButtonImage);
                    Grid.Children.Add(combinedGrid);
                }
                else
                {
                    Grid.Children.Add(ButtonImage);
                }
            }
        }

        if (parent != null)
        {
            parent.Children.Add(this); // Add this CustomButton to the parent grid
        }
    }

    // Dependency Property Change Callbacks
    private static void OnMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CustomButton customButton && customButton.Grid != null) // Check if Grid is not null
        {
            customButton.Grid.Margin = (Thickness)e.NewValue;
        }
    }

    private static void OnOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CustomButton customButton && customButton.Grid != null) // Check if Grid is not null
        {
            customButton.Grid.Opacity = (double)e.NewValue;
        }
    }

    private static void OnHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CustomButton customButton && customButton.Grid != null) // Check if Grid is not null
        {
            double newHeight = (double)e.NewValue;
            customButton.Grid.Height = newHeight;
            customButton.ButtonBase.Height = newHeight;
            customButton.ButtonImage.Height = newHeight / 2; // Adjust image size accordingly
        }
    }

    // Properties to access the dependency properties
    public new Thickness Margin
    {
        get => (Thickness)GetValue(MarginProperty);
        set => SetValue(MarginProperty, value);
    }

    public new double Opacity
    {
        get => (double)GetValue(OpacityProperty);
        set => SetValue(OpacityProperty, value);
    }

    public new double Height
    {
        get => (double)GetValue(HeightProperty);
        set => SetValue(HeightProperty, value);
    }
}