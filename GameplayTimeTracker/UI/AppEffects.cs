using System.Windows.Media;
using System.Windows.Media.Effects;

namespace GameplayTimeTracker;

public static class AppEffects
{
    public static readonly BlurEffect fakeShadow = new BlurEffect
    {
        Radius = 8,
    };

    public static readonly BlurEffect blurEffect = new BlurEffect
    {
        Radius = 2,
        RenderingBias = RenderingBias.Performance
    };

    public static readonly BlurEffect bigBlurEffect = new BlurEffect
    {
        Radius = 30,
        RenderingBias = RenderingBias.Performance
    };
    
    public static readonly DropShadowEffect dropShadowText = new DropShadowEffect
    {
        BlurRadius = 8,
        ShadowDepth = 0,
        Color = Colors.Black,
        Opacity = 0.7,
        Direction = 200,
    };

    public static readonly DropShadowEffect DropShadowTitle = new DropShadowEffect
    {
        BlurRadius = 0,
        ShadowDepth = 3,
        Color = Colors.Black,
        Opacity = 1,
        Direction = -45,
    };

    public static readonly DropShadowEffect DropShadowGameIcon = new DropShadowEffect
    {
        BlurRadius = 0,
        ShadowDepth = 10,
        Color = Colors.Black,
        Opacity = 0.4,
        Direction = -45,
    };

    public static readonly DropShadowEffect DropOuterGlow = new DropShadowEffect
    {
        BlurRadius = 5,
        ShadowDepth = 0,
        Color = Colors.Black,
    };

    public static readonly DropShadowEffect DropShadowIcon = new DropShadowEffect
    {
        BlurRadius = 10,
        ShadowDepth = 0
    };

    public static readonly DropShadowEffect dropShadowLightArea = new DropShadowEffect
    {
        BlurRadius = 7,
        ShadowDepth = 0
    };

    public static readonly DropShadowEffect DropShadowRectangle = new DropShadowEffect
    {
        BlurRadius = 20,
        ShadowDepth = 0
    };
    
    public static readonly DropShadowEffect DropShadowMedium = new DropShadowEffect
    {
        BlurRadius = 15,
        ShadowDepth = 0
    };
}