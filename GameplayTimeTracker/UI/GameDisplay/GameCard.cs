using System;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;
using Gtk;
using Grid = System.Windows.Controls.Grid;
using Image = System.Windows.Controls.Image;

namespace GameplayTimeTracker;

public class GameCard : UserControl
{
    public Panel ParentPanel { get; set; }
    private GameCardRepository GameCardRepository;
    public Entry DataEntry { get; set; }

    public double CardWidth { get; set; }
    public double CardHeight { get; set; }
    public double CornerRadius { get; set; }

    public Grid ContainerGrid { get; set; }
    public StackPanel TotalStack { get; set; }
    public StackPanel LastStack { get; set; }
    public Rectangle CardRectangle { get; set; }
    public TextBlock TitleBlock { get; set; }
    public TextBlock LastPlaytimeBlock { get; set; }
    public TextBlock LastPlayedOnBlock { get; set; }
    public TextBlock TotalPlaytimeBlock { get; set; }
    public TextBlock RunningTextBlock { get; set; }

    public Image HeroImage { get; set; }
    public Image IconImage { get; set; }

    public ProgressBar TotalProgressBar { get; set; }
    public ProgressBar LastProgressBar { get; set; }

    public CustomButton LaunchButton { get; set; }
    public CustomButton EditButton { get; set; }
    public CustomButton RemoveButton { get; set; }

    public GameCard(Entry dataEntry, GameCardRepository gameCardRepository, Panel parentPanel)
    {
        GameCardRepository = gameCardRepository;
        DataEntry = dataEntry;
        ParentPanel = parentPanel;

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
        // RunningTextBlock.Text = "Running!";
        RunningTextBlock.DataContext = DataEntry;
        Binding runningBinding = new Binding("RunningFormatted")
        {
            Source = DataEntry,
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(RunningTextBlock, TextBlock.TextProperty, runningBinding);
        ContainerGrid.Children.Add(RunningTextBlock);

        TotalProgressBar = new ProgressBar(150, 30, 5, 10, DataEntry.TotalPerc);
        LastProgressBar = new ProgressBar(150, 30, 5, 10, DataEntry.LastPerc);

        TotalPlaytimeBlock = new TextBlock
        {
            Text = "Total Playtime\n",
            Foreground = new SolidColorBrush(AppColors.Font),
            FontWeight = FontWeights.Bold,
            VerticalAlignment = VerticalAlignment.Top,
            TextAlignment = TextAlignment.Left,
            Effect = AppEffects.DropOuterGlow,
        };
        var totalTimeRun = new Run { FontWeight = FontWeights.Regular };
        TotalPlaytimeBlock.Inlines.Add(totalTimeRun);
        Binding totalPlayBinding = new Binding("TotalPlayFormatted")
        {
            Source = DataEntry,
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(totalTimeRun, Run.TextProperty, totalPlayBinding);

        LastPlaytimeBlock = new TextBlock
        {
            Text = "Last Playtime\n",
            Foreground = new SolidColorBrush(AppColors.Font),
            FontWeight = FontWeights.Bold,
            VerticalAlignment = VerticalAlignment.Top,
            TextAlignment = TextAlignment.Left,
            Effect = AppEffects.DropOuterGlow,
        };
        var lastTimeRun = new Run { FontWeight = FontWeights.Regular };
        LastPlaytimeBlock.Inlines.Add(lastTimeRun);
        Binding lastPlayBinding = new Binding("LastPlayFormatted")
        {
            Source = DataEntry,
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(lastTimeRun, Run.TextProperty, lastPlayBinding);

        LastPlayedOnBlock = new TextBlock
        {
            Foreground = new SolidColorBrush(AppColors.Font),
            FontWeight = FontWeights.Bold,
            VerticalAlignment = VerticalAlignment.Top,
            TextAlignment = TextAlignment.Left,
            Effect = AppEffects.DropOuterGlow,
        };
        var lastPlayDateRun = new Run { FontWeight = FontWeights.Regular };
        var lastPlayedOnRun = new Run { FontWeight = FontWeights.Bold };
        LastPlayedOnBlock.Inlines.Add(lastPlayedOnRun);
        LastPlayedOnBlock.Inlines.Add(lastPlayDateRun);

        Binding lastPlayStateBinding = new Binding("LastRunningStateFormatted")
        {
            Source = DataEntry,
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(lastPlayedOnRun, Run.TextProperty, lastPlayStateBinding);

        Binding lastPlayDateBinding = new Binding("LastDateFormatted")
        {
            Source = DataEntry,
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(lastPlayDateRun, Run.TextProperty, lastPlayDateBinding);

        TotalStack = new StackPanel();
        TotalStack.Children.Add(TotalPlaytimeBlock);
        TotalStack.Children.Add(TotalProgressBar);
        ContainerGrid.Children.Add(TotalStack);

        LastStack = new StackPanel();
        LastStack.Children.Add(LastPlaytimeBlock);
        LastStack.Children.Add(LastProgressBar);
        LastStack.Children.Add(LastPlayedOnBlock);
        ContainerGrid.Children.Add(LastStack);

        CreateButtons();

        Content = ContainerGrid;

        StartRunningOscillation();
        StartTimeIncrement();
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

    private DispatcherTimer progressBarTimer;

    private void StartRunningOscillation()
    {
        progressBarTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        progressBarTimer.Tick += (sender, e) => { DataEntry.IsRunning = !DataEntry.IsRunning; };

        progressBarTimer.Start();
    }

    private DispatcherTimer timeTimer;

    private void StartTimeIncrement()
    {
        timeTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        timeTimer.Tick += (sender, e) => { DataEntry.IncrementTime(); };
        timeTimer.Start();
    }
}