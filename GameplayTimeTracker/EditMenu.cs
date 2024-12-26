using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Gtk;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using Grid = System.Windows.Controls.Grid;
using Key = System.Windows.Input.Key;
using Style = System.Windows.Style;

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
    public bool ToSave { get; set; }

    public TextBlock TitleEditBlock { get; set; }
    public TextBox TitleEditBox { get; set; }

    public TextBlock PlaytimeEditBlock { get; set; }
    public TextBox PlaytimeEditBox { get; set; }

    public TextBlock PathEditBlock { get; set; }
    public TextBox PathEditBox { get; set; }
    public CustomButton BrowseExeButton { get; set; }
    public CustomButton OpenFolderButton { get; set; }

    public TextBlock ArgsEditBlock { get; set; }
    public TextBox ArgsEditBox { get; set; }

    public CustomButton ChangeIconButton { get; set; }
    public CustomButton SaveButton { get; set; }

    public Double[] colMargins = { 20, 210, 430 }; // Left margin
    public Double[] rowMargins = { 15, 35, 80, 100 }; // Top margin
    private double padding = 30;
    private double borderRadius = 8;
    private double indent = 5;
    private double animationTime = 0.5;
    private bool IsAnimating = false;
    private double FromValue;
    private double ToValue;
    private double bHeight = 35;
    private double bWidth = 140;
    


    public EditMenu(Tile parent)
    {
        Parent = parent;
        Width = parent.TileWidth - padding;
        Height = parent.TileHeight - Utils.dropShadowIcon.BlurRadius;
        Title = parent.GameName;
        IsOpen = false;
        FromValue = -Height;
        ToValue = -Utils.dropShadowIcon.BlurRadius;
    }

    private void CreateMenuContent()
    {
        Container = new Grid
        {
            Width = Width,
            Height = Height,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(-5, 0, 0, 0)
        };

        BgRectangle = new Rectangle
        {
            Width = Width,
            Height = Height,
            Fill = Parent.HorizontalEditG
                ? Utils.createLinGradBrushHor(Utils.EditColor1, Utils.EditColor2)
                : Utils.createLinGradBrushVer(Utils.EditColor1, Utils.EditColor2),
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
        PlaytimeEditBox = SampleBox($"{Parent.TotalH}h {Parent.TotalM}m {Parent.TotalS}s", 1, 1);
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
        
        BrowseExeButton = new CustomButton(text: "New exe", width: bWidth, height: bHeight, buttonImagePath: Utils.AddIcon,
            hA: HorizontalAlignment.Right);
        BrowseExeButton.Margin = new Thickness(0, -bHeight + 10, 180, 0);
        BrowseExeButton.Click += Parent.UpdateExe;
        
        OpenFolderButton = new CustomButton(text: "Open Folder", width: bWidth, height: bHeight, buttonImagePath: Utils.FolderIcon,
            hA: HorizontalAlignment.Right);
        OpenFolderButton.Margin = new Thickness(0, bHeight + 30, 180, 0);
        OpenFolderButton.Click += Parent.OpenExeFolder;
        
        ChangeIconButton = new CustomButton(text: "Change Icon", width: bWidth, height: bHeight, buttonImagePath: Utils.EditIcon,
            hA: HorizontalAlignment.Right);
        ChangeIconButton.Margin = new Thickness(0, -bHeight + 10, colMargins[0], 0);
        ChangeIconButton.Click += Parent.UpdateIcons;
        
        SaveButton = new CustomButton(text: "Save Data", width: bWidth, height: bHeight, buttonImagePath: Utils.SaveIcon,
            hA: HorizontalAlignment.Right, type:ButtonType.Positive);
        SaveButton.Margin = new Thickness(0, bHeight + 30, colMargins[0], 0);
        SaveButton.Click += Parent.editSaveButton_Click;

        Container.Children.Add(BrowseExeButton);
        Container.Children.Add(OpenFolderButton);
        Container.Children.Add(ChangeIconButton);
        Container.Children.Add(SaveButton);

        Content = Container;
    }

    private void ShowSaveIndicator(object sender, RoutedEventArgs e)
    {
        ShowSaveIndicatorMethod();
    }

    public void UpdateButtonColors()
    {
        BrowseExeButton.SetButtonColors();
        OpenFolderButton.SetButtonColors();
        ChangeIconButton.SetButtonColors();
        SaveButton.SetButtonColors();
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
            From = new Thickness(Container.Margin.Left, FromValue, 0, 0),
            To = new Thickness(Container.Margin.Left, ToValue, 0, 0),
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
            From = new Thickness(Container.Margin.Left, ToValue, 0, 0),
            To = new Thickness(Container.Margin.Left, FromValue, 0, 0),
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
            if (Parent.grid.Children.Contains(this)) Parent.grid.Children.Remove(this);
        };

        Container.BeginAnimation(MarginProperty, marginAnimation);
    }

    private TextBox SampleBox(string text = "", int col = 0, int row = 0)
    {
        TextBox sample = new TextBox
        {
            Text = text,
            Width = 180,
            Height = Utils.TextBoxHeight,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            TextAlignment = TextAlignment.Left,
            HorizontalContentAlignment = HorizontalAlignment.Left,
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

    private void editBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Parent.SaveEditedData();
            ShowSaveIndicatorMethod();

            e.Handled = true;
        }
    }

    public void ShowSaveIndicatorMethod()
    {
        if (ToSave)
        {
            SaveIndicator indicator = new SaveIndicator(Container, Parent.TileHeight);
            Container.Children.Add(indicator);
            ToSave = false;
        }
    }
}