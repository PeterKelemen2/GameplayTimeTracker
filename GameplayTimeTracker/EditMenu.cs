using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

// using Gdk;

namespace GameplayTimeTracker;

public class EditMenu : UserControl
{
    public Grid Container { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public string Title { get; set; }


    public TextBlock TitleEditBlock { get; set; }
    public TextBox TitleEditBox { get; set; }

    public TextBlock PlaytimeEditBlock { get; set; }
    public TextBox PlaytimeEditBox { get; set; }

    public TextBlock PathEditBlock { get; set; }
    public TextBox PathEditBox { get; set; }
    public Button BrowseExeButton { get; set; }
    
    public TextBlock ArgsEditBlock { get; set; }
    public TextBox ArgsEditBox { get; set; }

    public Button ChangeIconButton { get; set; }
    public Button SaveButton { get; set; }

    public EditMenu(Tile parent)
    {
        double padding = 30;
        double borderRadius = 8;
        Width = parent.TileWidth - padding;
        Height = parent.TileHeight;
        Title = parent.GameName;
        Console.WriteLine($"Edit menu for {Title}: {Width}x{Height}");
        Container = new Grid
        {
            Width = Width,
            Height = Height,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(-5, -borderRadius, 0, 0)
        };

        Rectangle bgRect = new Rectangle
        {
            Width = Width,
            Height = Height,
            Fill = new SolidColorBrush(Colors.White),
            RadiusX = Utils.BorderRadius,
            RadiusY = Utils.BorderRadius
        };
        TranslateTransform bgTransform = new TranslateTransform();
        Container.RenderTransform = bgTransform;
        Container.Children.Add(bgRect);

        Content = Container;
    }
}