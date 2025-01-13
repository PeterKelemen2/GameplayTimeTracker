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

    public double Percentage
    {
        get => _percentage;
        set
        {
            if (_percentage != value)
            {
                _percentage = value;
                OnPropertyChanged(nameof(Percentage)); // Notify that the percentage changed
                UpdateBarWidth(); // Update the BarRect width when Percentage changes
            }
        }
    }

    public double InnerMaxWidth { get; set; }

    private Grid ContainerGrid { get; set; }
    private Rectangle BackgroundRect { get; set; }
    private Rectangle BarRect { get; set; }

    private DispatcherTimer progressBarTimer;
    private bool isProgressingUp = true;

    public static readonly DependencyProperty MarginProperty =
        DependencyProperty.Register("Margin", typeof(Thickness), typeof(ProgressBar),
            new PropertyMetadata(new Thickness(0), OnMarginChanged));

    public ProgressBar(double width, double height, double padding, double cornerRadius, double percentage = 0.0)
    {
        BgWidth = width;
        BgHeight = height;
        BarPadding = padding;
        CornerRadius = cornerRadius;
        Percentage = percentage;
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
            Fill = new SolidColorBrush(AppColors.Background),
            Effect = AppEffects.dropShadowText,
        };
        ContainerGrid.Children.Add(BackgroundRect);

        BarRect = new Rectangle
        {
            Width = InnerMaxWidth * Percentage,
            Height = BgHeight - 2 * BarPadding,
            RadiusX = CornerRadius - BarPadding / 2,
            RadiusY = CornerRadius - BarPadding / 2,
            Fill = AppColors.CreateLinGradBrushHor(AppColors.ProgressBar1, AppColors.ProgressBar2),
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(BarPadding, 0, 0, 0)
        };
        ContainerGrid.Children.Add(BarRect);

        UpdateBarWidth();
        Content = ContainerGrid;
        // StartProgressBarOscillation();
    }

    private void UpdateBarWidth()
    {
        if (BarRect != null)
        {
            Percentage = Math.Clamp(Percentage, 0, 1);
            double newWidth = InnerMaxWidth * Percentage;
            if (newWidth > 0.0 && Math.Abs(BarRect.Width - newWidth) >= InnerMaxWidth * 0.01)
            {
                BarRect.Width = newWidth;
            }
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

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}