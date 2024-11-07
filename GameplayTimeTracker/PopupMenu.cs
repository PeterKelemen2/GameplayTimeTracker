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
using System.Windows.Threading;

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

    private readonly RoutedEventHandler ButtonAction;
    private DispatcherTimer blurUpdateTimer;

    public Grid ContainerGrid { get; set; }
    public Grid MenuContainerGrid { get; set; }

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

    public PopupMenu(string text = "", double w = 300, double h = 150, bool isToggled = false, string type = "yesNo",
        RoutedEventHandler bAction = null)
    {
        Window mainWindow = Utils.GetMainWindow();
        ContainerGrid = Utils.GetMainWindow().FindName("ContainerGrid") as Grid;
        WinHeight = mainWindow.RenderSize.Height;
        WinWidth = mainWindow.RenderSize.Width;
        MenuText = text;
        W = w;
        H = h;
        IsToggled = isToggled;
        Type = type;
        if (bAction != null) ButtonAction = bAction;
        IsImageSet = false;
        ToClose = false;

        blurUpdateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        blurUpdateTimer.Tick += (s, e) => SetBlurImage();

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
    }

    public void OpenMenu()
    {
        Console.WriteLine("Opening Menu...");

        CreateBlurOverlay();

        MenuContainerGrid = new Grid();
        MenuContainerGrid.Margin = new Thickness(0, 0, 0, 0);

        Rectangle settingsRect = new Rectangle
        {
            Width = W,
            Height = H,
            Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E2030")),
            RadiusX = Utils.SettingsRadius,
            RadiusY = Utils.SettingsRadius,
            Effect = Utils.dropShadowRectangle
        };
        MenuContainerGrid.Children.Add(settingsRect);

        switch (Type)
        {
            case "yesNo":
                Button yesButton = new Button
                {
                    Style = (Style)Application.Current.FindResource("RoundedButton"),
                    Content = "Yes",
                    Width = 80,
                    Height = 30,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(-100, 0, 0, 0),
                };
                Button noButton = new Button
                {
                    Style = (Style)Application.Current.FindResource("RoundedButton"),
                    Content = "No",
                    Width = 80,
                    Height = 30,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(100, 0, 0, 0),
                };
                yesButton.Click += ButtonAction;
                yesButton.Click += CloseMenu;
                noButton.Click += CloseMenu;
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
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, WinHeight * 0.25),
                };
                closeButton.Click += CloseMenu;
                MenuContainerGrid.Children.Add(closeButton);
                break;
        }

        TextBlock settingsTitle = new TextBlock
        {
            Text = "Settings",
            Foreground = new SolidColorBrush(Utils.FontColor),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 20,
            FontWeight = FontWeights.Bold,
        };
        settingsTitle.Margin = new Thickness(0, 0, 0, 0);
        MenuContainerGrid.Children.Add(settingsTitle);

        ContainerGrid.Children.Add(MenuContainerGrid);
        MenuContainerGrid.BeginAnimation(MarginProperty, rollInAnimation);
        
        blurUpdateTimer.Start();
        
        IsToggled = true;
    }

    public void CloseMenu(object sender, RoutedEventArgs e)
    {
        ToClose = true;
        SetBlurImage(true);
        blurUpdateTimer.Stop();
        
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
        MenuContainerGrid.BeginAnimation(MarginProperty, rollOutAnimation);
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