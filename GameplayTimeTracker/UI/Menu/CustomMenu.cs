using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace GameplayTimeTracker.Menu;

public class CustomMenu : UserControl
{
    private Window mainWindow = Application.Current.MainWindow;
    private Panel RootPanel;
    private Panel ContentPanel;
    private Grid ContainerGrid;
    private Rectangle BgRectangle;

    BlurEffect blurEffect = new BlurEffect { Radius = 0 };

    public CustomMenu()
    {
        RootPanel = (Panel)mainWindow.FindName("Root");
        ContentPanel = (Panel)RootPanel.FindName("MainGrid");

        ContentPanel.Effect = blurEffect;

        ContainerGrid = new Grid
        {
            Width = mainWindow.Width,
            Height = mainWindow.Height,
        };

        BgRectangle = new Rectangle
        {
            Width = ContainerGrid.Width,
            Height = ContainerGrid.Height,
            Fill = new SolidColorBrush(Colors.Black),
            Opacity = 0.5,
        };
        ContainerGrid.Children.Add(BgRectangle);
    }

    public void Open()
    {
        RootPanel.Children.Add(ContainerGrid);

        blurEffect.BeginAnimation(BlurEffect.RadiusProperty, AppAnimations.BgBlurInEffectAnim);
        ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, AppAnimations.ScaleUpAnim);
        ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, AppAnimations.ScaleUpAnim);
    }

    public void Close()
    {
        if (RootPanel.Children.Contains(ContainerGrid))
        {
            AppAnimations.ScaleDownAnim.Completed += (s, a) => { RootPanel.Children.Remove(ContainerGrid); };
        }

        blurEffect.BeginAnimation(BlurEffect.RadiusProperty, AppAnimations.BgBlurOutEffectAnim);
        ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, AppAnimations.ScaleDownAnim);
        ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, AppAnimations.ScaleDownAnim);
    }
}