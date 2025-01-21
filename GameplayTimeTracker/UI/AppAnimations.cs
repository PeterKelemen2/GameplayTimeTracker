using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace GameplayTimeTracker;

public class AppAnimations
{
    public static double scaleAnimDuration = 1;
    public static double deleteAnimDuration = 2;

    // private static double[] scaleAnimValues = { 1.0, 1.07 };
    private static double scaleAnimValue = 1.07;
    private static double blurAnimValue = 15;
    private static double menuBgOpacity = 0.6;

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
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };

    public static DoubleAnimation BgBlurInEffectAnim = new DoubleAnimation
    {
        From = 0,
        To = blurAnimValue,
        Duration = TimeSpan.FromSeconds(scaleAnimDuration),
        // AutoReverse = false,
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };

    public static DoubleAnimation BgBlurOutEffectAnim = new DoubleAnimation
    {
        From = blurAnimValue,
        To = 0,
        Duration = TimeSpan.FromSeconds(scaleAnimDuration),
        // AutoReverse = false,
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };

    public static DoubleAnimation MenuBgOpacityIn = new DoubleAnimation
    {
        From = 0.0,
        To = menuBgOpacity,
        Duration = TimeSpan.FromSeconds(scaleAnimDuration),
        AutoReverse = false,
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };

    public static DoubleAnimation MenuBgOpacityOut = new DoubleAnimation
    {
        From = menuBgOpacity,
        To = 0.0,
        Duration = TimeSpan.FromSeconds(scaleAnimDuration),
        AutoReverse = false,
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };

    public static DoubleAnimation DeleteScaleDownAnimation = new DoubleAnimation
    {
        From = 1,
        To = 0,
        Duration = TimeSpan.FromSeconds(deleteAnimDuration),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };
    
    public static DoubleAnimation DeleteOpacityAnimation = new DoubleAnimation
    {
        From = 1,
        To = 0,
        Duration = TimeSpan.FromSeconds(deleteAnimDuration),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };
    
    public static DoubleAnimation DeleteSizeDownAnimation = new DoubleAnimation
    {
        // From = 1,
        To = 0.01,
        Duration = TimeSpan.FromSeconds(deleteAnimDuration),
        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
    };
    
    public static ThicknessAnimation DeleteThicknessAnimation = new ThicknessAnimation
    {
        // From = new Thickness(10, 10, 10, 10),
        To = new Thickness(0, 0, 0, 0),
        Duration = TimeSpan.FromSeconds(deleteAnimDuration),
        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
    };
}