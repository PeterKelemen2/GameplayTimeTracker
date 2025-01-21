using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace GameplayTimeTracker.Menu;

public class AddMenu : CustomMenu
{
    public AddMenu(Entry entry, EntryRepository entryRepo, GameCardRepository cardRepo, Panel panel,
        double width = 350, double height = 550, bool performanceMode = true)
        : base(width, height, performanceMode)
    {
        double buttonSize = 20;
        double buttonMargin = (Common.TextBoxHeight - buttonSize) * 0.5;

        StackPanel stackPanel = new();
        MenuContentGrid.Children.Add(stackPanel);

        TextBlock titleTextBlock = UIHelper.CreateTextBlock(text: "Add New Entry", hA: HorizontalAlignment.Center,
            vA: VerticalAlignment.Center, margin: new Thickness(20), fontSize: Common.EditTitleFontSize, isBold: true);
        stackPanel.Children.Add(titleTextBlock);

        TextBlock generalTextBlock = UIHelper.CreateTextBlock("General", hA: HorizontalAlignment.Center, fontSize: 17);
        stackPanel.Children.Add(generalTextBlock);

        Grid nameGrid = UIHelper.CreateAddEntryGrid("Name", new Thickness(5, 0, 0, 30), binding: "Name", entry: entry);
        stackPanel.Children.Add(nameGrid);

        Grid exeGrid =
            UIHelper.CreateAddEntryGrid("Path", new Thickness(5, 10, 0, 30), binding: "ExePath", entry: entry);
        Common.FindTextBox(exeGrid).Padding = new Thickness(5, 0, buttonSize + buttonMargin * 2, 0);
        CustomButton exeBrowseButton = UIHelper.CreateBrowseButtonRB(buttonSize, buttonSize, buttonMargin);
        exeBrowseButton.Click += (_, _) =>
        {
            Common.FindTextBox(exeGrid).Text = Common.GetDialogPath(Common.exeFilter);
        };
        exeGrid.Children.Add(exeBrowseButton);
        stackPanel.Children.Add(exeGrid);

        Grid argsGrid = UIHelper.CreateAddEntryGrid("Arguments", new Thickness(5, 10, 0, 30), binding: "Arguments",
            entry: entry);
        stackPanel.Children.Add(argsGrid);

        TextBlock imagesTextBlock = UIHelper.CreateTextBlock("Images", hA: HorizontalAlignment.Center, fontSize: 17);
        imagesTextBlock.Margin = new Thickness(0, 20, 0, 0);
        stackPanel.Children.Add(imagesTextBlock);

        Grid iconGrid =
            UIHelper.CreateAddEntryGrid("Icon Path", new Thickness(5, 0, 0, 30), binding: "IconPath", entry: entry);
        Common.FindTextBox(iconGrid).Padding = new Thickness(5, 0, buttonSize + buttonMargin * 2, 0);
        CustomButton iconBrowseButton = UIHelper.CreateBrowseButtonRB(buttonSize, buttonSize, buttonMargin);
        iconBrowseButton.Click += (_, _) =>
        {
            Common.FindTextBox(iconGrid).Text = Common.GetDialogPath(Common.imageFilter);
        };
        iconGrid.Children.Add(iconBrowseButton);
        stackPanel.Children.Add(iconGrid);

        Grid heroGrid = UIHelper.CreateAddEntryGrid("Hero Path", new Thickness(5, 10, 0, 30), binding: "HeroPath",
            entry: entry);
        Common.FindTextBox(heroGrid).Padding = new Thickness(5, 0, buttonSize + buttonMargin * 2, 0);
        CustomButton heroBrowseButton = UIHelper.CreateBrowseButtonRB(buttonSize, buttonSize, buttonMargin);
        heroBrowseButton.Click += (_, _) =>
        {
            Common.FindTextBox(heroGrid).Text = Common.GetDialogPath(Common.imageFilter);
        };
        heroGrid.Children.Add(heroBrowseButton);
        stackPanel.Children.Add(heroGrid);

        CustomButton fetchButton = new CustomButton(w: 110, text: "Get Images", effect: AppEffects.dropShadowIcon);
        fetchButton.Margin = new Thickness(0, 10, 0, 10);
        stackPanel.Children.Add(fetchButton);

        CustomButton saveButton =
            new CustomButton(w: 110, h: 40, text: "Add Entry", effect: AppEffects.dropShadowIcon,
                type: BType.Positive);
        saveButton.Margin = new Thickness(0, 20, 0, 10);
        saveButton.Click += (_, _) => { AddConfiguredEntry(entry, entryRepo, cardRepo, panel); };
        stackPanel.Children.Add(saveButton);
    }

    private void AddConfiguredEntry(Entry entry, EntryRepository entryRepo, GameCardRepository cardRepo, Panel panel)
    {
        entryRepo.AddEntry(entry);
        GameCard gc = new GameCardVertical(entry, entryRepo, cardRepo, panel);
        cardRepo.GameCards.Add(gc);
        panel.Children.Add(gc);
    }
}