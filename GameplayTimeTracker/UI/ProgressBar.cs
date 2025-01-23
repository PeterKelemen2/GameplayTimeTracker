using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameplayTimeTracker;

public class ProgressBar : UserControl, INotifyPropertyChanged
{
    public double BgWidth { get; set; }
    public double BgHeight { get; set; }
    public double BarPadding { get; set; }
    public double CornerRadius { get; set; }

    private double _percentage;
    private double resolution = 100;

    public static readonly DependencyProperty PercentageProperty =
        DependencyProperty.Register(
            nameof(Percentage),
            typeof(double),
            typeof(ProgressBar),
            new PropertyMetadata(0.0, OnPercentageChanged));

    public double Percentage
    {
        get => (double)GetValue(PercentageProperty);
        set => SetValue(PercentageProperty, value);
    }

    public double InnerMaxWidth { get; set; }
    private Grid ContainerGrid { get; set; }
    public Rectangle BackgroundRect { get; set; }
    public Rectangle BarRect { get; set; }

    private DispatcherTimer progressBarTimer;
    private bool isProgressingUp = true;

    public static readonly DependencyProperty MarginProperty =
        DependencyProperty.Register("Margin", typeof(Thickness), typeof(ProgressBar),
            new PropertyMetadata(new Thickness(0), OnMarginChanged));

    public ProgressBar(double width, double height, double padding, double cornerRadius, double percentage = 0.00)
    {
        BgWidth = width;
        BgHeight = height;
        BarPadding = padding;
        CornerRadius = cornerRadius;
        // Percentage = percentage;
        InnerMaxWidth = width - padding * 2;

        ContainerGrid = new Grid
        {
            Width = BgWidth,
            Height = BgHeight,
        };

        BackgroundRect = new Rectangle
        {
            Width = BgWidth,
            Height = BgHeight,
            RadiusX = CornerRadius,
            RadiusY = CornerRadius,
            // Fill = new SolidColorBrush(AppColors.Background),
            Effect = AppEffects.dropShadowText,
        };
        ContainerGrid.Children.Add(BackgroundRect);

        BarRect = new Rectangle
        {
            Width = InnerMaxWidth * Percentage,
            Height = BgHeight - 2 * BarPadding,
            RadiusX = CornerRadius - BarPadding / 2,
            RadiusY = CornerRadius - BarPadding / 2,
            // Fill = ColorHelper.CreateLinGradBrushHor(AppColors.ProgressBar1, AppColors.ProgressBar2),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(BarPadding, 0, 0, 0)
        };
        ContainerGrid.Children.Add(BarRect);

        // UpdateBarRectWidth();
        Content = ContainerGrid;
        // StartProgressBarOscillation();
    }

    public void UpdateBgWidth(double newWidth)
    {
        if (newWidth != BgWidth)
        {
            BgWidth = newWidth;
            ContainerGrid.Width = BgWidth;
            BackgroundRect.Width = BgWidth;
            InnerMaxWidth = BgWidth - 2 * BarPadding;
            UpdateBarRectWidth();
        }
    }

    private void UpdateBarRectWidth()
    {
        if (BarRect != null)
        {
            double clampedValue = Math.Clamp(Percentage, 0, 1);
            double newWidth = InnerMaxWidth * clampedValue;
            if (newWidth > 0.0 && Math.Abs(BarRect.Width - newWidth) >= InnerMaxWidth * (1 / resolution))
                BarRect.Width = newWidth;
        }
    }

    private void StartProgressBarOscillation()
    {
        progressBarTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(8)
        };

        bool isProgressingUp = true;
        progressBarTimer.Tick += (sender, e) =>
        {
            Percentage += isProgressingUp ? 0.01 : -0.01;
            if (Percentage >= 0.99 || Percentage <= 0.01)
            {
                isProgressingUp = !isProgressingUp;
            }
        };

        progressBarTimer.Start();
    }

    private static void OnMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ProgressBar pBar && pBar.ContainerGrid != null)
        {
            pBar.ContainerGrid.Margin = (Thickness)e.NewValue;
        }
    }

    private static void OnPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ProgressBar progressBar)
        {
            progressBar.UpdateBarRectWidth();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}