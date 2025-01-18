using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GameplayTimeTracker.Menu;

public class EditMenu : CustomMenu
{
    private Entry Entry;
    private StackPanel StackPanel;
    private double LeftMargin = 30;
    private double bWidth = 140;
    private double bHeight = 35;


    public EditMenu(Entry entry, double width = 400, double height = 400, bool performanceMode = true) : base(width,
        height, performanceMode)
    {
        Entry = entry;

        StackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Top,
        };
        MenuContentGrid.Children.Add(StackPanel);

        TextBlock title = new TextBlock
        {
            Text = "Editing ",
            FontSize = Common.EditTitleFontSize,
            FontWeight = FontWeights.Regular,
            Foreground = new SolidColorBrush(AppColors.Font),
            TextWrapping = TextWrapping.Wrap,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(20),
            Effect = AppEffects.DropShadowMedium
        };
        title.Inlines.Add(new Run { Text = Entry.Name, FontWeight = FontWeights.Bold });
        StackPanel.Children.Add(title);

        Grid gridName = new Grid
            { Width = MenuContentBg.Width, Height = bHeight + 25, HorizontalAlignment = HorizontalAlignment.Left };
        TextBlock editNameTextBlock = CreateTextBlock("Name");
        TextBox editNameTextBox = CreateTextBox(Entry.Name);
        var BrowseExeButton = new CustomButton(text: "New exe", width: bWidth, height: bHeight,
            buttonImagePath: AppFiles.AddIcon,
            hA: HorizontalAlignment.Right, vA: VerticalAlignment.Bottom);
        BrowseExeButton.Effect = AppEffects.DropShadowMedium;
        BrowseExeButton.Margin = new Thickness(0, 5, LeftMargin, 5);
        BrowseExeButton.Click += (_, _) => { EditHelper.UpdateExe(entry); };
        gridName.Children.Add(editNameTextBlock);
        gridName.Children.Add(editNameTextBox);
        gridName.Children.Add(BrowseExeButton);
        StackPanel.Children.Add(gridName);

        Grid gridTime = new Grid
            { Width = MenuContentBg.Width, Height = bHeight + 25, HorizontalAlignment = HorizontalAlignment.Left };
        TextBlock editTimeTextBlock = CreateTextBlock("Playtime");
        TextBox editTimeTextBox = CreateTextBox(Entry.TotalPlayFormatted);
        var OpenFolderButton = new CustomButton(text: "Open Folder", width: bWidth, height: bHeight,
            buttonImagePath: AppFiles.FolderIcon,
            hA: HorizontalAlignment.Right, vA: VerticalAlignment.Bottom);
        OpenFolderButton.Effect = AppEffects.DropShadowMedium;
        OpenFolderButton.Margin = new Thickness(0, 5, LeftMargin, 5);
        OpenFolderButton.Click += (s, e) => { Process.Start("explorer.exe", $"/select,\"{Entry.ExePath}\""); };
        gridTime.Children.Add(editTimeTextBlock);
        gridTime.Children.Add(editTimeTextBox);
        gridTime.Children.Add(OpenFolderButton);
        StackPanel.Children.Add(gridTime);

        Grid gridPath = new Grid
            { Width = MenuContentBg.Width, Height = bHeight + 25, HorizontalAlignment = HorizontalAlignment.Left };
        TextBlock editPathTextBlock = CreateTextBlock("Path");
        TextBox editPathTextBox = CreateTextBox(Entry.ExePath);
        var ChangeIconButton = new CustomButton(text: "Change Icon", width: bWidth, height: bHeight,
            buttonImagePath: AppFiles.EditIcon,
            hA: HorizontalAlignment.Right, vA: VerticalAlignment.Bottom);
        ChangeIconButton.Margin = new Thickness(0, 5, LeftMargin, 5);
        ChangeIconButton.Effect = AppEffects.DropShadowMedium;
        // ChangeIconButton.Click += Parent.UpdateIcons;
        gridPath.Children.Add(editPathTextBlock);
        gridPath.Children.Add(editPathTextBox);
        gridPath.Children.Add(ChangeIconButton);
        StackPanel.Children.Add(gridPath);

        Grid gridArguments = new Grid
            { Width = MenuContentBg.Width, Height = bHeight + 25, HorizontalAlignment = HorizontalAlignment.Left };
        TextBlock editArgsTextBlock = CreateTextBlock("Arguments");
        TextBox editArgsTextBox = CreateTextBox(Entry.Arguments);
        var ChangeHeroButton = new CustomButton(text: "Change Hero", width: bWidth, height: bHeight,
            buttonImagePath: AppFiles.EditIcon,
            hA: HorizontalAlignment.Right, vA: VerticalAlignment.Bottom);
        ChangeHeroButton.Margin = new Thickness(0, 5, LeftMargin, 5);
        ChangeHeroButton.Effect = AppEffects.DropShadowMedium;
        // SaveButton.Click += Parent.editSaveButton_Click;
        gridArguments.Children.Add(editArgsTextBlock);
        gridArguments.Children.Add(editArgsTextBox);
        gridArguments.Children.Add(ChangeHeroButton);
        StackPanel.Children.Add(gridArguments);

        var SaveButton = new CustomButton(text: "Save", width: bWidth, height: bHeight,
            buttonImagePath: AppFiles.SaveIcon, type: CustomButton.ButtonType.Positive,
            hA: HorizontalAlignment.Center, vA: VerticalAlignment.Top);
        SaveButton.Effect = AppEffects.DropShadowMedium;
        SaveButton.Margin = new Thickness(0, LeftMargin, 0, 20);
        StackPanel.Children.Add(SaveButton);
    }

    private TextBox CreateTextBox(string text = "", HorizontalAlignment hA = HorizontalAlignment.Left,
        VerticalAlignment vA = VerticalAlignment.Bottom)
    {
        TextBox sample = new TextBox
        {
            Text = text,
            Width = 180,
            Height = Common.TextBoxHeight,
            HorizontalAlignment = hA,
            VerticalAlignment = vA,
            TextAlignment = TextAlignment.Left,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalContentAlignment = VerticalAlignment.Center,
            Effect = AppEffects.dropShadowIcon,
            Margin = new Thickness(LeftMargin, 0, 0, 5)
        };
        sample.Style = (Style)Application.Current.FindResource("RoundedTextBox");

        return sample;
    }

    private TextBlock CreateTextBlock(string text = "", HorizontalAlignment hA = HorizontalAlignment.Left,
        VerticalAlignment vA = VerticalAlignment.Top, bool isBold = true)
    {
        var sampleTextBlock = new TextBlock
        {
            Text = text,
            FontWeight = isBold ? FontWeights.Bold : FontWeights.Regular,
            FontSize = Common.TextFontSize,
            Foreground = new SolidColorBrush(AppColors.Font),
            HorizontalAlignment = hA,
            VerticalAlignment = vA,
            Margin =
                new Thickness(LeftMargin + 5, 5, 0, 5),
            Effect = AppEffects.dropShadowText,
        };

        return sampleTextBlock;
    }
}