using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
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
    private Rectangle MenuContentBg;

    private DoubleAnimation FlyInAnimation = new DoubleAnimation
    {
        To = 0, Duration = TimeSpan.FromSeconds(AppAnimations.scaleAnimDuration / 2),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };

    private DoubleAnimation FlyOutAnimation = new DoubleAnimation
    {
        From = 0, Duration = TimeSpan.FromSeconds(AppAnimations.scaleAnimDuration / 2),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };

    public CustomMenu(double width = 300, double height = 400)
    {
        RootPanel = (Panel)mainWindow.FindName("Root");
        RootPanel.SizeChanged += ContentGrid_SizeChanged;
        // ContentPanel = (Panel)RootPanel.FindName("MainGrid");
        // ContentPanel.SizeChanged += ContentGrid_SizeChanged;

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
        BgRectangle.MouseDown += Close_Click;
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
            Fill = AppColors.CreateLinGradBrushVer(AppColors.CardColor2, AppColors.CardColor1),
            Effect = AppEffects.DropShadowRectangle
        };

        MenuContentGrid.Children.Add(MenuContentBg);

        FlyInAnimation.From = ContainerGrid.Height + MenuContentGrid.Height / 2;
        FlyOutAnimation.To = -(ContainerGrid.Height - MenuContentGrid.Height / 2);
    }

    public void Open()
    {
        if (!IsOpen)
        {
            RootPanel.Children.Add(ContainerGrid);

            BgRectangle.BeginAnimation(OpacityProperty, AppAnimations.MenuBgOpacityIn);
            MenuContentGrid.RenderTransform.BeginAnimation(TranslateTransform.YProperty, FlyInAnimation);
            // These are way too slow:
            // BlurEffect.BeginAnimation(BlurEffect.RadiusProperty, AppAnimations.BgBlurInEffectAnim);
            // ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, AppAnimations.ScaleUpAnim);
            // ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, AppAnimations.ScaleUpAnim);

            IsOpen = true;
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
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
            MenuContentGrid.RenderTransform.BeginAnimation(TranslateTransform.YProperty, FlyOutAnimation);
            // These are way too slow:
            // BlurEffect.BeginAnimation(BlurEffect.RadiusProperty, AppAnimations.BgBlurOutEffectAnim);
            // ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, AppAnimations.ScaleDownAnim);
            // ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, AppAnimations.ScaleDownAnim);
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
            FlyInAnimation.From = ContainerGrid.Height;
            FlyOutAnimation.To = -(ContainerGrid.Height - MenuContentGrid.Height / 2);
        }
    }
}