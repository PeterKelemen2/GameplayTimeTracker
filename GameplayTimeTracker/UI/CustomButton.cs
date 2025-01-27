using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameplayTimeTracker;

public class CustomButton : UserControl
{
    public BType ButtonType { get; set; }

    public Rectangle ButtonBase;

    // private string ButtonImagePath;
    public Image ButtonImage { get; set; }
    public Color ButtonColor { get; set; }
    public Color ButtonHoverColor { get; set; }
    public Color ButtonPressedColor { get; set; }
    public Grid Grid { get; set; }
    public Effect ButtonEffect { get; set; }

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

    public static readonly DependencyProperty EffectProperty =
        DependencyProperty.Register("Effect", typeof(Effect), typeof(CustomButton),
            new PropertyMetadata(null, OnEffectChanged));

    public static readonly DependencyProperty ActiveProperty =
        DependencyProperty.Register(nameof(Active), typeof(bool), typeof(CustomButton),
            new PropertyMetadata(true, OnActiveChanged));

    public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
        "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomButton));

    // CLR Wrapper for the Click Event
    public event RoutedEventHandler Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    public CustomButton(double w = 100, double h = 30,
        BType type = BType.Default, double bRad = 7,
        string text = "", double fontSize = 16, bool isBold = true,
        string bImgPath = "", Effect effect = null,
        HorizontalAlignment hA = HorizontalAlignment.Center,
        VerticalAlignment vA = VerticalAlignment.Center)
    {
        bool isActive = (bool)GetValue(ActiveProperty);

        // ButtonImagePath = bImgPath;
        ButtonType = type;
        ButtonEffect ??= effect;

        Grid = new Grid
        {
            Width = w, Height = h,
            HorizontalAlignment = hA, VerticalAlignment = vA,
            Effect = ButtonEffect,
        };

        ButtonBase = new Rectangle
        {
            Width = w, Height = h,
            RadiusX = bRad, RadiusY = bRad,
            Fill = isActive ? new SolidColorBrush(ButtonColor) : new SolidColorBrush(Colors.Gray),
        };
        Grid.Children.Add(ButtonBase);
        SetButtonColors();

        TextBlock buttonTextBlock = new TextBlock
        {
            Text = text,
            Foreground = new SolidColorBrush(Colors.Black),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = fontSize,
            FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal,
            Margin = new Thickness(0, -3, 0, 0),
        };
        if (!buttonTextBlock.Text.Equals(""))
        {
            Grid.Children.Add(buttonTextBlock);
        }

        if (!bImgPath.Equals(""))
        {
            if (File.Exists(bImgPath))
            {
                ButtonImage = new Image();
                ButtonImage.Source = new BitmapImage(new Uri(bImgPath, UriKind.RelativeOrAbsolute));
                ButtonImage.Width = h / 2;
                ButtonImage.Height = h / 2;
                ButtonImage.HorizontalAlignment = HorizontalAlignment.Center;
                RenderOptions.SetBitmapScalingMode(ButtonImage, BitmapScalingMode.HighQuality);

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
            else
            {
                Console.WriteLine($"Could not find file: {bImgPath}");
            }
        }

        if (isActive)
        {
            Grid.MouseEnter += OnMouseEnter;
            Grid.MouseLeave += OnMouseLeave;
            Grid.MouseLeftButtonDown += OnMouseLeftButtonDown;
            Grid.MouseLeftButtonUp += OnMouseLeftButtonUp;
            ButtonBase.Fill = new SolidColorBrush(ButtonColor);
        }

        Content = Grid;
    }

    // TODO: Implement color change
    public void SetButtonColors()
    {
        switch (ButtonType)
        {
            case BType.Default:
                ButtonColor = AppColors.DefButton;
                break;
            case BType.Positive:
                ButtonColor = AppColors.PositiveButton;
                // ButtonHoverColor = ColorHelper.AdjustBrightness(ButtonColor, 1.2);
                // ButtonPressedColor = ColorHelper.AdjustBrightness(ButtonColor, 0.8);
                break;
            case BType.Negative:
                ButtonColor = AppColors.NegativeButton;
                // ButtonHoverColor = ColorHelper.AdjustBrightness(ButtonColor, 1.2);
                // ButtonPressedColor = ColorHelper.AdjustBrightness(ButtonColor, 0.8);
                break;
            default:
                ButtonColor = AppColors.DefButton;
                // ButtonHoverColor = ColorHelper.AdjustBrightness(ButtonColor, 1.2);
                // ButtonPressedColor = ColorHelper.AdjustBrightness(ButtonColor, 0.8);
                break;
        }
        ButtonHoverColor = ColorHelper.AdjustBrightness(ButtonColor, 1.2);
        ButtonPressedColor = ColorHelper.AdjustBrightness(ButtonColor, 0.8);
        ButtonBase.Fill = new SolidColorBrush(ButtonColor);
    }

    public void Enable()
    {
        Grid.MouseEnter += OnMouseEnter;
        Grid.MouseLeave += OnMouseLeave;
        Grid.MouseLeftButtonDown += OnMouseLeftButtonDown;
        Grid.MouseLeftButtonUp += OnMouseLeftButtonUp;
        ButtonBase.Fill = new SolidColorBrush(ButtonColor);
    }

    public void Disable()
    {
        Grid.MouseEnter -= OnMouseEnter;
        Grid.MouseLeave -= OnMouseLeave;
        Grid.MouseLeftButtonDown -= OnMouseLeftButtonDown;
        Grid.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        ButtonBase.Fill = new SolidColorBrush(Colors.Gray);
    }

    private double animTime = 0.1;

    private void OnMouseEnter(object sender, MouseEventArgs e)
    {
        ButtonBase.Fill = new SolidColorBrush(ButtonHoverColor); // Change to hover color

        // Animate scale transform
        if (Grid.Children.Contains(ButtonBase))
        {
            ScaleTransform scaleTransform = new ScaleTransform(1.0, 1.0); // Initial size
            Grid.RenderTransform = scaleTransform;
            Grid.RenderTransformOrigin = new Point(0.5, 0.5); // Set origin to center

            DoubleAnimation scaleAnimation = new DoubleAnimation
            {
                To = 1.05, // Target scale
                Duration = TimeSpan.FromSeconds(animTime),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
            };
            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
        }
    }

    private void OnMouseLeave(object sender, MouseEventArgs e)
    {
        ButtonBase.Fill = new SolidColorBrush(ButtonColor); // Revert to default color

        if (Grid.Children.Contains(ButtonBase))
        {
            // Animate scale back to normal
            ScaleTransform scaleTransform = Grid.RenderTransform as ScaleTransform;
            if (scaleTransform != null)
            {
                DoubleAnimation scaleAnimation = new DoubleAnimation
                {
                    To = 1.0, // Target scale
                    Duration = TimeSpan.FromSeconds(animTime),
                    EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
                };
                scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
                scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
            }
        }
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ButtonBase.Fill = new SolidColorBrush(ButtonPressedColor); // Change color for feedback
        e.Handled = true;
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        ButtonBase.Fill = new SolidColorBrush(ButtonColor); // Revert color
        RaiseEvent(new RoutedEventArgs(ClickEvent)); // Raise the Click event
        e.Handled = true;
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

    private static void OnEffectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CustomButton customButton && customButton.Grid != null)
        {
            var newEffect = (Effect)e.NewValue;
            customButton.Grid.Effect = newEffect;
        }
    }

    private static void OnActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CustomButton button)
        {
            bool newValue = (bool)e.NewValue;
            button.OnActiveChanged(newValue);
        }
    }

    protected virtual void OnActiveChanged(bool newValue)
    {
        if (newValue) Enable();
        else Disable();
    }

    public bool Active
    {
        get => (bool)GetValue(ActiveProperty);
        set => SetValue(ActiveProperty, value);
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

    public Effect Effect
    {
        get => (Effect)GetValue(EffectProperty);
        set => SetValue(EffectProperty, value);
    }
}