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
    private bool IsOpen = false;
    private Window mainWindow = Application.Current.MainWindow;
    private Panel RootPanel;
    private Panel ContentPanel;
    public Grid ContainerGrid;
    private Rectangle BgRectangle;
    public Grid MenuContentGrid;
    public Rectangle MenuContentBg;
    public BlurEffect BlurEffect;
    public bool PerformanceMode = true;

    public CustomMenu(double width = 300, double height = 400, bool performanceMode = true)
    {
        RootPanel = (Panel)mainWindow.FindName("Root");
        ContentPanel = (Panel)mainWindow.FindName("MainGrid");
        RootPanel.SizeChanged += ContentGrid_SizeChanged;
        PerformanceMode = performanceMode;

        BlurEffect = new BlurEffect { Radius = 0 };
        ContentPanel.Effect = BlurEffect;

        ContainerGrid = new Grid
        {
            Width = RootPanel.ActualWidth,
            Height = RootPanel.ActualHeight,
        };

        BgRectangle = new Rectangle
        {
            Width = ContainerGrid.Width,
            Height = ContainerGrid.Height,
            Fill = new SolidColorBrush(Colors.Black),
            Opacity = 0,
        };
        BgRectangle.MouseDown += (_, _) => { Close(); };
        ContainerGrid.Children.Add(BgRectangle);

        MenuContentGrid = new Grid
        {
            Width = width,
            Height = height,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        ContainerGrid.Children.Add(MenuContentGrid);
        var translateTransform = new TranslateTransform(0, 0);
        MenuContentGrid.RenderTransform = translateTransform;

        MenuContentBg = new Rectangle
        {
            Width = width,
            Height = height,
            RadiusX = Common.BorderRadius,
            RadiusY = Common.BorderRadius,
            Fill = AppColors.CreateLinGradBrushVer(AppColors.CardColor1, AppColors.CardColor2),
            Effect = AppEffects.DropShadowRectangle
        };

        MenuContentGrid.Children.Add(MenuContentBg);

        // FlyInAnimation.From = ContainerGrid.Height + MenuContentGrid.Height * 0.5;
        // FlyOutAnimation.To = -(ContainerGrid.Height + MenuContentGrid.Height * 0.5);
        AppAnimations.FlyInAnimation.From = mainWindow.Height;
        AppAnimations.FlyOutAnimation.To = -(mainWindow.Height * 0.5 + MenuContentGrid.Height * 0.5);
    }

    public void Open()
    {
        if (!IsOpen)
        {
            RootPanel.Children.Add(ContainerGrid);

            BgRectangle.BeginAnimation(OpacityProperty, AppAnimations.MenuBgOpacityIn);
            MenuContentGrid.RenderTransform.BeginAnimation(TranslateTransform.YProperty, AppAnimations.FlyInAnimation);
            MenuContentGrid.BeginAnimation(OpacityProperty, AppAnimations.FadeIn);

            if (!PerformanceMode)
            {
                BlurEffect.BeginAnimation(BlurEffect.RadiusProperty, AppAnimations.BgBlurInEffectAnim);
                ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, AppAnimations.ScaleUpAnim);
                ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, AppAnimations.ScaleUpAnim);
            }

            IsOpen = true;
        }
    }

    public void Close()
    {
        if (IsOpen)
        {
            if (RootPanel.Children.Contains(ContainerGrid))
            {
                AppAnimations.MenuBgOpacityOut.Completed += (s, a) => { RootPanel.Children.Remove(ContainerGrid); };
                IsOpen = false;
            }

            BgRectangle.BeginAnimation(OpacityProperty, AppAnimations.MenuBgOpacityOut);
            MenuContentGrid.RenderTransform.BeginAnimation(TranslateTransform.YProperty, AppAnimations.FlyOutAnimation);
            MenuContentGrid.BeginAnimation(OpacityProperty, AppAnimations.FadeOut);

            if (!PerformanceMode)
            {
                BlurEffect.BeginAnimation(BlurEffect.RadiusProperty, AppAnimations.BgBlurOutEffectAnim);
                ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, AppAnimations.ScaleDownAnim);
                ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, AppAnimations.ScaleDownAnim);
            }
        }
    }

    private void ContentGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var panel = sender as Panel;
        if (panel != null && IsOpen)
        {
            ContainerGrid.Width = panel.ActualWidth;
            ContainerGrid.Height = panel.ActualHeight;
            BgRectangle.Width = panel.ActualWidth;
            BgRectangle.Height = panel.ActualHeight;
            AppAnimations.FlyInAnimation.From = mainWindow.Height;
            AppAnimations.FlyOutAnimation.To = -(mainWindow.Height * 0.5 + MenuContentGrid.Height * 0.5);
        }
    }
}