using System.Windows;
using System.Windows.Data;

namespace GameplayTimeTracker;

public class BindingHelper
{
    public static void SetColorBinding(UIElement element, DependencyProperty property,
        ObservableDictionary<string, string> theme, string colorName)
    {
        Binding newBinding = new Binding
        {
            Source = theme,
            Path = new PropertyPath($"[{colorName}]"),
            Converter = new ColorDictionaryToBrushConverter(),
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(element, property, newBinding);
    }
}