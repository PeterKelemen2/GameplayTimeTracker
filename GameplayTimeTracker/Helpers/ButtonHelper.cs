using System.Drawing;
using System.Windows;

namespace GameplayTimeTracker.Helpers;

public static class ButtonHelper
{
    public static readonly DependencyProperty BaseBackgroundProperty =
        DependencyProperty.RegisterAttached(
            "BaseBackground",
            typeof(Brush),
            typeof(ButtonHelper),
            new PropertyMetadata(Brushes.Transparent));

    public static void SetBaseBackground(UIElement element, Brush value) => element.SetValue(BaseBackgroundProperty, value);
    public static Brush GetBaseBackground(UIElement element) => (Brush)element.GetValue(BaseBackgroundProperty);
}