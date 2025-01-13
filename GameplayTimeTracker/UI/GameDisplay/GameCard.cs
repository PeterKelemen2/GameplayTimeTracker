using System.Windows;
using System.Windows.Controls;
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

        ContainerGrid = new Grid
        {
            Width = ParentStackPanel.ActualWidth - Common.CardPadding * 2,
            Height = 150,
            Margin = new Thickness(10, 10, 10, 0)
        };

        Rectangle CardRectangle = new Rectangle
        {
            Width = ContainerGrid.Width,
            Height = ContainerGrid.Height,
            RadiusX = 10,
            RadiusY = 10,
            Fill = AppColors.CreateLinGradBrushHor(AppColors.CardColor1, AppColors.CardColor2),
            Effect = AppEffects.dropShadowIcon,
        };
        ContainerGrid.Children.Add(CardRectangle);

        Content = ContainerGrid;
    }
}