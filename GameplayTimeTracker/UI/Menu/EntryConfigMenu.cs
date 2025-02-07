using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GameplayTimeTracker.Menu;

public class EntryConfigMenu : CustomMenu
{
    public ScrollViewer scrollViewer { get; set; }
    public StackPanel stackPanel { get; set; }
    public TextBlock TitleTextBlock { get; set; }
    public CustomButton ConfirmButton { get; set; }
    public TextBox ExeBox { get; set; }
    public TextBox IconBox { get; set; }
    public TextBox HeroBox { get; set; }
    public Entry _entry { get; set; }

    public EntryConfigMenu(Entry entry,
        double width = 350, bool toScale = true)
        : base(width, toScale)
    {
        _entry = entry;
        ToScale = toScale;
        double buttonSize = 20;
        double buttonMargin = (Common.TextBoxHeight - buttonSize) * 0.5;

        scrollViewer = new ScrollViewer { Height = 600, VerticalScrollBarVisibility = ScrollBarVisibility.Hidden };
        stackPanel = new();
        scrollViewer.Content = stackPanel;
        MenuContentPanel.Children.Add(scrollViewer);

        TitleTextBlock = UIHelper.CreateTextBlock(text: "Configure Entry", hA: HorizontalAlignment.Center,
            vA: VerticalAlignment.Center, margin: new Thickness(20), fontSize: Common.EditTitleFontSize, isBold: true);
        BindingHelper.SetColorBinding(TitleTextBlock, ForegroundProperty, "Font");
        stackPanel.Children.Add(TitleTextBlock);

        CreateTitleBlock("General");

        Grid nameGrid = UIHelper.CreateAddEntryGrid(Settings, "Name", new Thickness(5, 0, 0, 30));
        var nameTextBox = Common.FindTextBox(nameGrid);
        Binding nameBinding = new Binding("Name")
            { Source = _entry, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
        BindingOperations.SetBinding(nameTextBox, TextBox.TextProperty, nameBinding);
        nameTextBox.GotFocus += (s, e) => { _entry.IsEditing = true; };
        nameTextBox.LostFocus += (s, e) => { _entry.IsEditing = false; };
        stackPanel.Children.Add(nameGrid);

        Grid timeGrid = UIHelper.CreateAddEntryGrid(Settings, "Playtime", new Thickness(5, 10, 0, 30));
        var timeBox = Common.FindTextBox(timeGrid);
        timeBox.Text =
            new TimeArrayConverter().Convert(entry.TotalPlay, typeof(string), null, CultureInfo.InvariantCulture) as
                string;
        Binding timeBinding = new Binding("TotalPlay")
        {
            Source = entry,
            Mode = BindingMode.TwoWay,
            Converter = new TimeArrayConverter(),
        };
        BindingOperations.SetBinding(timeBox, TextBox.TextProperty, timeBinding);

        timeBox.GotFocus += (s, e) =>
        {
            BindingOperations.ClearBinding(timeBox, TextBox.TextProperty);
            timeBox.Text = entry.TotalPlayFormatted;
        };

        timeBox.LostFocus += (s, e) =>
        {
            entry.TotalPlay =
                new TimeArrayConverter().ConvertBack(timeBox.Text, typeof(int[]), null, CultureInfo.InvariantCulture) as
                    int[];
            BindingOperations.SetBinding(timeBox, TextBox.TextProperty, timeBinding);
        };
        stackPanel.Children.Add(timeGrid);

        Grid exeGrid = UIHelper.CreateAddEntryGrid(Settings, "Path", new Thickness(5, 10, 0, 30));
        ExeBox = Common.FindTextBox(exeGrid);
        ExeBox.Padding = new Thickness(5, 0, buttonSize + buttonMargin * 2, 0);
        Binding exeBinding = new Binding("ExePath") { Source = entry, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(ExeBox, TextBox.TextProperty, exeBinding);
        CustomButton exeBrowseButton = UIHelper.CreateBrowseButtonRB(buttonSize, buttonSize, buttonMargin);
        exeBrowseButton.Click += ExeBrowse_Click;
        exeGrid.Children.Add(exeBrowseButton);
        stackPanel.Children.Add(exeGrid);

        CreateEditEntry("Arguments", "Arguments");

        TextBlock remoteTitleBlock =
            UIHelper.CreateTextBlock("Remote Backup", hA: HorizontalAlignment.Center, fontSize: 17);
        remoteTitleBlock.Margin = new Thickness(0, 15, 0, 0);
        BindingHelper.SetColorBinding(remoteTitleBlock, ForegroundProperty, "Font");
        stackPanel.Children.Add(remoteTitleBlock);

        Grid savePathGrid = UIHelper.CreateAddEntryGrid(Settings, "Local Save Path", new Thickness(5, 0, 0, 30));
        var saveBox = Common.FindTextBox(savePathGrid);
        Binding savePathBinding = new Binding("LocalSavePath") { Source = entry, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(saveBox, TextBox.TextProperty, savePathBinding);
        saveBox.Padding = new Thickness(5, 0, buttonSize + buttonMargin * 2, 0);
        CustomButton savePathBrowseButton = UIHelper.CreateBrowseButtonRB(buttonSize, buttonSize, buttonMargin);
        savePathBrowseButton.Click += (_, _) =>
        {
            string newPath = Common.GetFolderDialogPath();
            if (!string.IsNullOrEmpty(newPath))
            {
                saveBox.Text = newPath;
                entry.IconPath = newPath;
            }
        };
        savePathGrid.Children.Add(savePathBrowseButton);
        stackPanel.Children.Add(savePathGrid);

        PrefEntry remoteSavePref =
            new PrefEntry("Remote Backup", Common.Settings.PreferSteamGridDBImage, width: 220,
                description: "On session end");
        Binding remoteSavePrefBinding = new Binding("IsRemoteSaveEnabled")
            { Source = _entry, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(remoteSavePref.toggleButton, CustomToggleButton.IsToggledProperty,
            remoteSavePrefBinding);
        BindingHelper.SetColorBinding(remoteSavePref.textBlock, ForegroundProperty, "Font");
        stackPanel.Children.Add(remoteSavePref);

        Panel remoteButtonsContainer = new WrapPanel
            { HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 10, 0, 15) };
        CustomButton uploadButton = new CustomButton(w: 110, h: 40, text: "Upload", effect: AppEffects.DropShadowIcon);
        uploadButton.Margin = new Thickness(0, 0, 5, 0);
        uploadButton.Click += (_, _) => { entry.RemoteSave(); };

        CustomButton loadButton =
            new CustomButton(w: 110, h: 40, text: "Load Latest", effect: AppEffects.DropShadowIcon);
        loadButton.Margin = new Thickness(5, 0, 0, 0);
        loadButton.Click += (_, _) => { entry.RemoteLoad(); };
        remoteButtonsContainer.Children.Add(uploadButton);
        remoteButtonsContainer.Children.Add(loadButton);
        stackPanel.Children.Add(remoteButtonsContainer);

        TextBlock imagesTextBlock = UIHelper.CreateTextBlock("Images", hA: HorizontalAlignment.Center, fontSize: 17);
        imagesTextBlock.Margin = new Thickness(0, 0, 0, 0);
        BindingHelper.SetColorBinding(imagesTextBlock, ForegroundProperty, "Font");
        stackPanel.Children.Add(imagesTextBlock);

        Grid iconGrid = UIHelper.CreateAddEntryGrid(Settings, "Icon Path", new Thickness(5, 0, 0, 30));
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

        Grid heroGrid = UIHelper.CreateAddEntryGrid(Settings, "Hero Path", new Thickness(5, 10, 0, 30));
        heroGrid.Margin = new Thickness(0, 0, 0, 15);
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
            new CustomButton(w: 120, h: 40, text: "Finish", effect: AppEffects.DropShadowIcon,
                type: BType.Positive);
        ConfirmButton.Margin = new Thickness(0, 20, 0, 10);
    }


    private void ExeBrowse_Click(object sender, RoutedEventArgs e)
    {
        string newPath = Common.GetDialogPath(Common.exeFilter);
        if (!newPath.Equals(""))
        {
            ExeBox.Text = newPath;
            _entry.ExePath = newPath;
        }
    }

    private void CreateEditEntry(string title, string bindingPath)
    {
        Grid grid = UIHelper.CreateAddEntryGrid(Settings, title, new Thickness(5, 10, 0, 30));
        var textBox = Common.FindTextBox(grid);
        Binding binding = new Binding(bindingPath)
            { Source = _entry, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
        BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
        stackPanel.Children.Add(grid);
    }

    public void CreateTitleBlock(string title)
    {
        TextBlock textBlock = UIHelper.CreateTextBlock(title, hA: HorizontalAlignment.Center, fontSize: 17);
        BindingHelper.SetColorBinding(textBlock, ForegroundProperty, "Font");
        stackPanel.Children.Add(textBlock);
    }
}