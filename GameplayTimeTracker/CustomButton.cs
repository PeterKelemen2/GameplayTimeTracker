using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameplayTimeTracker;

public class CustomButton : UserControl
{
    private bool isDisabled = false;
    private Rectangle ButtonBase;

    public CustomButton(Grid parent, double width, double height, double borderRadius, string text = "",
        double fontSize = 16, bool isBold = false, bool isDisabled = false)
    {
        ButtonBase = new();
        BTextBlock = new();
        ButtonParent = parent;
        ButtonWidth = width;
        ButtonHeight = height;
        BorderRadius = borderRadius;
        ButtonText = text;
        ButtonFontSize = fontSize;
        IsBold = isBold;
        IsButtonDisabled = isDisabled;

        Grid = new Grid();
        Grid.Width = ButtonWidth;
        Grid.Height = ButtonHeight;

        ButtonBase = new Rectangle
        {
            Width = width,
            Height = height,
            RadiusX = borderRadius,
            RadiusY = borderRadius,
            Fill = new SolidColorBrush(Utils.ButtonColor),
            Effect = Utils.dropShadowIcon
        };
        Grid.Children.Add(ButtonBase);

        BTextBlock = new TextBlock
        {
            Text = ButtonText,
            Foreground = new SolidColorBrush(Colors.Black),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = ButtonFontSize,
            FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal,
            Margin = new Thickness(0, 0, 0, FontSize / 4),
        };
        Grid.Children.Add(BTextBlock);

        ButtonParent.Children.Add(Grid);
    }

    public bool IsButtonDisabled
    {
        get => GetButtonState();
        set => SetButtonState(value);
    }

    private void SetButtonState(bool value)
    {
        ButtonBase.Fill = value ? new SolidColorBrush(Colors.DarkGray) : new SolidColorBrush(Utils.ButtonColor);
        BTextBlock.Foreground = value ? new SolidColorBrush(Colors.DimGray) : new SolidColorBrush(Colors.Black);
        isDisabled = value;
    }

    private bool GetButtonState()
    {
        return isDisabled;
    }

    public double ButtonFontSize
    {
        get => GetFontSize();
        set => SetFontSize(value);
    }

    private void SetFontSize(double value)
    {
        BTextBlock.FontSize = value;
    }

    private double GetFontSize()
    {
        return BTextBlock.FontSize;
    }

    public Thickness Margin
    {
        get => GetGridMargin();
        set => SetGridMargin(value);
    }

    private Thickness GetGridMargin()
    {
        return new Thickness(Grid.Margin.Left, Grid.Margin.Top, Grid.Margin.Right, Grid.Margin.Bottom);
    }


    private void SetGridMargin(Thickness value)
    {
        Grid.Margin = value;
    }


    public TextBlock BTextBlock { get; set; }

    public bool IsBold { get; set; }

    public Grid Grid { get; set; }

    public Grid ButtonParent { get; set; }

    public string ButtonText { get; set; }

    public double ButtonHeight { get; set; }

    public double ButtonWidth { get; set; }

    public double BorderRadius { get; set; }
}