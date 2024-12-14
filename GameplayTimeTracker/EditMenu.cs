using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
// using Gtk;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using Grid = System.Windows.Controls.Grid;
using Style = System.Windows.Style;

// using Gdk;

namespace GameplayTimeTracker;

public class EditMenu : UserControl
{
    private Tile Parent { get; set; }
    public Grid Container { get; set; }
    public Rectangle BgRectangle { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public string Title { get; set; }

    public bool IsOpen { get; set; }

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

    public Double[] colMargins = { 20, 210, 430 }; // Left margin
    public Double[] rowMargins = { 15, 35, 80, 100 }; // Top margin
    private double padding = 30;
    private double borderRadius = 8;
    private double indent = 5;
    private double animationTime = 0.5;
    private bool IsAnimating = false;

    public EditMenu(Tile parent)
    {
        Parent = parent;
        Width = parent.TileWidth - padding;
        Height = parent.TileHeight - Utils.dropShadowIcon.BlurRadius;
        Title = parent.GameName;
        IsOpen = false;
        Console.WriteLine($"Edit menu for {Title}: {Width}x{Height}");
    }

    private void CreateMenuContent()
    {
        Container = new Grid
        {
            Width = Width,
            Height = Height,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(-5, -Height, 0, 0)
        };

        BgRectangle = new Rectangle
        {
            Width = Width,
            Height = Height,
            Fill = Utils.createLinGradBrushHor(Utils.EditColor1, Utils.EditColor2),
            RadiusX = Utils.BorderRadius,
            RadiusY = Utils.BorderRadius,
            Effect = Utils.dropShadowIcon
        };
        Container.Children.Add(BgRectangle);

        TitleEditBlock = SampleBlock("Title", 0, 0, indent);
        TitleEditBox = SampleBox(Parent.GameName, 0, 1);
        TitleEditBox.KeyDown += editBox_KeyDown;
        Container.Children.Add(TitleEditBlock);
        Container.Children.Add(TitleEditBox);

        PlaytimeEditBlock = SampleBlock("Playtime", 1, 0, indent);
        PlaytimeEditBox = SampleBox($"{Parent.HTotal}h {Parent.MTotal}m", 1, 1);
        PlaytimeEditBox.KeyDown += editBox_KeyDown;
        Container.Children.Add(PlaytimeEditBlock);
        Container.Children.Add(PlaytimeEditBox);

        PathEditBlock = SampleBlock("Path", 0, 2, indent);
        PathEditBox = SampleBox(Parent.ExePath, 0, 3);
        PathEditBox.KeyDown += editBox_KeyDown;
        Container.Children.Add(PathEditBlock);
        Container.Children.Add(PathEditBox);

        ArgsEditBlock = SampleBlock("Arguments", 1, 2, indent);
        ArgsEditBox = SampleBox(Parent.ShortcutArgs, 1, 3);
        ArgsEditBox.KeyDown += editBox_KeyDown;
        Container.Children.Add(ArgsEditBlock);
        Container.Children.Add(ArgsEditBox);

        BrowseExeButton = SampleButton("New exe", col: 2, row: 1, topMarginModifier: 10);
        BrowseExeButton.Click += Parent.UpdateExe;
        OpenFolderButton = SampleButton("Open Folder", col: 2, row: 3, topMarginModifier: -10);
        OpenFolderButton.Click += Parent.OpenExeFolder;

        ChangeIconButton = SampleButton("Change Icon", col: 0, row: 1, topMarginModifier: 10, fromRight: true);
        ChangeIconButton.Click += Parent.UpdateIcons;
        SaveButton = SampleButton("Save", col: 0, row: 3, topMarginModifier: -10, fromRight: true);
        SaveButton.Background = new SolidColorBrush(Colors.LightGreen);
        SaveButton.Click += Parent.editSaveButton_Click;
        SaveButton.Click += ShowSaveIndicator;
        Container.Children.Add(BrowseExeButton);
        Container.Children.Add(OpenFolderButton);
        Container.Children.Add(ChangeIconButton);
        Container.Children.Add(SaveButton);

        Content = Container;
    }

    private void ShowSaveIndicator(object sender, RoutedEventArgs e)
    {
        SaveIndicator indicator = new SaveIndicator(Container, Parent.TileHeight);
        Container.Children.Add(indicator);
    }

    public void OpenMenu()
    {
        if (IsOpen || IsAnimating)
        {
            return;
        }

        IsAnimating = true;
        CreateMenuContent();
        ThicknessAnimation marginAnimation = new ThicknessAnimation
        {
            From = new Thickness(0, -Height, 0, 0),
            To = new Thickness(0, -Utils.dropShadowIcon.BlurRadius, 0, 0),
            Duration = new Duration(TimeSpan.FromSeconds(animationTime)),
            FillBehavior = FillBehavior.HoldEnd,
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };
        marginAnimation.Completed += (s, e) =>
        {
            IsAnimating = false;
            IsOpen = true;
        };

        Container.BeginAnimation(MarginProperty, marginAnimation);
    }

    public void CloseMenu()
    {
        if (!IsOpen || IsAnimating)
        {
            return;
        }

        IsAnimating = true;

        ThicknessAnimation marginAnimation = new ThicknessAnimation
        {
            From = new Thickness(0, -Utils.dropShadowIcon.BlurRadius, 0, 0),
            To = new Thickness(0, -Height, 0, 0),
            Duration = new Duration(TimeSpan.FromSeconds(animationTime)),
            FillBehavior = FillBehavior.HoldEnd,
            EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
        };

        marginAnimation.Completed += (s, e) =>
        {
            Container.Children.Clear();
            Container = null;
            IsAnimating = false;
            IsOpen = false;
        };

        Container.BeginAnimation(MarginProperty, marginAnimation);
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

    private void editBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            // Call your method here
            Parent.SaveEditedData();
            SaveIndicator indicator = new SaveIndicator(Container, Parent.TileHeight);
            Container.Children.Add(indicator);
            e.Handled = true; // Optional, prevents the beep sound
        }
    }
}