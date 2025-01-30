using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameplayTimeTracker;

public class DragDropOverlay : UserControl
{
    private Grid DragDropGrid;
    private Rectangle DragOverBg;
    private Rectangle DragOverRect;
    private TextBlock DropText;
    private Window mainWindow = Application.Current.MainWindow;

    public DragDropOverlay()
    {
        DragDropGrid = new Grid();
        DragDropGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
        DragDropGrid.VerticalAlignment = VerticalAlignment.Stretch;

        DragOverBg = new Rectangle
        {
            Width = mainWindow.Width,
            Height = mainWindow.Height,
            Fill = new SolidColorBrush(Colors.Black),
            Opacity = 0.6,
        };
        DragDropGrid.Children.Add(DragOverBg);

        DragOverRect = new Rectangle
        {
            Width = mainWindow.Width - 80,
            Height = mainWindow.Height - 80,
            RadiusX = 30,
            RadiusY = 30,
            Fill = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Button"]))
                { Opacity = 0.3 },
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Stroke = Brushes.White, // Fully opaque stroke
            StrokeThickness = 5,
            StrokeDashArray = new DoubleCollection { 10, 10 },
            Opacity = 1, // Ensure the rectangle itself does not reduce the stroke's opacity
            Effect = AppEffects.DropShadowMedium
        };

        DragDropGrid.Children.Add(DragOverRect);

        DropText = new TextBlock
        {
            Text = "Drop a file here!",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 30,
            Foreground = new SolidColorBrush(Colors.White),
        };
        DragDropGrid.Children.Add(DropText);

        Content = DragDropGrid;
    }

    public void UpdateSize()
    {
        DragOverBg.Height = mainWindow.Height;
        DragOverBg.Width = mainWindow.Width;
        DragOverRect.Height = mainWindow.Height - 80;
        DragOverRect.Width = mainWindow.Width - 80;
    }
}