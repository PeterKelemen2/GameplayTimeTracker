using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace GameplayTimeTracker;

public class AppAnimations
{
    private static double scaleAnimDuration = 2;
    // private static double[] scaleAnimValues = { 1.0, 1.07 };
    private static double scaleAnimValue = 1.07;
    private static double blurAnimValue = 15;
    // private static double[] blurAnimValues = { 0, 30 };
    
    
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
        Duration = new Duration(TimeSpan.FromSeconds(2)),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };

    public static DoubleAnimation ScaleUpAnim = new DoubleAnimation
    {
        From = 1.0,
        To = scaleAnimValue,
        Duration = new Duration(TimeSpan.FromSeconds(scaleAnimDuration)),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };

    public static DoubleAnimation ScaleDownAnim = new DoubleAnimation
    {
        From = scaleAnimValue,
        To = 1.0,
        Duration = new Duration(TimeSpan.FromSeconds(scaleAnimDuration)),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
    };

    public static DoubleAnimation BgBlurInEffectAnim = new DoubleAnimation
    {
        From = 0,
        To = blurAnimValue,
        Duration = TimeSpan.FromSeconds(scaleAnimDuration),
        AutoReverse = false,
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };
    
    public static DoubleAnimation BgBlurOutEffectAnim = new DoubleAnimation
    {
        From = blurAnimValue,
        To = 0,
        Duration = TimeSpan.FromSeconds(scaleAnimDuration),
        AutoReverse = false,
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
    };
}