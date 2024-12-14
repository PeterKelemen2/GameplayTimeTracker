using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using Grid = System.Windows.Controls.Grid;
using Style = System.Windows.Style;

namespace GameplayTimeTracker;

public class SaveIndicator : UserControl
{
    public Grid Container { get; set; }
    public Grid Parent { get; set; }
    public Rectangle BgRectangle { get; set; }
    public TextBlock TextB { get; set; }
    public Double TopMargin { get; set; }

    public SaveIndicator(Grid parent, double topMargin)
    {
        Parent = parent;
        TopMargin = topMargin + Utils.dropShadowRectangle.BlurRadius;
        Container = new Grid
        {
            Width = 130,
            Height = 40,
            Margin = new Thickness(0, -TopMargin, 0, 0),
        };

        BgRectangle = new Rectangle
        {
            Width = Container.Width,
            Height = Container.Height,
            Fill = new SolidColorBrush(Utils.PositiveButtonColor),
            RadiusX = Utils.BorderRadius,
            RadiusY = Utils.BorderRadius,
            Effect = Utils.dropShadowRectangle
        };
        Container.Children.Add(BgRectangle);

        TextB = new TextBlock
        {
            Text = "Saved!",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = new SolidColorBrush(Colors.White),
            FontSize = Utils.TitleFontSize,
            FontWeight = FontWeights.Bold,
            Effect = Utils.dropShadowText
        };
        Container.Children.Add(TextB);

        Content = Container;

        RunAnimations();
    }

    private async void RunAnimations()
    {
        ThicknessAnimation comeDown = new ThicknessAnimation
        {
            From = new Thickness(0, -TopMargin, 0, 0),
            To = new Thickness(0, -TopMargin + 100, 0, 0),
            Duration = new Duration(TimeSpan.FromSeconds(0.5)),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        ThicknessAnimation goUp = new ThicknessAnimation
        {
            From = new Thickness(0, -TopMargin + 100, 0, 0),
            To = new Thickness(0, -TopMargin, 0, 0),
            Duration = new Duration(TimeSpan.FromSeconds(0.5)),
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
        };

        goUp.Completed += (s, e) =>
        {
            Parent.Children.Remove(this);
        };

        comeDown.Completed += async (s, e) =>
        {
            await Task.Delay(500);
            Container.BeginAnimation(MarginProperty, goUp);
        };

        Container.BeginAnimation(MarginProperty, comeDown);
    }
}