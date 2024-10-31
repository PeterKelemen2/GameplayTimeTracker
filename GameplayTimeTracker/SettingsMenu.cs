using System;
using System.Collections.Generic;
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
    public SettingsMenu(Grid containerGrid, bool isBlurToggled = false)
    {
        ContainerGrid = containerGrid;
        IsBlurToggled = isBlurToggled;
        WinHeight = 650;
        WinWidth = 800;

        fadeInAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = new Duration(TimeSpan.FromSeconds(0.5)),
            FillBehavior = FillBehavior.HoldEnd, // Holds the end value after the animation completes
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        fadeOutAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = new Duration(TimeSpan.FromSeconds(0.5)),
            FillBehavior = FillBehavior.HoldEnd, // Holds the end value after the animation completes
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

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
            To = new Thickness(0, 0, 0, -WinHeight),
            Duration = new Duration(TimeSpan.FromSeconds(0.25)),
            FillBehavior = FillBehavior.HoldEnd, // Holds the end value after the animation completes
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };
    }

    public int WinWidth { get; set; }

    public int WinHeight { get; set; }

    public bool IsBlurToggled { get; set; }
    public bool IsToggled { get; set; }

    public Grid ContainerGrid { get; set; }
    public Grid settingsContainerGrid { get; set; }

    private DoubleAnimation fadeInAnimation;
    private DoubleAnimation fadeOutAnimation;
    private ThicknessAnimation rollInAnimation;
    private ThicknessAnimation rollOutAnimation;

    public Image bgImage;

    public void OpenSettingsWindow(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Open Settings Window");
        IsBlurToggled = true;
        IsToggled = true;

        CreateBlurOverlay();
        bgImage.BeginAnimation(OpacityProperty, fadeInAnimation);

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

        ContainerGrid.Children.Add(settingsContainerGrid);
        // ContainerGrid.BeginAnimation(MarginProperty, rollInAnimation);
        settingsContainerGrid.BeginAnimation(MarginProperty, rollInAnimation);
    }

    public void CloseSettingsWindow(object sender, RoutedEventArgs e)
    {
        // Fade-out animation on bgImage
        fadeOutAnimation.Completed += (s, args) =>
        {
            // TODO: Time them correctly
            rollOutAnimation.Completed += (s, args) =>
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

                // Remove the marked children after the fade-out animation completes
                foreach (UIElement child in childrenToRemove)
                {
                    ContainerGrid.Children.Remove(child);
                }

                // Set IsBlurToggled to false after closing
                IsBlurToggled = false;
            };
            settingsContainerGrid.BeginAnimation(MarginProperty, rollOutAnimation);
        };

        bgImage.BeginAnimation(OpacityProperty, fadeOutAnimation);
    }


    private void CreateBlurOverlay()
    {
        int bRadius = 10;
        BitmapSource settingsBgBitmap = Utils.CaptureCurrentWindow();
        BitmapSource extendedBitmap = Utils.ExtendEdgesAroundCenter(settingsBgBitmap, bRadius);

        bgImage = new Image
        {
            Source = extendedBitmap,
            Width = WinWidth - bRadius / 2,
            Height = WinHeight - bRadius / 2,
            Stretch = Stretch.Uniform,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            // Margin = new Thickness(-bRadius, -bRadius, 0, 0),
            Effect = Utils.blurEffect
        };

        // Add the final image to the container
        ContainerGrid.Children.Add(bgImage);
        // bgImage.BeginAnimation(Image.OpacityProperty, fadeAnimation);
    }
}