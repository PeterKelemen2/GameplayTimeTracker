using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using GameplayTimeTracker.Settings;
using MonoMac.CoreMedia;

namespace GameplayTimeTracker.Menu;

public class CustomMenu : UserControl
{
    public bool IsOpen = false;
    public bool ToScale;

    private Window mainWindow = Application.Current.MainWindow;
    private Panel RootPanel;
    public Panel ContentPanel;
    public Grid ContainerGrid;
    private Rectangle BgRectangle;
    public Panel MenuContentPanel;
    private Border MenuContentBorder;
    public BlurEffect BlurEffect;
    public AppSettings Settings;

    public CustomMenu(double width = 300, bool toScale = true)
    {
        ToScale = toScale;
        RootPanel = (Panel)mainWindow.FindName("Root");
        ContentPanel = (Panel)mainWindow.FindName("MainGrid");
        RootPanel.SizeChanged += ContentGrid_SizeChanged;
        // Settings = appSettings;

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
        BindingHelper.SetColorBinding(BgRectangle, Shape.FillProperty, "Shadow");
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
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            CornerRadius = new CornerRadius(15),
            Child = MenuContentPanel,
            Effect = AppEffects.DropShadowRectangle
        };
        ContainerGrid.Children.Add(MenuContentBorder);
        BindingHelper.SetGradientColorBinding(MenuContentBorder, BackgroundProperty, "Card 1", "Card 2",
            horizontal: false);

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

            if (!Common.Settings.PerformanceMode)
            {
                if (ToScale)
                {
                    BlurEffect.BeginAnimation(BlurEffect.RadiusProperty, AppAnimations.BgBlurInEffectAnim);
                    ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty,
                        AppAnimations.ScaleUpAnim);
                    ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty,
                        AppAnimations.ScaleUpAnim);
                }
                else
                {
                    BlurEffect.Radius = AppAnimations.blurAnimValue;
                }
            }
            else
            {
                BlurEffect = new BlurEffect { Radius = 0 };
                ContentPanel.Effect = BlurEffect;
                ContentPanel.RenderTransform = new ScaleTransform(1, 1);
                ContentPanel.RenderTransformOrigin = new Point(0.5, 0.5);
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

            if (!Common.Settings.PerformanceMode)
            {
                Console.WriteLine($"{GetType().Name} to scale: {ToScale}");
                if (ToScale)
                {
                    AppAnimations.BgBlurOutEffectAnim.To = 0.0;
                    ContentPanel.Effect.BeginAnimation(BlurEffect.RadiusProperty, AppAnimations.BgBlurOutEffectAnim);

                    ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty,
                        AppAnimations.ScaleDownAnim);
                    ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty,
                        AppAnimations.ScaleDownAnim);
                }
                else
                {
                    AppAnimations.BgBlurOutEffectAnim.To = AppAnimations.blurAnimValue;
                    BlurEffect.BeginAnimation(BlurEffect.RadiusProperty, AppAnimations.BgBlurOutEffectAnim);
                }
            }
            else
            {
                BlurEffect = new BlurEffect { Radius = 0 };
                ContentPanel.Effect = BlurEffect;
                ContentPanel.RenderTransform = new ScaleTransform(1, 1);
                ContentPanel.RenderTransformOrigin = new Point(0.5, 0.5);
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