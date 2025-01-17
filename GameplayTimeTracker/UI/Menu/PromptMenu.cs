using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GameplayTimeTracker.Menu;

public class PromptMenu : CustomMenu
{
    public enum PromptType { YesNo, Ok }

    public PromptMenu(
        string[] textArray, double[] sizeArray = null, bool[] boldArray = null, double lineSpacing = 0,
        double width = 300, double height = 200, PromptType type = PromptType.Ok,
        RoutedEventHandler yesHandler = null, RoutedEventHandler noHandler = null,
        bool performanceMode = true)
        : base(width, height, performanceMode)
    {
        sizeArray = (sizeArray ?? new double[0])
            .Concat(Enumerable.Repeat(Common.EditTitleFontSize, Math.Max(0, textArray.Length - (sizeArray?.Length ?? 0))))
            .ToArray();
        boldArray = Enumerable.Range(0, textArray.Length)
            .Select(i => boldArray != null && i < boldArray.Length && boldArray[i])
            .ToArray();

        var promptTextBlock = new TextBlock
        {
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 20, 0, 0),
            Foreground = new SolidColorBrush(AppColors.Font),
            TextWrapping = TextWrapping.Wrap
        };

        for (int i = 0; i < textArray.Length; i++)
        {
            promptTextBlock.Inlines.Add(new Run
            {
                Text = textArray[i] + "\n",
                FontSize = sizeArray[i],
                FontWeight = boldArray[i] ? FontWeights.Bold : FontWeights.Regular
            });
            promptTextBlock.Inlines.Add(new Run { Text = ".\n", Foreground = Brushes.Transparent, FontSize = lineSpacing });
        }
        MenuContentGrid.Children.Add(promptTextBlock);

        if (type == PromptType.YesNo)
        {
            AddButton("Yes", 60, yesHandler, CustomButton.ButtonType.Positive);
            AddButton("No", -60, noHandler, CustomButton.ButtonType.Negative);
        }
        else
        {
            AddButton("Ok", 0, null, CustomButton.ButtonType.Default);
        }
    }

    private void AddButton(string text, double horizontalMargin, RoutedEventHandler handler, CustomButton.ButtonType buttonType)
    {
        var button = new CustomButton(text: text, width: 100, height: 35, hA: HorizontalAlignment.Center,
            vA: VerticalAlignment.Bottom, type: buttonType, effect: AppEffects.DropShadowMedium)
        {
            Margin = new Thickness(-horizontalMargin, 0, horizontalMargin, 20)
        };
        button.Click += (s, e) =>
        {
            (handler ?? ((s, e) => Console.WriteLine($"No Event set for {text} Button")))(s, e);
            Close();
        };
        MenuContentGrid.Children.Add(button);
    }
}
