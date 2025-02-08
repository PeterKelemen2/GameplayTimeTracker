using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace GameplayTimeTracker.Menu;

public class EditMenu : EntryConfigMenu
{
    static double buttonSize = 20;
    static double buttonMargin = (Common.TextBoxHeight - buttonSize) * 0.5;

    public EditMenu(Entry entry,
        double width = 350, bool toScale = true)
        : base(entry, width, toScale)
    {
        MenuContentPanel.Orientation = Orientation.Horizontal;
        MenuContentPanel.Width = Double.NaN;

        MenuContentPanel.Children.Remove(stackPanel);
        stackPanel.Width = width;

        StackPanel leftSide = stackPanel;
        StackPanel rightSide = new StackPanel { Width = width };

        Rectangle separator = new Rectangle
            { Width = 1, RadiusX = 2.5, RadiusY = 2.5 };
        BindingHelper.SetColorBinding(separator, Shape.FillProperty, "Font");
        MenuContentPanel.SizeChanged += (s, e) => { separator.Height = MenuContentPanel.RenderSize.Height - 40; };

        MenuContentPanel.Children.Add(leftSide);
        MenuContentPanel.Children.Add(separator);
        MenuContentPanel.Children.Add(rightSide);

        ToScale = toScale;
        TitleTextBlock.FontWeight = FontWeights.Regular;
        TitleTextBlock.Text = "Editing ";
        TitleTextBlock.Inlines.Add(new Run { Text = entry.Name, FontWeight = FontWeights.Bold });
        TitleTextBlock.TextTrimming = TextTrimming.CharacterEllipsis;
        TitleTextBlock.Padding = new Thickness(10, 0, 10, 0);

        TextBlock imagesTextBlock = CreateTitleBlock(stackPanel, "Images", margin: new Thickness(0, 10, 0, 0));
        CreateEditEntry(leftSide, "Icon Path", "IconPath", "", IconButton_Click);
        CreateEditEntry(leftSide, "Hero Path", "HeroPath", "", HeroButton_Click);

        CreateTitleBlock(leftSide, "Refresh Images", margin: new Thickness(0, 10, 0, 0));

        StackPanel buttonContainer = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
        StackPanel row1 = new StackPanel
            { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
        StackPanel row2 = new StackPanel
            { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
        buttonContainer.Children.Add(row1);
        buttonContainer.Children.Add(row2);

        var RefreshSGDBButton =
            new CustomButton(w: 120, h: 40, text: "Full SGDB", effect: AppEffects.DropShadowIcon,
                hA: HorizontalAlignment.Center);
        RefreshSGDBButton.Margin = new Thickness(5);
        RefreshSGDBButton.Click += (_, _) => { entry.RefreshImagesFromSGDB(); };
        if (Common.Settings.SGDBApiKey.Length == 0) RefreshSGDBButton.Active = false;
        row1.Children.Add(RefreshSGDBButton);

        var RefreshLocalHeroButton =
            new CustomButton(w: 120, h: 40, text: "Hero Local", effect: AppEffects.DropShadowIcon,
                hA: HorizontalAlignment.Center);
        RefreshLocalHeroButton.Margin = new Thickness(5);
        RefreshLocalHeroButton.Click += async (_, _) => { RefreshLocalHero(entry); };
        row1.Children.Add(RefreshLocalHeroButton);

        var RefreshLocalIconFromExeButton =
            new CustomButton(w: 120, h: 40, text: "Icon Local", effect: AppEffects.DropShadowIcon,
                hA: HorizontalAlignment.Center);
        RefreshLocalIconFromExeButton.Margin = new Thickness(5);
        RefreshLocalIconFromExeButton.Click += async (_, _) => { RefreshLocalIcon(entry); };
        Binding activeBinding = new Binding("IsLaunchable")
        {
            Source = entry,
            Mode = BindingMode.OneWay,
        };
        BindingOperations.SetBinding(RefreshLocalIconFromExeButton, CustomButton.ActiveProperty, activeBinding);
        row2.Children.Add(RefreshLocalIconFromExeButton);

        leftSide.Children.Add(buttonContainer);

        var ShowStatsButton =
            new CustomButton(w: 120, h: 40, text: "Show Stats", effect: AppEffects.DropShadowIcon,
                type: BType.Positive, hA: HorizontalAlignment.Center);
        ShowStatsButton.Margin = new Thickness(0, 10, 0, 20);
        ShowStatsButton.Click += (_, _) =>
        {
            StatsGraph statsGraph = new StatsGraph(entry);
            statsGraph.Open();
        };
        leftSide.Children.Add(ShowStatsButton);

        rightSide.Children.Add(remoteStackPanel);
    }

    private void IconButton_Click(object sender, RoutedEventArgs e)
    {
        string newPath = Common.GetDialogPath(Common.imageFilter);
        if (!newPath.Equals("")) _entry.IconPath = newPath;
    }

    private void HeroButton_Click(object sender, RoutedEventArgs e)
    {
        string newPath = Common.GetDialogPath(Common.imageFilter);
        if (!newPath.Equals("")) _entry.HeroPath = newPath;
    }

    private async void RefreshLocalHero(Entry entry)
    {
        await Task.Run(() =>
        {
            Guid guid = Guid.NewGuid();
            string newImagePath = Path.Combine(AppFiles.SavedImagesPath, $"_{guid}_hero.png");
            ImageHelper.ScatterImage(entry.IconPath, newImagePath);

            Dispatcher.Invoke(() => entry.HeroPath = newImagePath);
        });
    }

    private async void RefreshLocalIcon(Entry entry)
    {
        await Task.Run(() =>
        {
            Guid guid = Guid.NewGuid();
            string newImagePath = Path.Combine(AppFiles.SavedImagesPath, $"_{guid}_icon.png");
            string cloned = string.Copy(newImagePath);
            // bool success = ImageHelper.SaveIconFromExe(entry.ExePath, newImagePath);
            ImageHelper.SaveIconFromExe(entry.ExePath, newImagePath);
            Dispatcher.Invoke(() => entry.IconPath = newImagePath);
        });
    }
}