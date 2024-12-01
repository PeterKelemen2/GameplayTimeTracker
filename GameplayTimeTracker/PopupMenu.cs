﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
// using MonoMac.CoreImage;

namespace GameplayTimeTracker;

public class PopupMenu : UserControl
{
    public double WinWidth { get; set; }

    public double WinHeight { get; set; }

    public double W { get; set; }
    public double H { get; set; }

    public bool IsToggled { get; set; }
    public bool IsImageSet { get; set; }
    public bool ToClose { get; set; }

    public string MenuText { get; set; }
    public string Type { get; set; }

    private readonly RoutedEventHandler ButtonAction1;
    private readonly RoutedEventHandler ButtonAction2;
    private DispatcherTimer blurUpdateTimer;

    public Grid ContainerGrid { get; set; }
    public Grid MenuContainerGrid { get; set; }

    private DoubleAnimation fadeInAnimation = new();
    private DoubleAnimation otherFadeInAnimation = new();
    private DoubleAnimation fadeOutAnimation = new();
    private DoubleAnimation otherFadeOutAnimation = new();
    private DoubleAnimation zoomInAnimation = new();
    private DoubleAnimation zoomOutAnimation = new();
    private DoubleAnimation blurInAnimation = new();
    private DoubleAnimation blurOutAnimation = new();
    private ThicknessAnimation rollInAnimation = new();
    private ThicknessAnimation rollOutAnimation = new();
    private ScaleTransform scaleTransform = new();

    private Button yesButton = new();
    private Button noButton = new();

    public BitmapSource menuBgBitmap;
    BitmapSource extendedBitmap;

    private TextBlock menuTitle = new();

    private Window mainWindow;

    private double _zoomPercent = 1.07;
    int bRadius = 10;

    public Image bgImage;

    public PopupMenu()
    {
    }

    public PopupMenu(string text = "", double w = 350, double h = 150, bool isToggled = false,
        string type = "yesNo",
        RoutedEventHandler routedEvent1 = null, RoutedEventHandler routedEvent2 = null)
    {
        mainWindow = Utils.GetMainWindow();
        mainWindow.SizeChanged += MainWindow_SizeChanged;
        ContainerGrid = Utils.GetMainWindow().FindName("ContainerGrid") as Grid;
        WinHeight = mainWindow.RenderSize.Height;
        WinWidth = mainWindow.RenderSize.Width;
        MenuText = text;
        W = w;
        H = h;
        IsToggled = isToggled;
        Type = type;

        if (routedEvent1 != null) ButtonAction1 = routedEvent1;
        if (routedEvent2 != null) ButtonAction2 = routedEvent2;

        IsImageSet = false;
        ToClose = false;

        blurUpdateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        // blurUpdateTimer.Tick += (s, e) => SetBlurImage();

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
            To = new Thickness(0, 0, 0, WinHeight),
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

        otherFadeInAnimation = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = new Duration(TimeSpan.FromSeconds(1)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        otherFadeOutAnimation = new DoubleAnimation
        {
            From = 1,
            To = 0,
            Duration = new Duration(TimeSpan.FromSeconds(1)),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
    }

    // Recalculating dimensions when window size is changing 
    private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Update WinWidth and WinHeight
        WinWidth = e.NewSize.Width;
        WinHeight = e.NewSize.Height;

        rollInAnimation.From = new Thickness(0, 0, 0, -WinWidth);
        rollOutAnimation.To = new Thickness(0, 0, 0, WinWidth + H);
        SetBlurImage();
        // Adjust the height of the rectangle dynamically
        // H = WinHeight * 0.25; // For example, make the height 25% of the window height

        // Update the height of the menuRect if it has been created already
        if (MenuContainerGrid != null)
        {
            foreach (UIElement child in MenuContainerGrid.Children)
            {
                if (child is Rectangle menuRect)
                {
                    menuRect.Height = H;
                }

                if (child is TextBlock menuTitle)
                {
                    menuTitle.Margin = new Thickness(0, WinHeight / 2 - H / 2, 0, 0);
                }
            }

            if (yesButton != null)
            {
                yesButton.Margin = new Thickness(-100, 0, 0, WinHeight / 2 - H / 2);
            }

            if (noButton != null)
            {
                noButton.Margin = new Thickness(100, 0, 0, WinHeight / 2 - H / 2);
            }
        }
    }

    // Opens the menu, creates prompt based on the type of menu it is
    public void OpenMenu()
    {
        Console.WriteLine("Opening Menu...");
        double padding = 10;
        CreateBlurOverlay();

        MenuContainerGrid = new Grid();
        MenuContainerGrid.Margin = new Thickness(0, 0, 0, 0);

        Rectangle menuRect = new Rectangle
        {
            Width = W,
            Height = H,
            Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E2030")),
            RadiusX = Utils.SettingsRadius,
            RadiusY = Utils.SettingsRadius,
            Effect = Utils.dropShadowRectangle
        };
        MenuContainerGrid.Children.Add(menuRect);

        menuTitle = new TextBlock
        {
            Text = MenuText,
            Foreground = new SolidColorBrush(Utils.FontColor),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top,
            FontSize = 20,
            FontWeight = FontWeights.Bold,
            TextWrapping = TextWrapping.Wrap,
            Width = W - 2 * padding,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, WinHeight / 2 - H / 2, 0, 0),
        };

        MenuContainerGrid.Children.Add(menuTitle);

        switch (Type)
        {
            case "yesNo":
                yesButton = new Button
                {
                    Style = (Style)Application.Current.FindResource("RoundedButton"),
                    Content = "Yes",
                    Width = 80,
                    Height = 30,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(-100, 0, 0, WinHeight / 2 - H / 2),
                };
                noButton = new Button
                {
                    Style = (Style)Application.Current.FindResource("RoundedButton"),
                    Content = "No",
                    Width = 80,
                    Height = 30,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(100, 0, 0, WinHeight / 2 - H / 2),
                };
                yesButton.Click += ButtonAction1;
                yesButton.Click += CloseMenu;
                noButton.Click += CloseMenu;
                if (ButtonAction2 != null)
                {
                    noButton.Click += ButtonAction2;
                }

                MenuContainerGrid.Children.Add(yesButton);
                MenuContainerGrid.Children.Add(noButton);
                break;
            default:
                Button closeButton = new Button
                {
                    Style = (Style)Application.Current.FindResource("RoundedButton"),
                    Content = "OK",
                    Width = 80,
                    Height = 30,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, WinHeight / 2 - H / 2),
                };
                closeButton.Click += CloseMenu;
                MenuContainerGrid.Children.Add(closeButton);
                break;
        }

        ContainerGrid.Children.Add(MenuContainerGrid);
        menuTitle.BeginAnimation(OpacityProperty, otherFadeInAnimation);
        MenuContainerGrid.BeginAnimation(MarginProperty, rollInAnimation);
        blurUpdateTimer.Start();
        IsToggled = true;
    }

    // Handles the removal of the elements from any parent
    public void CloseMenu(object sender, RoutedEventArgs e)
    {
        ToClose = true;
        SetBlurImage(true);
        blurUpdateTimer.Stop();
        blurUpdateTimer.Tick -= (s, ev) => SetBlurImage(); // Unsubscribe timer event

        Console.WriteLine("Closing Menu...");
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
        MenuContainerGrid.BeginAnimation(MarginProperty, rollOutAnimation);
        bgImage.Effect.BeginAnimation(BlurEffect.RadiusProperty, blurOutAnimation);
        menuTitle.BeginAnimation(OpacityProperty, otherFadeOutAnimation);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, zoomOutAnimation);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, zoomOutAnimation);

        // MenuContainerGrid.Children.Clear();
        // ContainerGrid.Children.Remove(MenuContainerGrid);
        // mainWindow.SizeChanged -= MainWindow_SizeChanged; // Unsubscribe from events
    }

    // Sets a blurred background for the menu. Parameter is used when fading in and out for maximum clarity.
    public void SetBlurImage(bool toCapFullSize = false)
    {
        if (IsToggled)
        {
            // Capture a new bitmap only when needed
            var newBitmap = ToClose
                ? Utils.CaptureContainerGrid(1.0) // Full size capture
                : Utils.CaptureContainerGrid(); // Partial capture with scaling
        
            // Only update if the new bitmap is different or needs updating
            if (menuBgBitmap != newBitmap)
            {
                // Optionally, dispose of the old bitmap reference if you don't need it anymore
                menuBgBitmap = newBitmap;
        
                // Update the image source with the new bitmap
                bgImage.Source = menuBgBitmap;
            }
        
            // Ensure image dimensions are updated
            bgImage.Width = WinWidth;
            bgImage.Height = WinHeight;
        }
    }

    private void CreateBlurOverlay()
    {
        menuBgBitmap = Utils.CaptureContainerGrid();

        bgImage = new Image
        {
            Source = menuBgBitmap,
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