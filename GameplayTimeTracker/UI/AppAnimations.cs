using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace GameplayTimeTracker;

public class AppAnimations
{
    public static DoubleAnimation dragFadeInAnimation = new DoubleAnimation
    {
        From = 0,
        To = 1,
        Duration = new Duration(TimeSpan.FromSeconds(0.2)),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };

    public static DoubleAnimation dragFadeOutAnimation = new DoubleAnimation
    {
        From = 1,
        To = 0,
        Duration = new Duration(TimeSpan.FromSeconds(0.2)),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };
}