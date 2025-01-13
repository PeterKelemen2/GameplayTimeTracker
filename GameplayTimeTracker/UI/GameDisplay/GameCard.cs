using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameplayTimeTracker;

public class GameCard : UserControl
{
    private GameCardRepository GameCardRepository;
    public Entry DataEntry { get; set; }

    public double CardWidth { get; set; }
    public double CardHeight { get; set; }
    public double CornerRadius { get; set; }

    public Grid ContainerGrid { get; set; }
    public TextBlock TitleBlock { get; set; }
    public TextBlock LastPlaytimeBlock { get; set; }
    public TextBlock TotalPlaytimeBlock { get; set; }
    public TextBlock RunningTextBlock { get; set; }

    public Image HeroImage { get; set; }
    public Image IconImage { get; set; }

    public CustomButton LaunchButton { get; set; }
    public CustomButton EditButton { get; set; }
    public CustomButton RemoveButton { get; set; }

    public GameCard(Entry dataEntry, GameCardRepository gameCardRepository)
    {
        this.GameCardRepository = gameCardRepository;
        DataEntry = dataEntry;

        ContainerGrid = new Grid
        {
            Width = 200,
            Height = 100,
            Background = new SolidColorBrush(AppColors.EditColor1),
            Margin = new Thickness(10)
        };
        Content = ContainerGrid;
    }
}