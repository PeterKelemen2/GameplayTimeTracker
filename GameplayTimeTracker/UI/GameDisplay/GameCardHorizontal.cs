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
    public GameCardHorizontal(Entry dataEntry, GameCardRepository gameCardRepository, StackPanel parentStackPanel) :
        base(dataEntry, gameCardRepository, parentStackPanel)
    {
        Loaded += GameCardHorizontal_Loaded;
    }

    private void GameCardHorizontal_Loaded(object sender, RoutedEventArgs e)
    {
        ContainerGrid.Width = ParentStackPanel.ActualWidth - Common.CardPadding * 2;
        ContainerGrid.Height = 150;
        ContainerGrid.Margin = new Thickness(10, 10, 10, 0);

        CardRectangle.Width = ContainerGrid.Width;
        CardRectangle.Height = ContainerGrid.Height;
        CardRectangle.Fill = AppColors.CreateLinGradBrushHor(AppColors.CardColor1, AppColors.CardColor2);

        EditButton.Margin = new Thickness(0, ContainerGrid.Height / 2 - EditButton.Height - 5, 100, 0);
        RemoveButton.Margin = new Thickness(0, ContainerGrid.Height / 2 - RemoveButton.Height - 5, 50, 0);
        LaunchButton.Margin = new Thickness(0, 0, 50, ContainerGrid.Height / 2 - LaunchButton.Height - 5);

        
        HeroImage.Height = ContainerGrid.Height;

        IconImage.Height = ContainerGrid.Height * 0.6;
        IconImage.Margin = new Thickness(ContainerGrid.Height * 0.3, 30, 0, 0);

        HeroImage.OpacityMask = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0), // Start from the left
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