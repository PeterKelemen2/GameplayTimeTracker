using System.Windows.Media;
using System.Windows.Media.Effects;

namespace GameplayTimeTracker;

public static class AppEffects
{
    public static BlurEffect fakeShadow = new BlurEffect
    {
        Radius = 8,
    };

    public static BlurEffect blurEffect = new BlurEffect
    {
        Radius = 10,
        RenderingBias = RenderingBias.Performance
    };

    public static BlurEffect bigBlurEffect = new BlurEffect
    {
        Radius = 30,
        RenderingBias = RenderingBias.Performance
    };

    public static OuterGlowBitmapEffect outerGlowEffect = new OuterGlowBitmapEffect
    {
        GlowSize = 10
    };

    public static DropShadowEffect dropShadowText = new DropShadowEffect
    {
        BlurRadius = 8,
        ShadowDepth = 0,
        Color = Colors.Black,
        Opacity = 1,
        Direction = 200,
    };

    public static DropShadowEffect dropShadowGameIcon = new DropShadowEffect
    {
        BlurRadius = 0,
        ShadowDepth = 10,
        Color = Colors.Black,
        Opacity = 0.4,
        Direction = -45,
    };

    public static DropShadowEffect dropShadowIcon = new DropShadowEffect
    {
        BlurRadius = 10,
        ShadowDepth = 0
    };

    public static DropShadowEffect dropShadowLightArea = new DropShadowEffect
    {
        BlurRadius = 7,
        ShadowDepth = 0
    };

    public static DropShadowEffect dropShadowRectangle = new DropShadowEffect
    {
        BlurRadius = 20,
        ShadowDepth = 0
    };
}