using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace GameplayTimeTracker;

public class ProgressBar : UserControl
{
    public double BgWidth { get; set; }
    public double BgHeight { get; set; }
    public double BarPadding { get; set; }
    public double CornerRadius { get; set; }
    public double Percentage { get; set; }

    private Grid ContainerGrid { get; set; }
    private Rectangle BackgroundRect { get; set; }
    private Rectangle BarRect { get; set; }

    public static readonly DependencyProperty MarginProperty =
        DependencyProperty.Register("Margin", typeof(Thickness), typeof(ProgressBar),
            new PropertyMetadata(new Thickness(0), OnMarginChanged));
    
    public ProgressBar(double width, double height, double padding, double cornerRadius, double percentage)
    {
        BgWidth = width;
        BgHeight = height;
        BarPadding = padding;
        CornerRadius = cornerRadius;
        Percentage = percentage;

        ContainerGrid = new Grid
        {
            Width = BgWidth,
            Height = BgHeight,
        };

        BackgroundRect = new Rectangle
        {
            Width = BgWidth,
            Height = BgHeight,
            Fill = AppColors.CreateLinGradBrushHor(AppColors.ProgressBar1, AppColors.ProgressBar2),
        };
        ContainerGrid.Children.Add(BackgroundRect);
        
        Content = ContainerGrid;
    }
    
    private static void OnMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ProgressBar pBar && pBar.ContainerGrid != null)
        {
            pBar.ContainerGrid.Margin = (Thickness)e.NewValue;
        }
    }
}