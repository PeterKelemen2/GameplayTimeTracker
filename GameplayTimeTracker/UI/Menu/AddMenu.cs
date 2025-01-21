using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Accessibility;


namespace GameplayTimeTracker.Menu;

public class AddMenu : CustomMenu
{
    public AddMenu(double width = 350, double height = 550, bool performanceMode = true)
        : base(width, height, performanceMode)
    {
        double buttonSize = 20;
        double buttonMargin = (Common.TextBoxHeight - buttonSize) * 0.5;

        StackPanel stackPanel = new();
        MenuContentGrid.Children.Add(stackPanel);

        TextBlock titleTextBlock = new TextBlock
        {
            Text = "Add New Entry",
            FontSize = Common.EditTitleFontSize,
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(AppColors.Font),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(20),
            Effect = AppEffects.DropShadowMedium
        };
        stackPanel.Children.Add(titleTextBlock);

        TextBlock generalTextBlock = UIHelper.CreateTextBlock("General", hA: HorizontalAlignment.Center, fontSize: 17);
        stackPanel.Children.Add(generalTextBlock);

        Grid nameGrid = CreateEntryGrid("Name", new Thickness(5, 0, 0, 30));
        stackPanel.Children.Add(nameGrid);

        Grid exeGrid = CreateEntryGrid("Path", new Thickness(5, 10, 0, 30));
        FindTextBox(exeGrid).Padding = new Thickness(5, 0, buttonSize + buttonMargin * 2, 0);
        CustomButton exeBrowseButton = CreateBrowseButton(buttonSize, buttonSize, buttonMargin);
        exeBrowseButton.Click += (_, _) => { FindTextBox(exeGrid).Text = Common.GetDialogPath(Common.exeFilter); };
        exeGrid.Children.Add(exeBrowseButton);
        stackPanel.Children.Add(exeGrid);

        Grid argsGrid = CreateEntryGrid("Arguments", new Thickness(5, 10, 0, 30));
        stackPanel.Children.Add(argsGrid);

        TextBlock imagesTextBlock = UIHelper.CreateTextBlock("Images", hA: HorizontalAlignment.Center, fontSize: 17);
        imagesTextBlock.Margin = new Thickness(0, 20, 0, 0);
        stackPanel.Children.Add(imagesTextBlock);

        Grid iconGrid = CreateEntryGrid("Icon Path", new Thickness(5, 0, 0, 30));
        FindTextBox(iconGrid).Padding = new Thickness(5, 0, buttonSize + buttonMargin * 2, 0);
        CustomButton iconBrowseButton = CreateBrowseButton(buttonSize, buttonSize, buttonMargin);
        iconBrowseButton.Click += (_, _) => { FindTextBox(iconGrid).Text = Common.GetDialogPath(Common.imageFilter); };
        iconGrid.Children.Add(iconBrowseButton);
        stackPanel.Children.Add(iconGrid);

        Grid heroGrid = CreateEntryGrid("Hero Path", new Thickness(5, 10, 0, 30));
        FindTextBox(heroGrid).Padding = new Thickness(5, 0, buttonSize + buttonMargin * 2, 0);
        CustomButton heroBrowseButton = CreateBrowseButton(buttonSize, buttonSize, buttonMargin);
        heroBrowseButton.Click += (_, _) => { FindTextBox(heroGrid).Text = Common.GetDialogPath(Common.imageFilter); };
        heroGrid.Children.Add(heroBrowseButton);
        stackPanel.Children.Add(heroGrid);

        CustomButton fetchButton = new CustomButton(width: 110, text: "Get Images", effect: AppEffects.dropShadowIcon);
        fetchButton.Margin = new Thickness(0, 10, 0, 10);
        stackPanel.Children.Add(fetchButton);

        CustomButton saveButton =
            new CustomButton(width: 110, height: 40, text: "Add Entry", effect: AppEffects.dropShadowIcon,
                type: CustomButton.ButtonType.Positive);
        saveButton.Margin = new Thickness(0, 20, 0, 10);
        stackPanel.Children.Add(saveButton);
    }

    private Grid CreateEntryGrid(string text, Thickness textMargin, string boxText = "", double boxWidth = 220)
    {
        Grid grid = new Grid { HorizontalAlignment = HorizontalAlignment.Center };
        TextBlock textBlock = UIHelper.CreateTextBlock(text, margin: textMargin);
        TextBox textBox = UIHelper.CreateTextBox(boxText, width: boxWidth);
        grid.Children.Add(textBlock);
        grid.Children.Add(textBox);
        return grid;
    }

    private CustomButton CreateBrowseButton(double w, double h, double m)
    {
        CustomButton button = new CustomButton(width: w, height: h, borderRadius: 3,
            hA: HorizontalAlignment.Right, vA: VerticalAlignment.Bottom, buttonImagePath: AppFiles.FolderIcon);
        button.Margin = new Thickness(0, 0, m, m);
        return button;
    }

    private TextBox FindTextBox(Grid parentGrid)
    {
        foreach (UIElement child in parentGrid.Children)
        {
            if (child is TextBox textBox) return textBox;
        }

        return null;
    }
}