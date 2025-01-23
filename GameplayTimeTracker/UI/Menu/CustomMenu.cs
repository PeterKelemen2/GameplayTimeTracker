using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using GameplayTimeTracker.Settings;

namespace GameplayTimeTracker.Menu;

public class CustomMenu : UserControl
{
    private bool IsOpen = false;
    private Window mainWindow = Application.Current.MainWindow;
    private Panel RootPanel;
    private Panel ContentPanel;
    public Grid ContainerGrid;
    private Rectangle BgRectangle;
    public Panel MenuContentPanel;
    private Border MenuContentBorder;
    public BlurEffect BlurEffect;
    public bool PerformanceMode = true;

    public CustomMenu(AppSettings appSettings, double width = 300, bool performanceMode = true)
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
            Opacity = 0,
        };
        BindingHelper.SetColorBinding(BgRectangle, Shape.FillProperty, appSettings, "Shadow");
        BgRectangle.MouseDown += (_, _) => { Close(); };
        ContainerGrid.Children.Add(BgRectangle);

        MenuContentPanel = new StackPanel
        {
            Width = width,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
        };
        MenuContentBorder = new Border
        {
            // Background = ColorHelper.CreateLinGradBrushVer(AppColors.CardColor1, AppColors.CardColor2),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            CornerRadius = new CornerRadius(15),
            Child = MenuContentPanel,
            Effect = AppEffects.DropShadowRectangle
        };
        ContainerGrid.Children.Add(MenuContentBorder);
        BindingHelper.SetGradientColorBinding(MenuContentBorder, BackgroundProperty, appSettings, "Card 1",
            "Card 2", horizontal: false);

        var translateTransform = new TranslateTransform(0, 0);
        MenuContentBorder.RenderTransform = translateTransform;
    }

    public void Open()
    {
        if (!IsOpen)
        {
            RootPanel.Children.Add(ContainerGrid);

            AppAnimations.FlyInAnimation.From = mainWindow.Height;
            AppAnimations.FlyOutAnimation.To = -(mainWindow.Height * 0.5 + MenuContentPanel.ActualHeight * 0.5);

            BgRectangle.BeginAnimation(OpacityProperty, AppAnimations.MenuBgOpacityIn);
            MenuContentBorder.RenderTransform.BeginAnimation(TranslateTransform.YProperty,
                AppAnimations.FlyInAnimation);
            MenuContentBorder.BeginAnimation(OpacityProperty, AppAnimations.FadeIn);
            MenuContentPanel.BeginAnimation(OpacityProperty, AppAnimations.FadeIn);

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
            MenuContentBorder.RenderTransform.BeginAnimation(TranslateTransform.YProperty,
                AppAnimations.FlyOutAnimation);
            MenuContentBorder.BeginAnimation(OpacityProperty, AppAnimations.FadeOut);
            MenuContentPanel.BeginAnimation(OpacityProperty, AppAnimations.FadeOut);

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
            AppAnimations.FlyOutAnimation.To = -(mainWindow.Height * 0.5 + MenuContentPanel.ActualHeight * 0.5);
        }
    }
}