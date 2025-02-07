using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using Xceed.Wpf.AvalonDock.Controls;
using UserControl = System.Windows.Controls.UserControl;

namespace GameplayTimeTracker.UI.Menu.Content;

public class MenuContent : UserControl
{
    public ScrollViewer _scrollViewer { get; set; }
    public StackPanel _stackPanel { get; set; }
    private double viewHeight = 400;

    public MenuContent()
    {
        _scrollViewer = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Hidden, };
        _scrollViewer.Opacity = 0.0;

        _stackPanel = new StackPanel();
        _scrollViewer.Content = _stackPanel;
        FadeIn();
    }

    public void FadeOut()
    {
        _scrollViewer.BeginAnimation(OpacityProperty, AppAnimations.FadeOut);
    }

    public void FadeIn()
    {
        _scrollViewer.BeginAnimation(OpacityProperty, AppAnimations.FadeIn);
        // DoubleAnimation heightAnimation = new DoubleAnimation
        // {
        //     From = 0.01,
        //     To = viewHeight,
        //     Duration = new Duration(TimeSpan.FromMilliseconds(500)),
        //     EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        // };
        // _scrollViewer.BeginAnimation(HeightProperty, heightAnimation);
    }
}