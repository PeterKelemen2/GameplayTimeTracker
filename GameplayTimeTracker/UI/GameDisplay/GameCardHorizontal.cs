using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameplayTimeTracker;

public class GameCardHorizontal : GameCard
{
    public GameCardHorizontal(Entry dataEntry, GameCardRepository gameCardRepository, Panel parentPanel) :
        base(dataEntry, gameCardRepository, parentPanel)
    {
        Loaded += GameCardHorizontal_Loaded;
    }

    private void GameCardHorizontal_Loaded(object sender, RoutedEventArgs e)
    {
        ContainerGrid.Width = 850;
        ContainerGrid.Height = 150;
        ContainerGrid.Margin = new Thickness(10, 10, 10, 0);

        CardRectangle.Width = ContainerGrid.Width;
        CardRectangle.Height = ContainerGrid.Height;
        CardRectangle.Fill = AppColors.CreateLinGradBrushHor(AppColors.CardColor1, AppColors.CardColor2);

        EditButton.Margin = new Thickness(0, ContainerGrid.Height / 2 - EditButton.Height - 5, 80, 0);
        RemoveButton.Margin = new Thickness(0, ContainerGrid.Height / 2 - RemoveButton.Height - 5, 30, 0);
        LaunchButton.Margin = new Thickness(0, 0, 30, ContainerGrid.Height / 2 - LaunchButton.Height - 5);

        TitleBlock.FontSize = Common.TitleFontSize;
        TitleBlock.FontWeight = FontWeights.Bold;
        TitleBlock.HorizontalAlignment = HorizontalAlignment.Left;
        TitleBlock.VerticalAlignment = VerticalAlignment.Top;
        TitleBlock.Margin = new Thickness(CardRectangle.RadiusX, CardRectangle.RadiusX / 2, 0, 0);

        RunningTextBlock.FontSize = Common.TitleFontSize - 4;
        RunningTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
        RunningTextBlock.VerticalAlignment = VerticalAlignment.Top;
        RunningTextBlock.Margin =
            new Thickness(CardRectangle.RadiusX, CardRectangle.RadiusX + Common.TitleFontSize - 3, 0, 0);

        double stackMargin = 200;
        TotalPlaytimeBlock.FontSize = Common.TitleFontSize - 2;
        TotalProgressBar.HorizontalAlignment = HorizontalAlignment.Left;
        TotalStack.HorizontalAlignment = HorizontalAlignment.Left;
        foreach (UIElement child in TotalStack.Children)
        {
            if (child is FrameworkElement fe) fe.Margin = new Thickness(0, 5, 0, 0);
        }

        TotalStack.Margin = new Thickness(stackMargin, 20, 0, 0);

        LastPlaytimeBlock.FontSize = Common.TitleFontSize - 2;
        LastPlayedOnBlock.FontSize = Common.TitleFontSize - 2;
        LastProgressBar.HorizontalAlignment = HorizontalAlignment.Left;
        LastStack.HorizontalAlignment = HorizontalAlignment.Right;
        foreach (UIElement child in LastStack.Children)
        {
            if (child is FrameworkElement fe) fe.Margin = new Thickness(0, 5, 0, 0);
        }

        LastStack.Width = 160;
        LastStack.Margin = new Thickness(0, 20, stackMargin, 0);

        IconImage.Height = ContainerGrid.Height * 0.6;
        IconImage.HorizontalAlignment = HorizontalAlignment.Left;
        IconImage.Margin = new Thickness(ContainerGrid.Height * 0.3, 30, 0, 0);

        HeroImage.Height = ContainerGrid.Height;
        HeroImage.HorizontalAlignment = HorizontalAlignment.Left;
        HeroImage.OpacityMask = new LinearGradientBrush
        {
            StartPoint = new Point(0.33, 0), // Start from the left
            EndPoint = new Point(1, 0), // End on the right
            GradientStops = new GradientStopCollection
            {
                new GradientStop(Colors.Black, 0.0), // Full opacity on the left
                new GradientStop(Colors.Transparent, 1.0) // Fully transparent on the right
            }
        };
        HeroImage.Clip = new RectangleGeometry(new Rect(0, 0, CardRectangle.Width, ContainerGrid.Height),
            CardRectangle.RadiusX, CardRectangle.RadiusY);
    }
}