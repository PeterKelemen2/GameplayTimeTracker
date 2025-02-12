using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GameplayTimeTracker;

public class CustomToggleButton : UserControl
{
    public static readonly DependencyProperty IsToggledProperty =
        DependencyProperty.Register(nameof(IsToggled), typeof(bool), typeof(CustomToggleButton),
            new PropertyMetadata(true, OnCheckedChanged));

    public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(
        "Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomToggleButton));

    public event RoutedEventHandler Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    public event Action<bool> ToggledChanged;

    private static void OnCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is CustomToggleButton toggleButton)
        {
            bool newValue = (bool)e.NewValue;
            toggleButton.UpdateVisualState(newValue);

            toggleButton.ToggledChanged?.Invoke(newValue);
        }
    }

    private void UpdateVisualState(bool isToggled)
    {
        if (ButtonBorder != null)
        {
            if (ButtonBorder.Background is not SolidColorBrush backgroundBrush)
            {
                backgroundBrush = new SolidColorBrush(Colors.Gray); // Default initial color
                ButtonBorder.Background = backgroundBrush;
            }

            // Define the target color based on the toggle state
            Color targetColor = isToggled
                ? (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Button"])
                : (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Negative Button"]);

            ColorAnimation colorAnimation = new ColorAnimation
            {
                To = targetColor,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            backgroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

            ThicknessAnimation thicknessAnimation = new ThicknessAnimation
            {
                Duration = TimeSpan.FromSeconds(0.2),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            thicknessAnimation.To = isToggled
                ? new Thickness(KnobSize + padding, 0, 0, 0)
                : new Thickness(0, 0, KnobSize + padding, 0);

            Knob.BeginAnimation(MarginProperty, thicknessAnimation);
        }
    }

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    public Grid ButtonGrid { get; set; }
    public Border ButtonBorder { get; set; }
    public Ellipse Knob { get; set; }
    private double padding;
    private double KnobSize { get; set; }
    private double ButtonWidth { get; set; }

    public CustomToggleButton(double w = 50, double h = 25, bool isActive = true)
    {
        padding = h / 4;
        ButtonWidth = w;
        KnobSize = h - padding;
        ButtonGrid = new Grid();
        ButtonBorder = new Border
        {
            Width = w,
            Height = h,
            CornerRadius = new CornerRadius(h / 2)
        };

        Knob = new Ellipse
        {
            Width = KnobSize,
            Height = KnobSize,
            Fill = new SolidColorBrush(Colors.White),
            Effect = AppEffects.dropShadowText
        };

        ButtonGrid.Children.Add(ButtonBorder);
        ButtonGrid.Children.Add(Knob);
        Content = ButtonGrid;

        // Set the initial state for IsToggled.
        IsToggled = isActive;
        UpdateVisualState(isActive);

        ButtonGrid.MouseLeftButtonDown += OnMouseLeftButtonDown;
        ButtonGrid.MouseLeftButtonUp += OnMouseLeftButtonUp;
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        IsToggled = !IsToggled;
        e.Handled = true;
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClickEvent)); // Raise the Click event
        e.Handled = true;
    }
}