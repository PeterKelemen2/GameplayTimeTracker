using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameplayTimeTracker;

public class GameCard : UserControl
{
    public StackPanel ParentStackPanel { get; set; }
    private GameCardRepository GameCardRepository;
    public Entry DataEntry { get; set; }

    public double CardWidth { get; set; }
    public double CardHeight { get; set; }
    public double CornerRadius { get; set; }

    public Grid ContainerGrid { get; set; }
    public Rectangle CardRectangle { get; set; }
    public TextBlock TitleBlock { get; set; }
    public TextBlock LastPlaytimeBlock { get; set; }
    public TextBlock TotalPlaytimeBlock { get; set; }
    public TextBlock RunningTextBlock { get; set; }

    public Image HeroImage { get; set; }
    public Image IconImage { get; set; }

    public CustomButton LaunchButton { get; set; }
    public CustomButton EditButton { get; set; }
    public CustomButton RemoveButton { get; set; }

    public GameCard(Entry dataEntry, GameCardRepository gameCardRepository, StackPanel parentStackPanel)
    {
        GameCardRepository = gameCardRepository;
        DataEntry = dataEntry;
        ParentStackPanel = parentStackPanel;

        // ContainerGrid = new Grid
        // {
        //     Width = ParentStackPanel.ActualWidth - Common.CardPadding * 2,
        //     Height = 150,
        //     Margin = new Thickness(10, 10, 10, 0)
        // };
        ContainerGrid = new Grid();

        CardRectangle = new Rectangle
        {
            // Width = ContainerGrid.Width,
            // Height = ContainerGrid.Height,
            RadiusX = 10,
            RadiusY = 10,
            // Fill = AppColors.CreateLinGradBrushHor(AppColors.CardColor1, AppColors.CardColor2),
            Effect = AppEffects.dropShadowIcon,
        };
        ContainerGrid.Children.Add(CardRectangle);

        HeroImage = new Image
        {
            Source = new BitmapImage(new Uri(DataEntry.HeroPath, UriKind.RelativeOrAbsolute)),
            // Height = ContainerGrid.Height,
            HorizontalAlignment = HorizontalAlignment.Left,
            Stretch = Stretch.Uniform,
        };
        // HeroImage.OpacityMask = new LinearGradientBrush
        // {
        //     StartPoint = new Point(0, 0), // Start from the left
        //     EndPoint = new Point(1, 0), // End on the right
        //     GradientStops = new GradientStopCollection
        //     {
        //         new GradientStop(Colors.Black, 0.0), // Full opacity on the left
        //         new GradientStop(Colors.Transparent, 1.0) // Fully transparent on the right
        //     }
        // };
        // HeroImage.Clip = new RectangleGeometry(new Rect(0, 0, CardRectangle.Width, ContainerGrid.Height),
        //     CardRectangle.RadiusX, CardRectangle.RadiusY);
        RenderOptions.SetBitmapScalingMode(HeroImage, BitmapScalingMode.HighQuality);
        ContainerGrid.Children.Add(HeroImage);

        IconImage = new Image
        {
            Source = new BitmapImage(new Uri(DataEntry.IconPath, UriKind.RelativeOrAbsolute)),
            // Height = ContainerGrid.Height * 0.6,
            HorizontalAlignment = HorizontalAlignment.Left,
            Stretch = Stretch.Uniform,
            // Margin = new Thickness(ContainerGrid.Height * 0.3, 30, 0, 0),
            Effect = AppEffects.DropShadowGameIcon,
        };
        RenderOptions.SetBitmapScalingMode(IconImage, BitmapScalingMode.HighQuality);
        ContainerGrid.Children.Add(IconImage);

        TitleBlock = new TextBlock
        {
            FontWeight = FontWeights.Bold,
            FontSize = Common.TitleFontSize,
            Foreground = new SolidColorBrush(AppColors.Font),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin =
                new Thickness(CardRectangle.RadiusX, CardRectangle.RadiusX / 2, 0, 0),
            Effect = AppEffects.DropOuterGlow,
        };
        Binding titleBinding = new Binding("Name")
        {
            Source = DataEntry,
            Mode = BindingMode.TwoWay
        };
        TitleBlock.SetBinding(TextBlock.TextProperty, titleBinding);
        ContainerGrid.Children.Add(TitleBlock);

        RunningTextBlock = new TextBlock
        {
            FontWeight = FontWeights.Bold,
            FontSize = Common.TitleFontSize - 4,
            Foreground = new SolidColorBrush(AppColors.Running),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(CardRectangle.RadiusX, CardRectangle.RadiusX + Common.TitleFontSize -3 , 0, 0),
            Effect = AppEffects.dropShadowText,
        };
        RunningTextBlock.Text = "Running!";
        // RunningTextBlock.DataContext = DataEntry; 
        // Binding runningBinding = new Binding("RunningFormatted")
        // {
        //     Source = DataEntry, 
        //     Mode = BindingMode.OneWay,
        // };
        // BindingOperations.SetBinding(RunningTextBlock, TextBlock.TextProperty, runningBinding);
        ContainerGrid.Children.Add(RunningTextBlock);

        CreateButtons();

        Content = ContainerGrid;
    }

    private void CreateButtons()
    {
        var bEffect = AppEffects.dropShadowIcon;

        EditButton = new CustomButton(width: 40, height: 40, buttonImagePath: AppFiles.EditIcon,
            type: CustomButton.ButtonType.Default, hA: HorizontalAlignment.Right, vA: VerticalAlignment.Top);
        // EditButton.Margin = new Thickness(0, ContainerGrid.Height / 2 - EditButton.Height - 5, 100, 0);
        EditButton.Effect = bEffect;
        // EditButton.Click += ToggleEdit_Click;
        Panel.SetZIndex(EditButton, 3);
        ContainerGrid.Children.Add(EditButton);

        RemoveButton = new CustomButton(width: 40, height: 40, buttonImagePath: AppFiles.RemoveIcon,
            type: CustomButton.ButtonType.Negative, hA: HorizontalAlignment.Right, vA: VerticalAlignment.Top);
        // RemoveButton.Margin = new Thickness(0, ContainerGrid.Height / 2 - RemoveButton.Height - 5, 50, 0);
        RemoveButton.Effect = bEffect;
        // RemoveButton.Click += OpenDeleteDialog;
        Panel.SetZIndex(RemoveButton, 3);
        ContainerGrid.Children.Add(RemoveButton);

        LaunchButton = new CustomButton(text: "Launch", width: 90, height: 40, type: CustomButton.ButtonType.Positive,
            hA: HorizontalAlignment.Right, vA: VerticalAlignment.Bottom);
        // LaunchButton.Margin = new Thickness(0, 0, 50, ContainerGrid.Height / 2 - LaunchButton.Height - 5);
        LaunchButton.Effect = bEffect;
        // LaunchButton.Click += LaunchExe;
        // SetLaunchButtonState();
        Panel.SetZIndex(LaunchButton, 3);
        ContainerGrid.Children.Add(LaunchButton);
    }
}