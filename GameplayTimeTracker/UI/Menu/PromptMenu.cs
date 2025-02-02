using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using GameplayTimeTracker.Settings;

namespace GameplayTimeTracker.Menu;

public class PromptMenu : CustomMenu
{
    public enum PromptType
    {
        YesNo,
        Ok
    }

    public Grid ButtonsGrid;
    public TextBlock promptTextBlock;
    public PrefEntry dontShowAgainPref;

    public PromptMenu(
        string[] textArray, double[] sizeArray = null, bool[] boldArray = null, double lineSpacing = 5,
        double width = 300, PromptType type = PromptType.Ok, bool dontShowAgainQuestion = false,
        RoutedEventHandler yesHandler = null, RoutedEventHandler noHandler = null,
        bool toScale = true)
        : base(width, toScale)
    {
        sizeArray = (sizeArray ?? new double[0])
            .Concat(Enumerable.Repeat(Common.EditTitleFontSize,
                Math.Max(0, textArray.Length - (sizeArray?.Length ?? 0))))
            .ToArray();
        boldArray = Enumerable.Range(0, textArray.Length)
            .Select(i => boldArray != null && i < boldArray.Length && boldArray[i])
            .ToArray();

        promptTextBlock = new TextBlock
        {
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 20, 0, 0),
            Foreground = new SolidColorBrush(AppColors.Font),
            TextWrapping = TextWrapping.Wrap,
            Padding = new Thickness(10, 0, 10, 0),
        };

        for (int i = 0; i < textArray.Length; i++)
        {
            promptTextBlock.Inlines.Add(new Run
            {
                Text = textArray[i] + "\n",
                FontSize = sizeArray[i],
                FontWeight = boldArray[i] ? FontWeights.Bold : FontWeights.Regular
            });
            promptTextBlock.Inlines.Add(new Run
                { Text = ".\n", Foreground = Brushes.Transparent, FontSize = lineSpacing });
        }

        MenuContentPanel.Children.Add(promptTextBlock);

        if (dontShowAgainQuestion)
        {
            dontShowAgainPref = new PrefEntry("Don't show again", false, 220);
            dontShowAgainPref.Margin = new Thickness(10,-10,0,10);
            Binding dontShowBinding = new Binding("DontShowApiKeyPrompt")
                { Source = Common.Settings, Mode = BindingMode.TwoWay, };
            BindingOperations.SetBinding(dontShowAgainPref.toggleButton, CustomToggleButton.IsToggledProperty,
                dontShowBinding);
            BindingHelper.SetColorBinding(dontShowAgainPref.textBlock, ForegroundProperty, "Font");
            MenuContentPanel.Children.Add(dontShowAgainPref);
        }

        ButtonsGrid = new Grid { Height = 60 };
        if (type == PromptType.YesNo)
        {
            AddButton("Yes", 60, yesHandler, BType.Positive);
            AddButton("No", -60, noHandler, BType.Negative);
        }
        else
        {
            AddButton("Ok", 0, null, BType.Default);
        }

        MenuContentPanel.Children.Add(ButtonsGrid);
    }

    private void AddButton(string text, double horizontalMargin, RoutedEventHandler handler, BType buttonType)
    {
        var button = new CustomButton(text: text, w: 100, h: 35, hA: HorizontalAlignment.Center,
            vA: VerticalAlignment.Bottom, type: buttonType, effect: AppEffects.DropShadowMedium)
        {
            Margin = new Thickness(-horizontalMargin, 0, horizontalMargin, 20)
        };
        button.Click += (s, e) =>
        {
            (handler ?? ((s, e) => Console.WriteLine($"No Event set for {text} Button")))(s, e);
            Close();
        };
        ButtonsGrid.Children.Add(button);
    }
}