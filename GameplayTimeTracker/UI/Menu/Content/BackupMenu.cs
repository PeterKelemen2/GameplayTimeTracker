using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GameplayTimeTracker.Settings;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;

namespace GameplayTimeTracker.UI.Menu.Content;

public class BackupMenu : MenuContent
{
    private AppSettings appSettings;
    private ScrollViewer backupEntryScrollViewer;
    private ScrollViewer backupContentScrollViewer;
    private TextBlock backupContentBlock;
    private StackPanel backupEntriesPanel;
    private StackPanel backupContentPanel;
    private Border backupContentBorder;
    private StackPanel ContentPanel;
    private CustomButton createBackupButton;
    private CustomButton restoreBackupButton;
    private List<string> backupFilesList = new();
    private string currentSelectedPath = "";
    private double scrollWidth = 350;
    private double scrollHeight = 200;

    public BackupMenu()
    {
        appSettings = Common.Settings;
        TextBlock backupListBlock =
            UIHelper.CreateTextBlock("Select a backup", hA: HorizontalAlignment.Center, fontSize: 20);
        backupListBlock.Margin = new Thickness(0, 10, 0, 0);

        _stackPanel.Children.Add(backupListBlock);

        backupEntryScrollViewer = new ScrollViewer
        {
            Height = scrollHeight, Width = scrollWidth,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
        };
        Border backupEntryBorder = new Border
        {
            Height = scrollHeight, Width = scrollWidth,
            CornerRadius = new CornerRadius(Common.BorderRadius),
            Background = new SolidColorBrush(ColorHelper.AdjustBrightness(
                (Color)ColorConverter.ConvertFromString(appSettings.CurrentTheme.Colors["Background"]), 1.2)),
            Margin = new Thickness(10),
            Child = backupEntryScrollViewer,
            Effect = AppEffects.DropShadowIcon
        };

        if (!Path.Exists(AppFiles.BackupDataFolder))
        {
            Directory.CreateDirectory(AppFiles.BackupDataFolder);
        }

        backupEntriesPanel = new StackPanel();
        ShowBackupEntries();
        backupEntryScrollViewer.Content = backupEntriesPanel;
        _stackPanel.Children.Add(backupEntryBorder);

        ContentPanel = new StackPanel();
        ContentPanel.Visibility = Visibility.Collapsed;
        backupContentBlock =
            UIHelper.CreateTextBlock("Backup Content", hA: HorizontalAlignment.Center, fontSize: 20);
        backupContentBlock.Margin = new Thickness(0, 10, 0, 0);
        ContentPanel.Children.Add(backupContentBlock);

        backupContentScrollViewer = new ScrollViewer
        {
            Height = scrollHeight, Width = scrollWidth,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
            VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
        };
        backupContentBorder = new Border
        {
            Height = scrollHeight, Width = scrollWidth,
            CornerRadius = new CornerRadius(Common.BorderRadius),
            Background = new SolidColorBrush(ColorHelper.AdjustBrightness(
                (Color)ColorConverter.ConvertFromString(appSettings.CurrentTheme.Colors["Background"]), 1.2)),
            Margin = new Thickness(10),
            Child = backupContentScrollViewer,
            Effect = AppEffects.DropShadowIcon
        };

        backupContentPanel = new StackPanel();
        backupContentScrollViewer.Content = backupContentPanel;

        ContentPanel.Children.Add(backupContentBorder);
        _stackPanel.Children.Add(ContentPanel);

        restoreBackupButton = new CustomButton(text: "Restore Backup", w: 170, h: 40, isBold: true,
            effect: AppEffects.DropShadowMedium);
        restoreBackupButton.Margin = new Thickness(10, 5, 10, 5);
        restoreBackupButton.Click += (s, e) =>
        {
            DataHandler.RestoreBackup(currentSelectedPath);
            ((MainWindow)Application.Current.MainWindow).LoadAndShowData();
        };
        _stackPanel.Children.Add(restoreBackupButton);
        restoreBackupButton.Disable();

        createBackupButton = new CustomButton(text: "Create Backup", w: 170, h: 40, isBold: true,
            effect: AppEffects.DropShadowMedium, type: BType.Positive);
        createBackupButton.Margin = new Thickness(10, 5, 10, 20);
        createBackupButton.Click += (s, e) =>
        {
            DataHandler.CreateBackup();
            ShowBackupEntries();
        };
        _stackPanel.Children.Add(createBackupButton);
    }

    private void ShowBackupEntries()
    {
        backupEntriesPanel.Children.Clear();
        backupFilesList = GetAllFiles(AppFiles.BackupDataFolder);
        foreach (var file in backupFilesList)
        {
            Console.WriteLine(file);
            TextBlock bEntryBlock = new TextBlock
            {
                Text = file,
                FontSize = Common.TitleFontSize,
                Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(appSettings.CurrentTheme.Colors["Font"])),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Padding = new Thickness(5),
                Margin = new Thickness(5),
            };
            bEntryBlock.MouseDown += (s, e) =>
            {
                currentSelectedPath = Path.Combine(AppFiles.BackupDataFolder, file);
                HighlightSelected(bEntryBlock);
                Console.WriteLine(currentSelectedPath);
                ShowBackupContents(file);
            };

            backupEntriesPanel.Children.Add(bEntryBlock);
        }
    }

    private void ShowBackupContents(string backupPath)
    {
        string path = Path.Combine(AppFiles.BackupDataFolder, backupPath);
        ObservableCollection<Entry> entryList = new();
        entryList = DataHandler.GetEntriesFromFile(path);
        Common.CheckForOldTime(entryList);
        Console.WriteLine($"Found {entryList.Count} entries from {path}");
        backupContentPanel.Children.Clear();
        foreach (var entry in entryList)
        {
            TextBlock entryBlock = new TextBlock
            {
                // Text = entry.Name,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = Common.TitleFontSize,
                Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(appSettings.CurrentTheme.Colors["Font"])),
                Margin = new Thickness(10, 5, 0, 5),
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Left,
            };

            var entryNameRun = new Run { Text = Common.Trim(entry.Name, 25, true), FontWeight = FontWeights.Bold, };
            var entryTimeRun = new Run { Text = $" - {entry.TotalPlayFormatted}", FontWeight = FontWeights.Regular, };
            entryBlock.Inlines.Add(entryNameRun);
            entryBlock.Inlines.Add(entryTimeRun);
            backupContentPanel.Children.Add(entryBlock);
            Console.WriteLine($"{entry.Name} - {entry.TotalPlayFormatted}");
        }
    }

    private void HighlightSelected(TextBlock textBlock)
    {
        if (ContentPanel.Visibility == Visibility.Collapsed)
        {
            ContentPanel.Visibility = Visibility.Visible;
            ContentPanel.Measure(new Size(scrollWidth, double.PositiveInfinity));
            ContentPanel.Arrange(new Rect(ContentPanel.DesiredSize));
            AppAnimations.BackupPanelGrowAnimation.From = _scrollViewer.ActualHeight;
            AppAnimations.BackupPanelGrowAnimation.To = ContentPanel.ActualHeight + _scrollViewer.ActualHeight;
            _scrollViewer.BeginAnimation(HeightProperty, AppAnimations.BackupPanelGrowAnimation);
            restoreBackupButton.Enable();
        }

        foreach (UIElement element in backupEntriesPanel.Children)
        {
            if (element is TextBlock tb)
            {
                tb.Background = tb == textBlock
                    ? new SolidColorBrush(ColorHelper.AdjustBrightness(
                        (Color)ColorConverter.ConvertFromString(appSettings.CurrentTheme.Colors["Background"]), 0.8))
                    : new SolidColorBrush(Colors.Transparent);
            }
        }
    }

    List<string?> GetAllFiles(string folderPath, string extension = ".json")
    {
        if (Directory.Exists(folderPath))
        {
            return Directory.GetFiles(folderPath, $"*{extension}")
                .Select(Path.GetFileName)
                .ToList();
        }
        else
        {
            throw new DirectoryNotFoundException($"The directory '{folderPath}' does not exist.");
        }
    }
}