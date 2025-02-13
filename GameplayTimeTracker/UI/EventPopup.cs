using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameplayTimeTracker;

public enum EventType
{
    Positive,
    Negative,
}

public class EventPopup : UserControl
{
    private double W = 250;
    private double H = 70;
    private double animDuration = 0.5;
    double shownDuration = 3;
    private Grid grid;
    private Border gridBorder;
    private Border timeIndicatorBorder;
    private Border shadowBorder;
    private DispatcherTimer dispatcherTimer;

    public EventPopup(string text, EventType eventType = EventType.Positive)
    {
        StackPanel popPanel = Application.Current.MainWindow.FindName("PopPanel") as StackPanel;

        grid = new Grid { Width = W, Height = H };

        gridBorder = new Border
        {
            Child = grid,
            RenderTransform = new TranslateTransform(),
            CornerRadius = new CornerRadius(10),
            Clip = new RectangleGeometry(new Rect(0, 0, W, H), 10, 10),
            Margin = new Thickness(5)
        };

        shadowBorder = new Border
        {
            Effect = AppEffects.DropShadowRectangle, // Apply shadow to this wrapper
            Child = gridBorder
        };

        timeIndicatorBorder = new Border
        {
            Width = W + 3, Height = 8,
            CornerRadius = new CornerRadius(0, 3, 0, 0),
            Effect = AppEffects.dropShadowText,
            VerticalAlignment = VerticalAlignment.Bottom, HorizontalAlignment = HorizontalAlignment.Left,
        };
        grid.Children.Add(timeIndicatorBorder);

        TextBlock textBlock = new TextBlock
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top,
            FontSize = Common.TextFontSize, TextWrapping = TextWrapping.Wrap,
            Effect = AppEffects.dropShadowText, Margin = new Thickness(7, 2, 0, 0),
        };
        BindingHelper.SetColorBinding(textBlock, TextBlock.ForegroundProperty, "Font");
        grid.Children.Add(textBlock);

        switch (eventType)
        {
            case EventType.Positive:
                BindingHelper.SetColorBinding(gridBorder, BackgroundProperty, "Button");
                timeIndicatorBorder.Background = new SolidColorBrush(ColorHelper.AdjustBrightness(
                    (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Button"]),
                    0.7));
                break;
            case EventType.Negative:
                BindingHelper.SetColorBinding(gridBorder, BackgroundProperty, "Negative Button");
                timeIndicatorBorder.Background = new SolidColorBrush(ColorHelper.AdjustBrightness(
                    (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Negative Button"]),
                    0.7));
                break;
        }

        popPanel.Children.Add(shadowBorder);

        dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(shownDuration) };
        dispatcherTimer.Tick += ClosePopup;

        Open();
    }

    private void Close()
    {
        dispatcherTimer.Stop();
        Console.WriteLine(" ####### CLOSING EVENT POPUP");
        DoubleAnimation fadeOut = new DoubleAnimation
        {
            To = 0,
            Duration = TimeSpan.FromSeconds(animDuration),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        DoubleAnimation slideOutEventAnimation = new DoubleAnimation
        {
            From = 0,
            To = W * 2,
            Duration = TimeSpan.FromSeconds(animDuration),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
        };

        TranslateTransform transform = new TranslateTransform();
        gridBorder.RenderTransform = transform;
        transform.BeginAnimation(TranslateTransform.XProperty, slideOutEventAnimation);

        gridBorder.BeginAnimation(OpacityProperty, fadeOut);
    }

    private void Open()
    {
        DoubleAnimation slideInEventAnimation = new DoubleAnimation
        {
            From = W * 2,
            To = 0,
            Duration = TimeSpan.FromSeconds(animDuration),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        DoubleAnimation shrinkTime = new DoubleAnimation
        {
            To = 0,
            Duration = TimeSpan.FromSeconds(shownDuration),
        };

        DoubleAnimation fadeIn = new DoubleAnimation
        {
            From = 0,
            To = 1,
            Duration = TimeSpan.FromSeconds(animDuration),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
        };
        fadeIn.Completed += (_, __) =>
        {
            dispatcherTimer.Start();
            timeIndicatorBorder.BeginAnimation(Border.WidthProperty, shrinkTime);
        };
        // shrinkTime.Completed += (_, __) => { Close(); };


        TranslateTransform transform = new TranslateTransform();
        gridBorder.RenderTransform = transform;
        transform.BeginAnimation(TranslateTransform.XProperty, slideInEventAnimation);

        gridBorder.BeginAnimation(OpacityProperty, fadeIn);
    }


    private void ClosePopup(object? sender, EventArgs e)
    {
        Close();
    }
}