using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GameplayTimeTracker.Menu.Content;
using GameplayTimeTracker.UI.Menu.Content;

namespace GameplayTimeTracker.Menu;

public class EntryConfigMenu : CustomMenu
{
    public ScrollViewer scrollViewer { get; set; }
    public StackPanel stackPanel { get; set; }
    public StackPanel remoteStackPanel { get; set; }
    PrefEntry remoteSavePref { get; set; }


    public TextBlock TitleTextBlock { get; set; }
    public TextBlock GeneralTitleTextBlock { get; set; }
    public CustomButton ConfirmButton { get; set; }
    public TextBox ExeBox { get; set; }
    public TextBox IconBox { get; set; }
    public TextBox HeroBox { get; set; }
    public Entry _entry { get; set; }
    static double buttonSize = 20;
    static double buttonMargin = (Common.TextBoxHeight - buttonSize) * 0.5;

    public EntryConfigMenu(Entry entry,
        double width = 350, bool toScale = true)
        :
        base(width, toScale)
    {
        _entry = entry;
        ToScale = toScale;

        stackPanel = new();
        MenuContentPanel.Children.Add(stackPanel);

        TitleTextBlock =
            CreateTitleBlock(stackPanel, "Configure Entry", margin: new Thickness(20),
                fontSize: Common.EditTitleFontSize);

        GeneralTitleTextBlock = CreateTitleBlock(stackPanel, "General", margin: new Thickness(0, 0, 0, 0));

        CreateEditEntry(stackPanel, "Name", "Name", bindName: true, first: true,
            updateSourceTrigger: UpdateSourceTrigger.PropertyChanged);

        string? timeBoxText = new TimeArrayConverter().Convert(entry.TotalPlay) as string;
        CreateEditEntry(stackPanel, "Playtime", "TotalPlay", textBoxText: timeBoxText, bindTime: true);
        CreateEditEntry(stackPanel, "Path", "ExePath", buttonClick: ExeBrowse_Click);
        CreateEditEntry(stackPanel, "Arguments", "Arguments");

        remoteStackPanel = new();
        TextBlock remoteTitleBlock = CreateTitleBlock(remoteStackPanel, "Remote Backup",
            margin: new Thickness(0, 15, 0, 0), fontSize: 21);

        CreateEditEntry(remoteStackPanel, "Local Save Path", "LocalSavePath", buttonClick: SavePath_Click,
            updateSourceTrigger: UpdateSourceTrigger.PropertyChanged);

        remoteSavePref =
            new PrefEntry("Remote Backup", Common.Settings.IsRemoteSavingEnabled, width: 220,
                description: "On session end", margin: new Thickness(0, 0, 0, 20));
        Binding remoteSavePrefBinding = new Binding("IsRemoteSaveEnabled")
            { Source = _entry, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(remoteSavePref.toggleButton, CustomToggleButton.IsToggledProperty,
            remoteSavePrefBinding);
        BindingHelper.SetColorBinding(remoteSavePref.textBlock, ForegroundProperty, "Font");
        remoteSavePref.toggleButton.ToggledChanged += (state) =>
        {
            RemoteSavePrefToggledChanged(remoteSavePref.toggleButton.IsToggled);
        };

        CreateEditEntry(remoteStackPanel, "Save if session longer (m)", "RemoteSaveAfterMinutes");

        remoteStackPanel.Children.Add(remoteSavePref);

        stackPanel.Children.Add(remoteStackPanel);
    }

    private void RemoteSavePrefToggledChanged(bool buttonToggled)
    {
        if (!Common.Settings.IsRemoteSavingEnabled && buttonToggled)
        {
            PromptMenu turnOnBackup = new PromptMenu(new[]
                    { "Remote saving is disabled in the settings.", "Do you want to enable it?" },
                boldArray: new[] { false, true },
                type: PromptMenu.PromptType.YesNo, toScale: false,
                yesHandler: (_, _) =>
                {
                    if (!Common.Settings.RemoteMachine.IsRemoteMachineConfigured())
                    {
                        PromptMenu remoteConfigPrompt = new PromptMenu(
                            new[]
                            {
                                "Remote machine is not properly configured.",
                                "Do you want to configure it now?"
                            },
                            boldArray: new[] { false, true },
                            type: PromptMenu.PromptType.YesNo, toScale: false,
                            yesHandler: (_, _) =>
                            {
                                SettingsMenu sm = new SettingsMenu(toScale: false);
                                sm.Open();
                                sm.SetMenu<RemoteMenu>(sm.RemoteBlock);
                            }
                        );
                        remoteConfigPrompt.Open();
                    }
                    else
                    {
                        Common.Settings.IsRemoteSavingEnabled = true;
                    }
                },
                noHandler: (_, _) => { remoteSavePref.toggleButton.IsToggled = false; });
            turnOnBackup.Open();
        }
    }

    private void SavePath_Click(object sender, RoutedEventArgs e)
    {
        string newPath = Common.GetFolderDialogPath();
        if (!string.IsNullOrEmpty(newPath)) _entry.LocalSavePath = newPath;
    }

    private void ExeBrowse_Click(object sender, RoutedEventArgs e)
    {
        string newPath = Common.GetDialogPath(Common.exeFilter);
        if (!newPath.Equals("")) _entry.ExePath = newPath;
    }

    public void CreateEditEntry(Panel parent, string text, string bindPath, string textBoxText = "",
        RoutedEventHandler buttonClick = null,
        bool bindTime = false, bool bindName = false, bool first = false,
        UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default)
    {
        Grid grid = UIHelper.CreateAddEntryGrid(Settings, text, new Thickness(5, first ? 0 : 10, 0, 30));
        TextBox textBox = Common.FindTextBox(grid);
        if (!string.IsNullOrEmpty(textBoxText))
        {
            textBox.Text = textBoxText;
        }

        textBox.Padding = new Thickness(5, 0, buttonSize + buttonMargin * 2, 0);
        Binding textBoxBinding = new Binding(bindPath)
            { Source = _entry, Mode = BindingMode.TwoWay, UpdateSourceTrigger = updateSourceTrigger };
        BindingOperations.SetBinding(textBox, TextBox.TextProperty, textBoxBinding);

        if (buttonClick != null)
        {
            CustomButton browseButton = UIHelper.CreateBrowseButtonRB(buttonSize, buttonSize, buttonMargin);
            browseButton.Click += buttonClick;
            grid.Children.Add(browseButton);
        }

        if (bindTime) AssignFocusHandlersTime(textBox);

        if (bindName) AssignFocusHandlersName(textBox);

        parent.Children.Add(grid);
    }

    private void AssignFocusHandlersName(TextBox textBox)
    {
        textBox.GotFocus += (s, e) => { _entry.IsEditing = true; };
        textBox.LostFocus += (s, e) => { _entry.IsEditing = false; };
    }

    private void AssignFocusHandlersTime(TextBox textBox)
    {
        Binding timeBinding = new Binding("TotalPlay")
        {
            Source = _entry,
            Mode = BindingMode.TwoWay,
            Converter = new TimeArrayConverter(),
        };
        BindingOperations.SetBinding(textBox, TextBox.TextProperty, timeBinding);

        int[] prevTime = { 0, 0, 0 };
        textBox.GotFocus += (s, e) =>
        {
            BindingOperations.ClearBinding(textBox, TextBox.TextProperty);
            textBox.Text = new TimeArrayConverter().Convert(_entry.TotalPlay) as string ?? string.Empty;
            prevTime = _entry.TotalPlay;
        };

        textBox.LostFocus += (s, e) =>
        {
            var textBoxTime = new TimeArrayConverter().ConvertBack(textBox.Text) as int[];

            if (!Common.TimeArraysEqual(textBoxTime, prevTime)) _entry.TotalPlay = textBoxTime;

            BindingOperations.SetBinding(textBox, TextBox.TextProperty, timeBinding);
        };
    }

    public TextBlock CreateTitleBlock(Panel parent, string title, double fontSize = 17, Thickness margin = new(),
        bool isBold = true,
        bool toAdd = true)
    {
        TextBlock textBlock =
            UIHelper.CreateTextBlock(title, hA: HorizontalAlignment.Center, fontSize: fontSize, margin: margin,
                isBold: isBold);
        BindingHelper.SetColorBinding(textBlock, ForegroundProperty, "Font");

        if (toAdd) parent.Children.Add(textBlock);

        return textBlock;
    }
}