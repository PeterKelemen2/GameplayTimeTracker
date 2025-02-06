using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameplayTimeTracker;

public class GameCardCompact : GameCard
{
    public GameCardCompact(Entry dataEntry, EntryRepository dataEntryRepository,
        GameCardRepository gameCardRepository,
        Panel parentPanel) :
        base(dataEntry, dataEntryRepository, gameCardRepository, parentPanel)
    {
        Loaded += GameCardCompact_Loaded;
    }

    private void GameCardCompact_Loaded(object sender, RoutedEventArgs e)
    {
        IsVertical = false;
        ContainerGrid.Width = 830;
        ContainerGrid.Height = 80;

        CardRectangle.Width = ContainerGrid.Width;
        CardRectangle.Height = ContainerGrid.Height;
        BindingHelper.SetGradientColorBinding(CardRectangle, Shape.FillProperty, "Card 1", "Card 2", true);

        IconImage.Height = ContainerGrid.Height * 0.6;
        IconImage.HorizontalAlignment = HorizontalAlignment.Left;
        IconImage.Margin = new Thickness(ContainerGrid.Height * 0.3, 0, 0, 0);

        TitleBlock.FontSize = Common.TitleFontSize;
        TitleBlock.FontWeight = FontWeights.Bold;
        TitleBlock.HorizontalAlignment = HorizontalAlignment.Left;
        TitleBlock.VerticalAlignment = VerticalAlignment.Top;
        TitleBlock.Margin = new Thickness(85, CardRectangle.RadiusX / 2, 0, 0);
        BindingHelper.SetRunningColorBinding(TitleBlock, ForegroundProperty, DataEntry);

        double stackMargin = 200;
        TotalPlaytimeBlock.FontSize = Common.TitleFontSize - 2;
        TotalProgressBar.HorizontalAlignment = HorizontalAlignment.Left;
        TotalProgressBar.UpdateBgWidth(180);
        TotalStack.HorizontalAlignment = HorizontalAlignment.Left;
        foreach (UIElement child in TotalStack.Children)
        {
            if (child is FrameworkElement fe) fe.Margin = new Thickness(0, 5, 0, 0);
        }

        TotalStack.Margin = new Thickness(stackMargin, 25, 0, 0);

        LastPlaytimeBlock.FontSize = Common.TitleFontSize - 2;
        LastPlayedOnBlock.FontSize = Common.TitleFontSize - 2;
        LastProgressBar.HorizontalAlignment = HorizontalAlignment.Left;
        LastProgressBar.UpdateBgWidth(180);
        LastStack.HorizontalAlignment = HorizontalAlignment.Right;
        foreach (UIElement child in LastStack.Children)
        {
            if (child is FrameworkElement fe) fe.Margin = new Thickness(0, 5, 0, 0);
        }

        LastStack.Width = 180;
        LastStack.Margin = new Thickness(0, 25, stackMargin, 0);

        RunningTextBlock.Visibility = Visibility.Collapsed;
        HeroImage.Visibility = Visibility.Collapsed;
        TotalProgressBar.Visibility = Visibility.Collapsed;
        LastPlayedOnBlock.Visibility = Visibility.Collapsed;
        LastProgressBar.Visibility = Visibility.Collapsed;

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        EditButton.HorizontalAlignment = HorizontalAlignment.Right;
        EditButton.VerticalAlignment = VerticalAlignment.Center;
        RemoveButton.HorizontalAlignment = HorizontalAlignment.Right;
        RemoveButton.VerticalAlignment = VerticalAlignment.Center;
        LaunchButton.HorizontalAlignment = HorizontalAlignment.Right;
        LaunchButton.VerticalAlignment = VerticalAlignment.Center;

        LaunchButton.Margin = new Thickness(0, 0, 20, 0);
        RemoveButton.Margin = new Thickness(0, 0, 30 + LaunchButton.W, 0);
        EditButton.Margin = new Thickness(0, 0, 40 + LaunchButton.W + RemoveButton.W, 0);
    }
}