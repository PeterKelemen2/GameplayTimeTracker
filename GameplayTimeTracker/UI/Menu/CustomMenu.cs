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
    private Grid ContainerGrid;
    private Rectangle BgRectangle;
    private Grid MenuContentGrid;
    private Rectangle MenuContentBg;

    private BlurEffect BlurEffect;
    private Storyboard OpenMenuStoryboard = new Storyboard();
    private Storyboard CloseMenuStoryboard = new Storyboard();

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

    public void CreateStoryboards()
    {
        OpenMenuStoryboard = new Storyboard();

        // Create and configure the animations
        DoubleAnimation bgOpacityAnimation = new DoubleAnimation
        {
            From = 0,
            To = 0.3, // Set final opacity
            Duration = TimeSpan.FromSeconds(1),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(bgOpacityAnimation, BgRectangle);
        Storyboard.SetTargetProperty(bgOpacityAnimation, new PropertyPath(Rectangle.OpacityProperty));

        DoubleAnimation blurEffectAnimation = new DoubleAnimation
        {
            From = 0,
            To = 15, // Set desired blur radius
            Duration = TimeSpan.FromSeconds(1),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(blurEffectAnimation, BlurEffect);
        Storyboard.SetTargetProperty(blurEffectAnimation, new PropertyPath(BlurEffect.RadiusProperty));

        DoubleAnimation scaleXAnimation = new DoubleAnimation
        {
            From = 1.0,
            To = 1.07, // Set final scale X
            Duration = TimeSpan.FromSeconds(1),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(scaleXAnimation, ContentPanel.RenderTransform);
        Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath(ScaleTransform.ScaleXProperty));

        DoubleAnimation scaleYAnimation = new DoubleAnimation
        {
            From = 1.0,
            To = 1.07, // Set final scale Y
            Duration = TimeSpan.FromSeconds(1),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(scaleYAnimation, ContentPanel.RenderTransform);
        Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath(ScaleTransform.ScaleYProperty));

        DoubleAnimation translateYAnimation = new DoubleAnimation
        {
            From = 800,
            To = 0, // Set final Y position
            Duration = TimeSpan.FromSeconds(1),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(translateYAnimation, MenuContentGrid.RenderTransform);
        Storyboard.SetTargetProperty(translateYAnimation, new PropertyPath(TranslateTransform.YProperty));

        // Add the animations to the storyboard
        // OpenMenuStoryboard.Children.Add(bgOpacityAnimation);
        OpenMenuStoryboard.Children.Add(blurEffectAnimation);
        OpenMenuStoryboard.Children.Add(scaleXAnimation);
        OpenMenuStoryboard.Children.Add(scaleYAnimation);
        OpenMenuStoryboard.Children.Add(translateYAnimation);
    }


    public CustomMenu(double width = 300, double height = 400)
    {
        RootPanel = (Panel)mainWindow.FindName("Root");
        ContentPanel = (Panel)RootPanel.FindName("MainGrid");
        ContentPanel.SizeChanged += ContentGrid_SizeChanged;
        // var scaleTransform = new ScaleTransform();
        // ContentPanel.RenderTransform = scaleTransform;

        // BlurEffect = new BlurEffect();
        // ContentPanel.Effect = BlurEffect;

        ContainerGrid = new Grid
        {
            Width = mainWindow.ActualWidth,
            Height = mainWindow.ActualHeight,
        };
        Console.WriteLine($"ContainerGrid.Height: {ContainerGrid.Height}");

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
        };
        ContainerGrid.Children.Add(MenuContentGrid);
        var translateTransform = new TranslateTransform(0,0);
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

        // CreateStoryboards();
        Open();
    }

    public void Open()
    {
        if (!IsOpen)
        {
            RootPanel.Children.Add(ContainerGrid);

            BgRectangle.BeginAnimation(OpacityProperty, AppAnimations.MenuBgOpacityIn);
            // BlurEffect.BeginAnimation(BlurEffect.RadiusProperty, AppAnimations.BgBlurInEffectAnim);
            // ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, AppAnimations.ScaleUpAnim);
            // ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, AppAnimations.ScaleUpAnim);
            MenuContentGrid.RenderTransform.BeginAnimation(TranslateTransform.YProperty, FlyInAnimation);

            // OpenMenuStoryboard.Begin();
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
            // BlurEffect.BeginAnimation(BlurEffect.RadiusProperty, AppAnimations.BgBlurOutEffectAnim);
            // ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, AppAnimations.ScaleDownAnim);
            // ContentPanel.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, AppAnimations.ScaleDownAnim);
            MenuContentGrid.RenderTransform.BeginAnimation(TranslateTransform.YProperty, FlyOutAnimation);

            // CloseMenuStoryboard.Begin();
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