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

    public ProgressBar TotalProgressBar { get; set; }
    public ProgressBar LastProgressBar { get; set; }


    public CustomButton LaunchButton { get; set; }
    public CustomButton EditButton { get; set; }
    public CustomButton RemoveButton { get; set; }

    public GameCard(Entry dataEntry, GameCardRepository gameCardRepository, StackPanel parentStackPanel)
    {
        GameCardRepository = gameCardRepository;
        DataEntry = dataEntry;
        ParentStackPanel = parentStackPanel;

        ContainerGrid = new Grid();

        CardRectangle = new Rectangle
        {
            RadiusX = 10,
            RadiusY = 10,
            Effect = AppEffects.dropShadowIcon,
        };
        ContainerGrid.Children.Add(CardRectangle);

        HeroImage = new Image
        {
            Source = new BitmapImage(new Uri(DataEntry.HeroPath, UriKind.RelativeOrAbsolute)),
            Stretch = Stretch.Uniform,
        };
        RenderOptions.SetBitmapScalingMode(HeroImage, BitmapScalingMode.HighQuality);
        ContainerGrid.Children.Add(HeroImage);

        IconImage = new Image
        {
            Source = new BitmapImage(new Uri(DataEntry.IconPath, UriKind.RelativeOrAbsolute)),
            Stretch = Stretch.Uniform,
            Effect = AppEffects.DropShadowGameIcon,
        };
        RenderOptions.SetBitmapScalingMode(IconImage, BitmapScalingMode.HighQuality);
        ContainerGrid.Children.Add(IconImage);

        TitleBlock = new TextBlock
        {
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(AppColors.Font),
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
            Foreground = new SolidColorBrush(AppColors.Running),
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

        TotalProgressBar = new ProgressBar(150, 30, 5, 10, 0.25);
        ContainerGrid.Children.Add(TotalProgressBar);

        CreateButtons();

        Content = ContainerGrid;
    }

    private void CreateButtons()
    {
        var bEffect = AppEffects.dropShadowIcon;

        EditButton = new CustomButton(width: 40, height: 40, buttonImagePath: AppFiles.EditIcon,
            type: CustomButton.ButtonType.Default, hA: HorizontalAlignment.Right, vA: VerticalAlignment.Top);
        EditButton.Effect = bEffect;
        // EditButton.Click += ToggleEdit_Click;
        Panel.SetZIndex(EditButton, 3);
        ContainerGrid.Children.Add(EditButton);

        RemoveButton = new CustomButton(width: 40, height: 40, buttonImagePath: AppFiles.RemoveIcon,
            type: CustomButton.ButtonType.Negative, hA: HorizontalAlignment.Right, vA: VerticalAlignment.Top);
        RemoveButton.Effect = bEffect;
        // RemoveButton.Click += OpenDeleteDialog;
        Panel.SetZIndex(RemoveButton, 3);
        ContainerGrid.Children.Add(RemoveButton);

        LaunchButton = new CustomButton(text: "Launch", width: 90, height: 40, type: CustomButton.ButtonType.Positive,
            hA: HorizontalAlignment.Right, vA: VerticalAlignment.Bottom);
        LaunchButton.Effect = bEffect;
        // LaunchButton.Click += LaunchExe;
        // SetLaunchButtonState();
        Panel.SetZIndex(LaunchButton, 3);
        ContainerGrid.Children.Add(LaunchButton);
    }
}