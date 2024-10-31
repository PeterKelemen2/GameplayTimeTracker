using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameplayTimeTracker;

public class SettingsMenu : UserControl
{
    public SettingsMenu(Grid containerGrid, bool isToggled = false)
    {
        ContainerGrid = containerGrid;
        WinHeight = 650;
        WinWidth = 800;
        IsToggled = isToggled;
        IsImageSet = false;
        ToClose = false;

        rollInAnimation = new ThicknessAnimation
        {
            From = new Thickness(0, 0, 0, -WinHeight),
            To = new Thickness(0, 0, 0, 0),
            Duration = new Duration(TimeSpan.FromSeconds(0.25)),
            FillBehavior = FillBehavior.HoldEnd, // Holds the end value after the animation completes
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        rollOutAnimation = new ThicknessAnimation
        {
            From = new Thickness(0, 0, 0, 0),
            To = new Thickness(0, 0, 0, -1.5 * WinHeight),
            Duration = new Duration(TimeSpan.FromSeconds(0.25)),
            FillBehavior = FillBehavior.HoldEnd, // Holds the end value after the animation completes
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        zoomInAnimation = new DoubleAnimation
        {
            From = 1.0, // Starting scale (normal size)
            To = _zoomPercent, // Ending scale (7% zoom in)
            Duration = new Duration(TimeSpan.FromSeconds(0.5)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        zoomOutAnimation = new DoubleAnimation
        {
            From = _zoomPercent,
            To = 1.0,
            Duration = new Duration(TimeSpan.FromSeconds(0.5)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        blurInAnimation = new DoubleAnimation
        {
            From = 0, // Start with no blur
            To = bRadius, // Increase to a blur radius of 20 (adjust as needed)
            Duration = new Duration(TimeSpan.FromSeconds(0.5)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        blurOutAnimation = new DoubleAnimation
        {
            From = bRadius, // Start with no blur
            To = 0, // Increase to a blur radius of 20 (adjust as needed)
            Duration = new Duration(TimeSpan.FromSeconds(0.5)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        fadeInAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = new Duration(TimeSpan.FromSeconds(0.05)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        fadeOutAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = new Duration(TimeSpan.FromSeconds(0.05)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
    }

    public int WinWidth { get; set; }

    public int WinHeight { get; set; }

    public bool IsToggled { get; set; }
    public bool IsImageSet { get; set; }
    public bool ToClose { get; set; }

    public Grid ContainerGrid { get; set; }
    public Grid settingsContainerGrid { get; set; }

    private DoubleAnimation fadeInAnimation;
    private DoubleAnimation fadeOutAnimation;
    private DoubleAnimation zoomInAnimation;
    private DoubleAnimation zoomOutAnimation;
    private DoubleAnimation blurInAnimation;
    private DoubleAnimation blurOutAnimation;
    private ThicknessAnimation rollInAnimation;
    private ThicknessAnimation rollOutAnimation;
    private ScaleTransform scaleTransform;

    BitmapSource settingsBgBitmap;
    BitmapSource extendedBitmap;

    private double _zoomPercent = 1.07;
    int bRadius = 10;

    public Image bgImage;

    public void OpenSettingsWindow(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Opening Settings Window...");

        CreateBlurOverlay();

        settingsContainerGrid = new Grid();
        settingsContainerGrid.Margin = new Thickness(0, 0, 0, 0);

        Rectangle settingsRect = new Rectangle
        {
            Width = WinWidth / 2,
            Height = WinHeight / 2,
            Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E2030")),
            RadiusX = Utils.SettingsRadius,
            RadiusY = Utils.SettingsRadius,
            Effect = Utils.dropShadowRectangle
        };
        settingsContainerGrid.Children.Add(settingsRect);
        Button closeButton = new Button
        {
            Style = (Style)Application.Current.FindResource("RoundedButton"),
            Content = "Close",
            Width = 80,
            Height = 30,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(0, 0, 0, WinHeight * 0.25),
        };
        closeButton.Click += CloseSettingsWindow;
        settingsContainerGrid.Children.Add(closeButton);

        TextBlock settingsTitle = new TextBlock
        {
            Text = "Settings",
            Foreground = new SolidColorBrush(Utils.FontColor),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 20,
            FontWeight = FontWeights.Bold,
        };
        settingsTitle.Margin = new Thickness(0, 0, 0, WinHeight * 0.25 + 110);
        settingsContainerGrid.Children.Add(settingsTitle);
        
        TextBlock placeholderText = new TextBlock
        {
            Text = "Work in progress!",
            Foreground = new SolidColorBrush(Utils.FontColor),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 15,
            FontWeight = FontWeights.Bold,
        };
        placeholderText.Margin = new Thickness(0, 0, 0, WinHeight * 0.25 + 50);
        settingsContainerGrid.Children.Add(placeholderText);

        ContainerGrid.Children.Add(settingsContainerGrid);
        settingsContainerGrid.BeginAnimation(MarginProperty, rollInAnimation);

        IsToggled = true;
    }

    public void CloseSettingsWindow(object sender, RoutedEventArgs e)
    {
        ToClose = true;
        SetBlurImage(true);

        Console.WriteLine("Closing Settings Window...");
        // Zoom-out animation on bgImage
        zoomOutAnimation.Completed += (s, args) =>
        {
            // Create a list to store children that need to be removed after the animation
            List<UIElement> childrenToRemove = new List<UIElement>();

            foreach (UIElement child in ContainerGrid.Children)
            {
                if (child is FrameworkElement frameworkElement && frameworkElement.Name != "Grid")
                {
                    childrenToRemove.Add(child); // Mark for removal
                }
            }

            // Remove the marked children after the zoom-out animation completes
            foreach (UIElement child in childrenToRemove)
            {
                ContainerGrid.Children.Remove(child);
            }


            bgImage.BeginAnimation(OpacityProperty, fadeOutAnimation);
        };
        IsToggled = false;
        IsImageSet = false;
        ToClose = false;
        settingsContainerGrid.BeginAnimation(MarginProperty, rollOutAnimation);
        bgImage.Effect.BeginAnimation(BlurEffect.RadiusProperty, blurOutAnimation);

        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, zoomOutAnimation);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, zoomOutAnimation);
    }


    public void SetBlurImage(bool toCapFullSize = false)
    {
        if (IsToggled)
        {
            settingsBgBitmap = ToClose
                ? Utils.CaptureContainerGrid(1.0)
                : Utils.CaptureContainerGrid();
            // extendedBitmap = Utils.ExtendEdgesLeftRight(settingsBgBitmap, bRadius);
            bgImage.Source = settingsBgBitmap;
        }
    }

    private void CreateBlurOverlay()
    {
        settingsBgBitmap = Utils.CaptureContainerGrid();
        // extendedBitmap = Utils.ExtendEdgesLeftRight(settingsBgBitmap, bRadius);
        bgImage = new Image
        {
            Source = settingsBgBitmap,
            Width = WinWidth,
            Height = WinHeight,
            Stretch = Stretch.Fill,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(0, 0, 0, 0)
        };
        // Create a BlurEffect and set its initial Radius to 0 (no blur)
        BlurEffect blurEffect = new BlurEffect { Radius = 0 };
        bgImage.Effect = blurEffect;

        scaleTransform = new ScaleTransform();
        bgImage.RenderTransform = scaleTransform;
        bgImage.RenderTransformOrigin = new Point(0.5, 0.5);

        bgImage.Effect.BeginAnimation(BlurEffect.RadiusProperty, blurInAnimation);
        bgImage.BeginAnimation(OpacityProperty, fadeInAnimation);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, zoomInAnimation);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, zoomInAnimation);
        // Add the final image to the container
        ContainerGrid.Children.Add(bgImage);
    }
}