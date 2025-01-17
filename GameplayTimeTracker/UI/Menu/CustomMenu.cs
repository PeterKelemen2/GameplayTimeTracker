using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameplayTimeTracker.Menu;

public class CustomMenu : UserControl
{
    private Window mainWindow = Application.Current.MainWindow;

    public CustomMenu()
    {
        Panel ParentPanel = (Panel)mainWindow.FindName("Root");
        Panel ContentPanel = (Panel)ParentPanel.FindName("MainGrid");
        
        Grid ContainerGrid = new Grid
        {
            Width = mainWindow.Width,
            Height = mainWindow.Height,
            // Background = new SolidColorBrush(AppColors.Footer)
        };

        Rectangle BgRectangle = new Rectangle
        {
            Width = ContainerGrid.Width,
            Height = ContainerGrid.Height,
            Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fcba03")), // Test color
            Opacity = 0.5,
        };
        ContainerGrid.Children.Add(BgRectangle);

        ParentPanel.Children.Add(ContainerGrid);
    
        ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, AppAnimations.ScaleUpAnim);
        ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, AppAnimations.ScaleUpAnim);
    }
}