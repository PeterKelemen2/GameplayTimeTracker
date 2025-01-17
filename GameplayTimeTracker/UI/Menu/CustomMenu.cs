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
    private Grid MenuContentGrid;
    private Rectangle MenuContentBg;

    BlurEffect blurEffect = new BlurEffect { Radius = 0, RenderingBias = RenderingBias.Quality };

    public CustomMenu(double width = 300, double height = 400)
    {
        RootPanel = (Panel)mainWindow.FindName("Root");
        ContentPanel = (Panel)RootPanel.FindName("MainGrid");
        ContentPanel.SizeChanged += ContentGrid_SizeChanged;

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
        };
        BgRectangle.MouseDown += Close_Click;
        ContainerGrid.Children.Add(BgRectangle);

        MenuContentGrid = new Grid
        {
            Width = width,
            Height = height,
        };
        ContainerGrid.Children.Add(MenuContentGrid);

        MenuContentBg = new Rectangle
        {
            Width = width,
            Height = height,
            RadiusX = Common.BorderRadius,
            RadiusY = Common.BorderRadius,
            Fill = new SolidColorBrush(AppColors.Background),
            Effect = AppEffects.DropShadowRectangle
        };
        MenuContentGrid.Children.Add(MenuContentBg);
    }

    public void Open()
    {
        RootPanel.Children.Add(ContainerGrid);
        BgRectangle.BeginAnimation(OpacityProperty, AppAnimations.MenuBgOpacityIn);
        blurEffect.BeginAnimation(BlurEffect.RadiusProperty, AppAnimations.BgBlurInEffectAnim);
        ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, AppAnimations.ScaleUpAnim);
        ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, AppAnimations.ScaleUpAnim);
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    public void Close()
    {
        if (RootPanel.Children.Contains(ContainerGrid))
        {
            AppAnimations.ScaleDownAnim.Completed += (s, a) => { RootPanel.Children.Remove(ContainerGrid); };
        }

        BgRectangle.BeginAnimation(OpacityProperty, AppAnimations.MenuBgOpacityOut);
        blurEffect.BeginAnimation(BlurEffect.RadiusProperty, AppAnimations.BgBlurOutEffectAnim);
        ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, AppAnimations.ScaleDownAnim);
        ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, AppAnimations.ScaleDownAnim);
    }

    private void ContentGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var panel = sender as Panel;
        if (panel != null)
        {
            ContainerGrid.Width = panel.ActualWidth;
            ContainerGrid.Height = panel.ActualHeight;
            BgRectangle.Width = panel.ActualWidth;
            BgRectangle.Height = panel.ActualHeight;
        }
    }
}