using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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

    public EventPopup(string text, EventType eventType = EventType.Positive)
    {
        StackPanel popPanel = Application.Current.MainWindow.FindName("PopPanel") as StackPanel;

        Grid grid = new Grid { Width = W, Height = H, Effect = AppEffects.DropShadowRectangle };
        
        Rectangle bg = new Rectangle
        {
            Width = W, Height = H,
            RadiusX = 10, RadiusY = 10,
        };
        BindingHelper.SetColorBinding(bg, Shape.FillProperty, "Button");
        grid.Children.Add(bg);

        Border timeBorder = new Border
        {
            Width = W, Height = 8,
            CornerRadius = new CornerRadius(0, 0, 9, 9),
            Background = new SolidColorBrush(ColorHelper.AdjustBrightness(
                (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Button"]),
                0.7)),
            VerticalAlignment = VerticalAlignment.Bottom,
        };
        grid.Children.Add(timeBorder);

        TextBlock textBlock = new TextBlock
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            FontSize = Common.TextFontSize,
            TextWrapping = TextWrapping.Wrap,
            Effect = AppEffects.dropShadowText,
            Margin = new Thickness(7, 2, 0, 0),
        };
        BindingHelper.SetColorBinding(textBlock, TextBlock.ForegroundProperty, "Font");
        grid.Children.Add(textBlock);

        popPanel.Children.Add(grid);
    }
}