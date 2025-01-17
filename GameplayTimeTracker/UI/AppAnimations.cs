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

    private static double scaleAnimDuration = 2;
    private static double[] scaleAnimValues = { 1.0, 1.07 };

    public static DoubleAnimation dragFadeOutAnimation = new DoubleAnimation
    {
        From = 1,
        To = 0,
        Duration = new Duration(TimeSpan.FromSeconds(2)),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };

    public static DoubleAnimation ScaleUpAnim = new DoubleAnimation
    {
        From = scaleAnimValues[0],
        To = scaleAnimValues[1],
        Duration = new Duration(TimeSpan.FromSeconds(scaleAnimDuration)),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };

    public static DoubleAnimation ScaleDownAnim = new DoubleAnimation
    {
        From = scaleAnimValues[1],
        To = scaleAnimValues[0],
        Duration = new Duration(TimeSpan.FromSeconds(scaleAnimDuration)),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
    };
}