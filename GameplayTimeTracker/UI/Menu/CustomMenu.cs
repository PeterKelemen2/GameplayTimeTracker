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

    public CustomMenu()
    {
        // Find the Root and MainGrid panels
        Panel RootPanel = (Panel)mainWindow.FindName("Root");
        Panel ContentPanel = (Panel)RootPanel.FindName("MainGrid");

        // Create a BlurEffect and apply it to MainGrid
        BlurEffect blurEffect = new BlurEffect
        {
            Radius = 0 // Start with no blur
        };
        ContentPanel.Effect = blurEffect;

        // Animate the BlurEffect's Radius
        blurEffect.BeginAnimation(BlurEffect.RadiusProperty, AppAnimations.BgBlurInEffectAnim);

        // Create the menu background
        Grid ContainerGrid = new Grid
        {
            Width = mainWindow.Width,
            Height = mainWindow.Height,
        };

        Rectangle BgRectangle = new Rectangle
        {
            Width = ContainerGrid.Width,
            Height = ContainerGrid.Height,
            Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fcba03")), // Test color
            Opacity = 0.5,
        };
        ContainerGrid.Children.Add(BgRectangle);

        // Add the menu background to the Root panel
        RootPanel.Children.Add(ContainerGrid);

        // Animate scaling of the MainGrid (optional)
        ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, AppAnimations.ScaleUpAnim);
        ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, AppAnimations.ScaleUpAnim);
    }
}
