using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameplayTimeTracker;

public class CustomButton : UserControl
{
    private bool isDisabled = false;
    private Rectangle ButtonBase;
    private string ButtonImagePath;
    public Image ButtonImage = new Image();

    public CustomButton(Grid parent, double width, double height, double borderRadius, string text = "",
        double fontSize = 16, bool isBold = false, string buttonImagePath = "", bool isDisabled = false)
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
        ButtonImagePath = buttonImagePath;
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
        if (!BTextBlock.Text.Equals(""))
        {
            Grid.Children.Add(BTextBlock);
        }

        if (!string.Equals(ButtonImagePath, ""))
        {
            if (File.Exists(ButtonImagePath))
            {
                ButtonImage.Source = new BitmapImage(new Uri(ButtonImagePath, UriKind.RelativeOrAbsolute));
                ButtonImage.Width = (int)ButtonHeight / 2;
                ButtonImage.Height = (int)ButtonHeight / 2;
                ButtonImage.HorizontalAlignment = HorizontalAlignment.Center;

                if (!BTextBlock.Text.Equals(""))
                {
                    double padding = 5;
                    Grid combinedGrid = new();
                    combinedGrid.HorizontalAlignment = HorizontalAlignment.Center;
                    ButtonImage.HorizontalAlignment = HorizontalAlignment.Left;
                    ButtonImage.Margin = new Thickness(0, 0, ButtonImage.Width + padding, 0);
                    BTextBlock.Margin = new Thickness(ButtonImage.Width + padding, 0, 0, 0);
                    if (Grid.Children.Contains(BTextBlock))
                    {
                        Grid.Children.Remove(BTextBlock);
                    }

                    combinedGrid.Children.Add(BTextBlock);
                    combinedGrid.Children.Add(ButtonImage);
                    Grid.Children.Add(combinedGrid);
                }
                else
                {
                    Grid.Children.Add(ButtonImage);
                }
                // RenderOptions.SetBitmapScalingMode(ButtonImage, BitmapScalingMode.HighQuality);
            }
        }

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