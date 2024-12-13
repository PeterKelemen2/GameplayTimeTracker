using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Gtk;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using Grid = System.Windows.Controls.Grid;
using Style = System.Windows.Style;

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
    public Button OpenFolderButton { get; set; }

    public TextBlock ArgsEditBlock { get; set; }
    public TextBox ArgsEditBox { get; set; }

    public Button ChangeIconButton { get; set; }
    public Button SaveButton { get; set; }

    public Double[] colMargins = { 20, 210, 430}; // Left margin
    public Double[] rowMargins = { 15, 35, 80, 100 }; // Top margin


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
            RadiusY = Utils.BorderRadius,
            Effect = Utils.dropShadowRectangle
        };
        // TranslateTransform bgTransform = new TranslateTransform();
        // Container.RenderTransform = bgTransform;
        Container.Children.Add(BgRectangle);

        TitleEditBlock = SampleBlock("Title", 0, 0, indent);
        TitleEditBox = SampleBox(parent.GameName, 0, 1);
        Container.Children.Add(TitleEditBlock);
        Container.Children.Add(TitleEditBox);

        PlaytimeEditBlock = SampleBlock("Playtime", 1, 0, indent);
        PlaytimeEditBox = SampleBox($"{parent.hTotal}h {parent.mTotal}m", 1, 1);
        Container.Children.Add(PlaytimeEditBlock);
        Container.Children.Add(PlaytimeEditBox);

        PathEditBlock = SampleBlock("Path", 0, 2, indent);
        PathEditBox = SampleBox(parent.ExePath, 0, 3);
        Container.Children.Add(PathEditBlock);
        Container.Children.Add(PathEditBox);

        ArgsEditBlock = SampleBlock("Arguments", 1, 2, indent);
        ArgsEditBox = SampleBox(parent.ShortcutArgs, 1, 3);
        Container.Children.Add(ArgsEditBlock);
        Container.Children.Add(ArgsEditBox);

        BrowseExeButton = SampleButton("New exe", col: 2, row: 1, topMarginModifier: 10);
        // BrowseExeButton.Click += parent.UpdateExe;
        OpenFolderButton = SampleButton("Open Folder", col: 2, row: 3, topMarginModifier: -10);

        ChangeIconButton = SampleButton("Change Icon", col: 0, row: 1, topMarginModifier: 10, fromRight: true);
        SaveButton = SampleButton("Save", col: 0, row: 3, topMarginModifier: -10, fromRight: true);
        SaveButton.Background = new SolidColorBrush(Colors.LightGreen);
        Container.Children.Add(BrowseExeButton);
        Container.Children.Add(OpenFolderButton);
        Container.Children.Add(ChangeIconButton);
        Container.Children.Add(SaveButton);

        Content = Container;
    }

    private TextBox SampleBox(string text = "", int col = 0, int row = 0)
    {
        TextBox sample = new TextBox
        {
            Text = text,
            Width = 180,
            // MaxHeight = 0,
            Height = Utils.TextBoxHeight,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            TextAlignment = TextAlignment.Left,
            HorizontalContentAlignment = HorizontalAlignment.Left, // Center-align content horizontally
            VerticalContentAlignment = VerticalAlignment.Center,
            Effect = Utils.dropShadowIcon,
            Margin = new Thickness(colMargins[col], rowMargins[row], 0, 0)
        };
        sample.Style = (Style)Application.Current.FindResource("RoundedTextBox");

        return sample;
    }

    private TextBlock SampleBlock(string text = "", int col = 0, int row = 0, double indent = 0, bool isBold = true)
    {
        var sampleTextBlock = new TextBlock
        {
            Text = text,
            FontWeight = isBold ? FontWeights.Bold : FontWeights.Regular,
            FontSize = Utils.TextFontSize,
            Foreground = new SolidColorBrush(Utils.FontColor),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin =
                new Thickness(colMargins[col] + indent, rowMargins[row], 0, 0),
            Effect = Utils.dropShadowText,
        };

        return sampleTextBlock;
    }

    private Button SampleButton(string text = "", double width = 120, double height = Utils.TextBoxHeight, int col = 0,
        int row = 0, double leftMarginModifier = 0, double topMarginModifier = 0, bool fromRight = false)
    {
        var newButton = new Button
        {
            Style = (Style)Application.Current.FindResource("RoundedButton"),
            Content = text,
            Height = height,
            Width = width,
            HorizontalAlignment = fromRight ? HorizontalAlignment.Right : HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(
                fromRight ? 0 : colMargins[col] + leftMarginModifier,
                rowMargins[row] + topMarginModifier,
                fromRight ? colMargins[col] + leftMarginModifier : 0,
                0),
            Effect = Utils.dropShadowIcon
        };

        return newButton;
    }
}