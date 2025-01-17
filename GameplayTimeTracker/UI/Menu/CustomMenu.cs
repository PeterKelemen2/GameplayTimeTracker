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
        Panel ParentPanel = (Panel)mainWindow.FindName("MainGrid");
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
            Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fcba03")) // Test color
        };
        ContainerGrid.Children.Add(BgRectangle);

        ParentPanel.Children.Add(ContainerGrid);
    }
}