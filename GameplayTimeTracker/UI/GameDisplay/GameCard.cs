using System;
using System.Drawing.Imaging;
using System.Text;
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
using GameplayTimeTracker.Menu;
using GameplayTimeTracker.Settings;
using Gtk;
using Grid = System.Windows.Controls.Grid;
using Image = System.Windows.Controls.Image;

namespace GameplayTimeTracker;

public class GameCard : UserControl
{
    public Panel ParentPanel { get; set; }
    private GameCardRepository GameCardRepository;

    private EntryRepository DataEntryRepository;

    public Entry DataEntry { get; set; }
    public EditMenu EditMenu { get; set; }

    public double CardWidth { get; set; }
    public double CardHeight { get; set; }
    public double CornerRadius { get; set; }
    public bool IsVertical { get; set; }
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

    public GameCard()
    {
    }

    public GameCard(Entry dataEntry, EntryRepository dataEntryRepository, GameCardRepository gameCardRepository,
        Panel parentPanel)
    {
        DataEntry = dataEntry;
        DataEntryRepository = dataEntryRepository;
        GameCardRepository = gameCardRepository;
        ParentPanel = parentPanel;
        // _appSettings = appSettings;

        ContainerGrid = new Grid();
        ContainerGrid.Margin = new Thickness(5);

        CardRectangle = new Rectangle
        {
            RadiusX = Common.BorderRadius,
            RadiusY = Common.BorderRadius,
            Effect = AppEffects.DropShadowIcon,
        };
        // BindingHelper.SetGradientColorBinding(CardRectangle, Shape.FillProperty, "Card 1", "Card 2", true);
        ContainerGrid.Children.Add(CardRectangle);

        HeroImage = new Image
        {
            Source = new BitmapImage(new Uri(DataEntry.HeroPath, UriKind.RelativeOrAbsolute)),
            Stretch = Stretch.Uniform,
        };
        Binding heroBinding = new Binding("HeroPath") { Source = DataEntry, Mode = BindingMode.OneWay, };
        BindingOperations.SetBinding(HeroImage, Image.SourceProperty, heroBinding);
        RenderOptions.SetBitmapScalingMode(HeroImage, BitmapScalingMode.HighQuality);
        ContainerGrid.Children.Add(HeroImage);

        IconImage = new Image
        {
            Source = new BitmapImage(new Uri(DataEntry.IconPath, UriKind.RelativeOrAbsolute)),
            Stretch = Stretch.Uniform,
            Effect = AppEffects.DropShadowGameIcon,
        };
        Binding iconBinding = new Binding("IconPath") { Source = DataEntry, Mode = BindingMode.OneWay, };
        BindingOperations.SetBinding(IconImage, Image.SourceProperty, iconBinding);
        RenderOptions.SetBitmapScalingMode(IconImage, BitmapScalingMode.HighQuality);
        ContainerGrid.Children.Add(IconImage);

        TitleBlock = new TextBlock
        {
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(AppColors.Font),
            Effect = AppEffects.DropOuterGlow,
            TextTrimming = TextTrimming.CharacterEllipsis,
            Padding = new Thickness(10, 0, 10, 0)
        };
        Binding titleBinding = new Binding("Name")
        {
            Source = DataEntry,
            Mode = BindingMode.TwoWay
        };
        TitleBlock.SetBinding(TextBlock.TextProperty, titleBinding);
        BindingHelper.SetColorBinding(TitleBlock, ForegroundProperty, "Font");
        ContainerGrid.Children.Add(TitleBlock);

        RunningTextBlock = new TextBlock
        {
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(AppColors.Running),
            Effect = AppEffects.dropShadowText,
            Padding = new Thickness(10, 0, 10, 0)
        };
        RunningTextBlock.DataContext = DataEntry;
        Binding runningBinding = new Binding("RunningFormatted")
        {
            Source = DataEntry,
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(RunningTextBlock, TextBlock.TextProperty, runningBinding);
        BindingHelper.SetColorBinding(RunningTextBlock, ForegroundProperty, "Running");
        ContainerGrid.Children.Add(RunningTextBlock);

        TotalProgressBar = new ProgressBar(150, 30, 5, 10);
        Binding totalPlayPercentBinding = new Binding("TotalPerc")
        {
            Source = DataEntry,
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(TotalProgressBar, ProgressBar.PercentageProperty, totalPlayPercentBinding);
        BindingHelper.SetColorBinding(TotalProgressBar.BackgroundRect, Shape.FillProperty,
            "Background");
        BindingHelper.SetGradientColorBinding(TotalProgressBar.BarRect, Shape.FillProperty,
            "Progress Bar 1", "Progress Bar 2", true);

        LastProgressBar = new ProgressBar(150, 30, 5, 10);
        Binding lastPlayPercentBinding = new Binding("LastPerc")
        {
            Source = DataEntry,
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(LastProgressBar, ProgressBar.PercentageProperty, lastPlayPercentBinding);
        BindingHelper.SetColorBinding(LastProgressBar.BackgroundRect, Shape.FillProperty, "Background");
        BindingHelper.SetGradientColorBinding(LastProgressBar.BarRect, Shape.FillProperty,
            "Progress Bar 1", "Progress Bar 2", true);

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
        BindingHelper.SetColorBinding(TotalPlaytimeBlock, ForegroundProperty, "Font");

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
        BindingHelper.SetColorBinding(LastPlaytimeBlock, ForegroundProperty, "Font");

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
        BindingHelper.SetColorBinding(LastPlayedOnBlock, ForegroundProperty, "Font");

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

        // StartRunningOscillation();
        // StartTimeIncrement();
    }

    private void CreateButtons()
    {
        var bEffect = AppEffects.DropShadowIcon;

        EditButton = new CustomButton(w: 40, h: 40, bImgPath: AppFiles.EditIcon,
            type: BType.Default, hA: HorizontalAlignment.Right, vA: VerticalAlignment.Top);
        EditButton.Effect = bEffect;
        EditButton.Click += ToggleEdit_Click;
        Panel.SetZIndex(EditButton, 3);
        ContainerGrid.Children.Add(EditButton);

        RemoveButton = new CustomButton(w: 40, h: 40, bImgPath: AppFiles.RemoveIcon,
            type: BType.Negative, hA: HorizontalAlignment.Right, vA: VerticalAlignment.Top);
        RemoveButton.Effect = bEffect;
        RemoveButton.Click += (s, e) =>
        {
            PromptMenu deletePrompt = new PromptMenu(
                textArray: new[] { "Are you sure to delete:", $"{DataEntry.Name}" },
                sizeArray: new[] { Common.EditTitleFontSize, Common.EditTitleFontSize + 2 },
                boldArray: new[] { false, true },
                lineSpacing: 5,
                type: PromptMenu.PromptType.YesNo,
                yesHandler: (s, e) => { DeleteInstance(); },
                noHandler: (s, e) => { Console.WriteLine("No clicked"); });
            deletePrompt.Open();
        };
        Panel.SetZIndex(RemoveButton, 3);
        ContainerGrid.Children.Add(RemoveButton);

        LaunchButton = new CustomButton(text: "Launch", w: 90, h: 40, type: BType.Positive,
            hA: HorizontalAlignment.Right, vA: VerticalAlignment.Bottom);
        LaunchButton.Effect = bEffect;
        Binding activeBinding = new Binding("IsLaunchable")
        {
            Source = DataEntry,
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(LaunchButton, CustomButton.ActiveProperty, activeBinding);
        LaunchButton.Click += (s, e) => Launcher.Launch(DataEntry);
        // SetLaunchButtonState();
        Panel.SetZIndex(LaunchButton, 3);
        ContainerGrid.Children.Add(LaunchButton);
    }

    private void DeleteInstance()
    {
        DataEntryRepository.RemoveEntry(DataEntry);
        DataHandler.WriteEntriesToFile(DataEntryRepository.EntriesList, AppFiles.DataFilePath);

        // Handle animation completion locally to avoid potential memory leaks
        EventHandler animationCompletedHandler = null;
        animationCompletedHandler = (s, e) =>
        {
            AppAnimations.DeleteOpacityAnimation.Completed -= animationCompletedHandler; // Unsubscribe after execution
            GameCardRepository.RemoveCard(this);
            ParentPanel.Children.Remove(this);
        };
        AppAnimations.DeleteOpacityAnimation.Completed += animationCompletedHandler;

        AppAnimations.DeleteThicknessAnimation.From = ContainerGrid.Margin;

        if (IsVertical)
        {
            double toMargin = -ContainerGrid.Width / 2;
            AppAnimations.DeleteThicknessAnimation.To =
                new Thickness(toMargin, 0, toMargin, 0);
        }
        else
        {
            double toMargin = -ContainerGrid.Height / 2;
            AppAnimations.DeleteThicknessAnimation.To =
                new Thickness(0, toMargin, 0, toMargin);
        }

        ContainerGrid.BeginAnimation(MarginProperty, AppAnimations.DeleteThicknessAnimation);
        ContainerGrid.BeginAnimation(OpacityProperty, AppAnimations.DeleteOpacityAnimation);
    }

    private void ToggleEdit_Click(object sender, RoutedEventArgs e)
    {
        // EditMenu = new EditMenu(DataEntry);
        // EditMenu.Open();
        EditMenu configMenu = new EditMenu(DataEntry);
        configMenu.Open();
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