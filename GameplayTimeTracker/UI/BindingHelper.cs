using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using GameplayTimeTracker.Settings;

namespace GameplayTimeTracker;

public class BindingHelper
{
    public static void SetColorBinding(UIElement element, DependencyProperty property, string colorName)
    {
        Binding newBinding = new Binding
        {
            Source = Common.Settings.CurrentTheme.Colors,
            Path = new PropertyPath($"[{colorName}]"),
            Converter = new ColorToBrushConverter(),
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(element, property, newBinding);
    }

    public static void SetGradientColorBinding(UIElement element, DependencyProperty property, string c1, string c2,
        bool horizontal = false)
    {
        MultiBinding fillMultiBinding = new MultiBinding
        {
            Converter = horizontal
                ? new HorizontalGradientBrushConverter()
                : new VerticalGradientBrushConverter(),
            Mode = BindingMode.OneWay
        };

        // Bind first color
        fillMultiBinding.Bindings.Add(new Binding
        {
            Source = Common.Settings.CurrentTheme.Colors,
            Path = new PropertyPath($"[{c1}]")
        });

        // Bind second color
        fillMultiBinding.Bindings.Add(new Binding
        {
            Source = Common.Settings.CurrentTheme.Colors,
            Path = new PropertyPath($"[{c2}]")
        });

        // Apply the multi-binding to the rectangle's Fill property
        BindingOperations.SetBinding(element, property, fillMultiBinding);
    }

    public static void SetCustomButtonColorBinding(CustomButton button, AppSettings settings, string colorName)
    {
        Binding buttonFillBinding = new Binding
        {
            Source = Common.Settings.CurrentTheme.Colors,
            Path = new PropertyPath($"[{colorName}]"),
            Converter = new ColorToBrushConverter(),
            Mode = BindingMode.OneWay
        };
        BindingOperations.SetBinding(button.ButtonBase, Shape.FillProperty, buttonFillBinding);
    }
}