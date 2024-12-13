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
    public Rectangle BgRectangle { get; set; }
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

    public Double[] colMargins = { 20, 100 }; // Left margin
    public Double[] rowMargins = { 15, 35, 120, 140 }; // Top margin


    public EditMenu(Tile parent)
    {
        double padding = 30;
        double borderRadius = 8;
        double indent = 5;
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

        BgRectangle = new Rectangle
        {
            Width = Width,
            Height = Height,
            Fill = Utils.createLinGradBrushHor(Utils.EditColor1, Utils.EditColor2),
            RadiusX = Utils.BorderRadius,
            RadiusY = Utils.BorderRadius
        };
        TranslateTransform bgTransform = new TranslateTransform();
        Container.RenderTransform = bgTransform;
        Container.Children.Add(BgRectangle);

        TextBlock sampleTextBlock = Utils.NewTextBlock();
        TextBox sampleTextBox = Utils.NewTextBoxEdit();

        // TitleEditBlock = new TextBlock
        // {
        //     Text = "Name",
        //     HorizontalAlignment = HorizontalAlignment.Left,
        //     VerticalAlignment = VerticalAlignment.Top,
        //     Margin = new Thickness(colMargins[0], rowMargins[0], 0, 0),
        // };
        TitleEditBlock = Utils.CloneTextBlock(sampleTextBlock);
        TitleEditBlock.Text = "Title";
        // TitleEditBlock.FontSize += 2;
        TitleEditBlock.HorizontalAlignment = HorizontalAlignment.Left;
        TitleEditBlock.VerticalAlignment = VerticalAlignment.Top;
        TitleEditBlock.Margin = new Thickness(colMargins[0] + indent, rowMargins[0], 0, 0);
        Container.Children.Add(TitleEditBlock);

        TitleEditBox = SampleBox();
        TitleEditBox.Margin = new Thickness(colMargins[0], rowMargins[1], 0, 0);
        Container.Children.Add(TitleEditBox);

        PlaytimeEditBox = SampleBox();
        PlaytimeEditBox.Text = "asd";
        PlaytimeEditBox.Margin = new Thickness(colMargins[1], rowMargins[1], 0, 0);
        Container.Children.Add(PlaytimeEditBox);

        Content = Container;
    }

    private TextBox SampleBox()
    {
        TextBox sample = new TextBox
        {
            Text = "Text",
            Width = 180,
            // MaxHeight = 0,
            Height = Utils.TextBoxHeight,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            TextAlignment = TextAlignment.Left,
            HorizontalContentAlignment = HorizontalAlignment.Left, // Center-align content horizontally
            VerticalContentAlignment = VerticalAlignment.Center,
            Effect = Utils.dropShadowIcon
        };
        sample.Style = (Style)Application.Current.FindResource("RoundedTextBox");

        return sample;
    }
}