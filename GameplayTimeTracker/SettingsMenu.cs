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

public class SettingsMenu : UserControl
{
    public double WinWidth { get; set; }

    public double WinHeight { get; set; }

    public double W { get; set; }
    public double H { get; set; }

    public bool IsToggled { get; set; }
    public bool IsImageSet { get; set; }
    public bool wasSet { get; set; }
    public bool ToClose { get; set; }

    public string Type { get; set; }

    private RoutedEventHandler ButtonAction1;
    private RoutedEventHandler ButtonAction2;
    private DispatcherTimer blurUpdateTimer;

    public Grid ContainerGrid;
    public Grid MenuContainerGrid { get; set; }
    public Grid SettingsGrid { get; set; }
    public Settings Settings { get; set; }
    public List<Theme> Themes { get; set; }


    private DoubleAnimation fadeInAnimation = new();
    private DoubleAnimation otherFadeInAnimation = new();
    private DoubleAnimation fadeOutAnimation = new();
    private DoubleAnimation otherFadeOutAnimation = new();
    private DoubleAnimation zoomInAnimation = new();
    private DoubleAnimation zoomOutAnimation = new();
    private DoubleAnimation zoomOutAnimation2 = new();
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
    private TextBlock _lastClickedTextBlock;

    private Window mainWindow;

    private double _zoomPercent = 1.07;
    int bRadius = 10;
    private double bgRefreshRate = 1.0;

    public Image bgImage;

    public SettingsMenu()
    {
    }


    private void Dummy(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Dummy method called!");
    }

    private void UpdateUnderline(TextBlock clickedTextBlock)
    {
        // Remove underline from the previously clicked TextBlock
        if (_lastClickedTextBlock != null)
        {
            _lastClickedTextBlock.FontWeight = FontWeights.Regular;
            // _lastClickedTextBlock.TextDecorations = null;
        }

        // Add underline to the newly clicked TextBlock
        // clickedTextBlock.TextDecorations = TextDecorations.Underline;
        clickedTextBlock.FontWeight = FontWeights.Bold;

        // Update the reference
        _lastClickedTextBlock = clickedTextBlock;
    }


    public SettingsMenu(Grid containerGrid, Grid menuGrid, Settings settings, double w = 350, double h = 400,
        bool isToggled = false,
        string type = "yesNo",
        RoutedEventHandler routedEvent1 = null, RoutedEventHandler routedEvent2 = null)
    {
        mainWindow = Utils.GetMainWindow();
        mainWindow.SizeChanged += MainWindow_SizeChanged;
        ContainerGrid = containerGrid;
        SettingsGrid = menuGrid;
        Settings = settings;
        Themes = settings.ThemeList;
        WinHeight = mainWindow.RenderSize.Height;
        WinWidth = mainWindow.RenderSize.Width;
        W = w;
        H = h;
        IsToggled = isToggled;
        Type = type;
        ButtonAction1 = routedEvent1 == null ? Dummy : routedEvent1;
        ButtonAction2 = routedEvent2 == null ? Dummy : routedEvent2;

        wasSet = false;
        IsImageSet = false;
        ToClose = false;

        ThemeMenu tm = new ThemeMenu(mainWindow.FindName("ContentPanel") as StackPanel, Themes, Settings.SelectedTheme);
        PrefMenu pm = new PrefMenu(mainWindow.FindName("ContentPanel") as StackPanel, Settings);

        StackPanel headerPanel = mainWindow.FindName("ContentPanel") as StackPanel;

        TextBlock PrefBlock = headerPanel.FindName("Pref") as TextBlock;
        PrefBlock.MouseDown += (sender, e) =>
        {
            UpdateUnderline(PrefBlock);
            pm.CreateMenu(sender, e);
        };
        
        TextBlock ThemesBlock = headerPanel.FindName("Themes") as TextBlock;
        ThemesBlock.MouseDown += (sender, e) =>
        {
            UpdateUnderline(ThemesBlock);
            tm.CreateMenu(sender, e);
        };
        
        pm.CreateMenuMethod();
        UpdateUnderline(PrefBlock);

        blurUpdateTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(bgRefreshRate)
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

        zoomOutAnimation2 = new DoubleAnimation
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
            Duration = new Duration(TimeSpan.FromSeconds(0.5)),
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

    public void OpenMenu()
    {
        Console.WriteLine("Opening Menu...");
        double padding = 10;
        CreateBlurOverlay();

        SettingsGrid.Visibility = Visibility.Visible;
        Console.WriteLine();
        SettingsGrid.BeginAnimation(MarginProperty, rollInAnimation);
        blurUpdateTimer.Start();
        IsToggled = true;
    }

    public void CloseMenu(object sender, RoutedEventArgs e)
    {
        ToClose = true;
        SetBlurImage(true);
        blurUpdateTimer.Stop();
        blurUpdateTimer.Tick -= (s, ev) => SetBlurImage();
        mainWindow.SizeChanged -= MainWindow_SizeChanged;

        Console.WriteLine("Closing Menu...");

        zoomOutAnimation2.Completed += (s, ev) =>
        {
            // MenuContainerGrid.Children.Clear();
            SettingsGrid.Visibility = Visibility.Collapsed;
            bgImage.Source = null;
            Console.WriteLine("Menu Closed!");
        };

        SettingsGrid.BeginAnimation(MarginProperty, rollOutAnimation);
        bgImage.Effect.BeginAnimation(BlurEffect.RadiusProperty, blurOutAnimation);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, zoomOutAnimation);
        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, zoomOutAnimation2);

        // Reset state flags
        IsToggled = false;
        IsImageSet = false;
        ToClose = false;

        Console.WriteLine("Menu closed!");
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
        bgImage.MouseDown += CloseMenu;

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