using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GameplayTimeTracker.Menu;

public class PromptMenu : CustomMenu
{
    public enum PromptType
    {
        YesNo,
        Ok
    }

    public PromptMenu(string[] textArray, double[] sizeArray = null, bool[] boldArray = null, double spaceBetween = 0,
        double width = 300, double height = 200,
        PromptType type = PromptType.Ok,
        RoutedEventHandler yesHandler = null, RoutedEventHandler noHandler = null,
        bool performanceMode = true)
        : base(width, height, performanceMode)
    {
        sizeArray = SetSizeArray(sizeArray, textArray);
        boldArray = SetBoldArray(boldArray, textArray);

        TextBlock promptTextBlock = new TextBlock
        {
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 20, 0, 0),
            Foreground = new SolidColorBrush(AppColors.Font),
            TextWrapping = TextWrapping.Wrap,
        };
        for (int i = 0; i < textArray.Length; i++)
        {
            promptTextBlock.Inlines.Add(new Run
            {
                Text = textArray[i] + "\n",
                FontSize = sizeArray[i],
                FontWeight = boldArray[i] ? FontWeights.Bold : FontWeights.Regular,
            });
            promptTextBlock.Inlines.Add(new Run
                { Text = ".\n", Foreground = Brushes.Transparent, FontSize = spaceBetween });
        }

        MenuContentGrid.Children.Add(promptTextBlock);

        switch (type)
        {
            case PromptType.YesNo:
                double margin = 120;
                var yesButton = new CustomButton(text: "Yes", width: 100, height: 35, vA: VerticalAlignment.Bottom,
                    type: CustomButton.ButtonType.Negative, effect: AppEffects.DropShadowMedium);
                yesButton.Margin = new Thickness(0, 0, margin, 20);
                var noButton = new CustomButton(text: "No", width: 100, height: 35, vA: VerticalAlignment.Bottom,
                    type: CustomButton.ButtonType.Positive, effect: AppEffects.DropShadowMedium);
                noButton.Margin = new Thickness(margin, 0, 0, 20);
                
                yesButton.Click += (s, e) =>
                {
                    (yesHandler ?? ((s, e) => Console.WriteLine("No REH")))(s, e);
                    Close();
                };
                noButton.Click += (s, e) =>
                {
                    (noHandler ?? ((s, e) => Console.WriteLine("No REH")))(s, e);
                    Close();
                };
                
                MenuContentGrid.Children.Add(yesButton);
                MenuContentGrid.Children.Add(noButton);
                break;
            case PromptType.Ok:
                var okButton = new CustomButton(text: "Ok", width: 100, height: 35, hA: HorizontalAlignment.Center,
                    vA: VerticalAlignment.Bottom, effect: AppEffects.DropShadowMedium);
                okButton.Margin = new Thickness(0, 0, 0, 20);
                okButton.Click += (s, e) => { Close(); };
                MenuContentGrid.Children.Add(okButton);
                break;
            default:
                break;
        }
    }

    private double[] SetSizeArray(double[] sizeArray, string[] textArray) =>
        (sizeArray ?? new double[0])
        .Concat(Enumerable.Repeat(Common.EditTitleFontSize, Math.Max(0, textArray.Length - (sizeArray?.Length ?? 0))))
        .ToArray();

    private bool[] SetBoldArray(bool[] boldArray, string[] textArray) =>
        Enumerable.Range(0, textArray.Length)
            .Select(i => boldArray != null && i < boldArray.Length && boldArray[i])
            .ToArray();
}