using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameplayTimeTracker;

public class GameCardVertical : GameCard
{
    public GameCardVertical(Entry dataEntry, GameCardRepository gameCardRepository, Panel parentPanel) :
        base(dataEntry, gameCardRepository, parentPanel)
    {
        Loaded += GameCardVertical_Loaded;
    }

    private void GameCardVertical_Loaded(object sender, RoutedEventArgs e)
    {
        ContainerGrid.Width = 270;
        ContainerGrid.Height = 420;
        ContainerGrid.Margin = new Thickness(10, 10, 0, 0);

        CardRectangle.Width = ContainerGrid.Width;
        CardRectangle.Height = ContainerGrid.Height;
        CardRectangle.Fill = AppColors.CreateLinGradBrushVer(AppColors.CardColor1, AppColors.CardColor2);

        EditButton.VerticalAlignment = VerticalAlignment.Bottom;
        EditButton.HorizontalAlignment = HorizontalAlignment.Right;
        RemoveButton.VerticalAlignment = VerticalAlignment.Bottom;
        RemoveButton.HorizontalAlignment = HorizontalAlignment.Right;
        LaunchButton.VerticalAlignment = VerticalAlignment.Bottom;
        LaunchButton.HorizontalAlignment = HorizontalAlignment.Left;
        double bMargin = 10;
        EditButton.Margin = new Thickness(0, 0, EditButton.Grid.Width + bMargin * 2, bMargin);
        RemoveButton.Margin = new Thickness(0, 0, bMargin, bMargin);
        LaunchButton.Margin = new Thickness(bMargin, 0, 0, bMargin);

        TitleBlock.FontSize = Common.TitleFontSize * 1.5;
        TitleBlock.FontWeight = FontWeights.Regular;
        TitleBlock.HorizontalAlignment = HorizontalAlignment.Center;
        TitleBlock.VerticalAlignment = VerticalAlignment.Top;
        TitleBlock.Margin = new Thickness(0, ContainerGrid.Height * 0.25, 0, 0);

        RunningTextBlock.FontSize = Common.TitleFontSize - 4;
        RunningTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
        RunningTextBlock.VerticalAlignment = VerticalAlignment.Top;
        RunningTextBlock.Margin =
            new Thickness(0, ContainerGrid.Height * 0.25 + 30, 0, 0);

        TotalPlaytimeBlock.FontSize = Common.TitleFontSize - 2;
        TotalProgressBar.HorizontalAlignment = HorizontalAlignment.Left;
        TotalProgressBar.UpdateBgWidth(200);
        TotalStack.HorizontalAlignment = HorizontalAlignment.Center;
        foreach (UIElement child in TotalStack.Children)
        {
            if (child is FrameworkElement fe) fe.Margin = new Thickness(0, 5, 0, 0);
        }

        TotalStack.Margin = new Thickness(0, 150, 0, 0);

        LastPlaytimeBlock.FontSize = Common.TitleFontSize - 2;
        LastPlayedOnBlock.FontSize = Common.TitleFontSize - 2;
        LastProgressBar.HorizontalAlignment = HorizontalAlignment.Left;
        LastProgressBar.UpdateBgWidth(200);
        LastStack.HorizontalAlignment = HorizontalAlignment.Center;
        foreach (UIElement child in LastStack.Children)
        {
            if (child is FrameworkElement fe) fe.Margin = new Thickness(0, 5, 0, 0);
        }

        LastStack.Margin = new Thickness(0, 240, 0, 0);

        IconImage.Height = ContainerGrid.Width * 0.33;
        IconImage.HorizontalAlignment = HorizontalAlignment.Center;
        IconImage.VerticalAlignment = VerticalAlignment.Top;
        IconImage.Margin = new Thickness(0, 30, 0, 0);

        HeroImage.Height = ContainerGrid.Height;
        HeroImage.HorizontalAlignment = HorizontalAlignment.Left;
        HeroImage.VerticalAlignment = VerticalAlignment.Top;
        HeroImage.OpacityMask = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0.5),
            EndPoint = new Point(0, 1),
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