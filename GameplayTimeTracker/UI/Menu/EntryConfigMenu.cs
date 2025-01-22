using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GameplayTimeTracker.Menu;

public class EntryConfigMenu : CustomMenu
{
    public StackPanel stackPanel { get; set; }
    public TextBlock TitleTextBlock { get; set; }
    public CustomButton ConfirmButton { get; set; }
    public TextBox ExeBox { get; set; }
    public TextBox IconBox { get; set; }
    public TextBox HeroBox { get; set; }

    public EntryConfigMenu(Entry entry,
        double width = 350, double height = 550, bool performanceMode = true)
        : base(width, height, performanceMode)
    {
        double buttonSize = 20;
        double buttonMargin = (Common.TextBoxHeight - buttonSize) * 0.5;

        stackPanel = new();
        MenuContentGrid.Children.Add(stackPanel);

        TitleTextBlock = UIHelper.CreateTextBlock(text: "Configure Entry", hA: HorizontalAlignment.Center,
            vA: VerticalAlignment.Center, margin: new Thickness(20), fontSize: Common.EditTitleFontSize, isBold: true);
        stackPanel.Children.Add(TitleTextBlock);

        TextBlock generalTextBlock = UIHelper.CreateTextBlock("General", hA: HorizontalAlignment.Center, fontSize: 17);
        stackPanel.Children.Add(generalTextBlock);

        Grid nameGrid = UIHelper.CreateAddEntryGrid("Name", new Thickness(5, 0, 0, 30));
        var nameBox = Common.FindTextBox(nameGrid);
        Binding nameBinding = new Binding("Name")
            { Source = entry, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
        BindingOperations.SetBinding(nameBox, TextBox.TextProperty, nameBinding);
        stackPanel.Children.Add(nameGrid);

        Grid timeGrid = UIHelper.CreateAddEntryGrid("Playtime", new Thickness(5, 0, 0, 30));
        var timeBox = Common.FindTextBox(timeGrid);
        Binding timeBinding = new Binding("TotalPlay")
        {
            Source = entry,
            Mode = BindingMode.TwoWay,
            Converter = new TimeArrayConverter()
        };
        BindingOperations.SetBinding(timeBox, TextBox.TextProperty, timeBinding);
        stackPanel.Children.Add(timeGrid);

        Grid exeGrid = UIHelper.CreateAddEntryGrid("Path", new Thickness(5, 10, 0, 30));
        ExeBox = Common.FindTextBox(exeGrid);
        ExeBox.Padding = new Thickness(5, 0, buttonSize + buttonMargin * 2, 0);
        Binding exeBinding = new Binding("ExePath") { Source = entry, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(ExeBox, TextBox.TextProperty, exeBinding);
        CustomButton exeBrowseButton = UIHelper.CreateBrowseButtonRB(buttonSize, buttonSize, buttonMargin);
        exeBrowseButton.Click += (_, _) => { ExeBox.Text = Common.GetDialogPath(Common.exeFilter); };
        exeGrid.Children.Add(exeBrowseButton);
        stackPanel.Children.Add(exeGrid);

        Grid argsGrid = UIHelper.CreateAddEntryGrid("Arguments", new Thickness(5, 10, 0, 30));
        stackPanel.Children.Add(argsGrid);
        var argsBox = Common.FindTextBox(argsGrid);
        Binding argsBinding = new Binding("Arguments") { Source = entry, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(argsBox, TextBox.TextProperty, argsBinding);


        TextBlock imagesTextBlock = UIHelper.CreateTextBlock("Images", hA: HorizontalAlignment.Center, fontSize: 17);
        imagesTextBlock.Margin = new Thickness(0, 20, 0, 0);
        stackPanel.Children.Add(imagesTextBlock);

        Grid iconGrid = UIHelper.CreateAddEntryGrid("Icon Path", new Thickness(5, 0, 0, 30));
        var iconBox = Common.FindTextBox(iconGrid);
        Binding iconPathBinding = new Binding("IconPath") { Source = entry, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(iconBox, TextBox.TextProperty, iconPathBinding);
        iconBox.Padding = new Thickness(5, 0, buttonSize + buttonMargin * 2, 0);
        CustomButton iconBrowseButton = UIHelper.CreateBrowseButtonRB(buttonSize, buttonSize, buttonMargin);
        iconBrowseButton.Click += (_, _) =>
        {
            string newPath = Common.GetDialogPath(Common.imageFilter);
            if (!newPath.Equals(""))
            {
                iconBox.Text = newPath;
                entry.IconPath = newPath;
            }
        };
        iconGrid.Children.Add(iconBrowseButton);
        stackPanel.Children.Add(iconGrid);

        Grid heroGrid = UIHelper.CreateAddEntryGrid("Hero Path", new Thickness(5, 10, 0, 30));
        var heroBox = Common.FindTextBox(heroGrid);
        Binding heroPathBinding = new Binding("HeroPath") { Source = entry, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(heroBox, TextBox.TextProperty, heroPathBinding);
        heroBox.Padding = new Thickness(5, 0, buttonSize + buttonMargin * 2, 0);
        CustomButton heroBrowseButton = UIHelper.CreateBrowseButtonRB(buttonSize, buttonSize, buttonMargin);
        heroBrowseButton.Click += (_, _) =>
        {
            string newPath = Common.GetDialogPath(Common.imageFilter);
            if (!newPath.Equals(""))
            {
                heroBox.Text = newPath;
                entry.HeroPath = newPath;
            }
        };
        heroGrid.Children.Add(heroBrowseButton);
        stackPanel.Children.Add(heroGrid);

        ConfirmButton =
            new CustomButton(w: 120, h: 40, text: "Finish", effect: AppEffects.dropShadowIcon,
                type: BType.Positive);
        ConfirmButton.Margin = new Thickness(0, 20, 0, 10);
        // ConfirmButton.Click += (_, _) => { AddConfiguredEntry(entry, entryRepo, cardRepo, panel); };
        stackPanel.Children.Add(ConfirmButton);
    }
}