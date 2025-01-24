using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GameplayTimeTracker.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace GameplayTimeTracker.UI.Menu.Content;

public class BackupMenu : UserControl
{
    public StackPanel Panel = new();
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

    public BackupMenu(AppSettings settings)
    {
        appSettings = settings;
        Panel = new StackPanel();

        TextBlock backupListBlock =
            UIHelper.CreateTextBlock("Select a backup", hA: HorizontalAlignment.Center, fontSize: 20);
        backupListBlock.Margin = new Thickness(0, 10, 0, 0);

        Panel.Children.Add(backupListBlock);

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
        };

        if (!Path.Exists(AppFiles.BackupDataFolder))
        {
            Directory.CreateDirectory(AppFiles.BackupDataFolder);
        }

        backupEntriesPanel = new StackPanel();
        ShowBackupEntries();
        backupEntryScrollViewer.Content = backupEntriesPanel;
        Panel.Children.Add(backupEntryBorder);

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
        };

        backupContentPanel = new StackPanel();
        backupContentScrollViewer.Content = backupContentPanel;

        ContentPanel.Children.Add(backupContentBorder);
        Panel.Children.Add(ContentPanel);

        restoreBackupButton = new CustomButton(text: "Restore Backup", w: 170, h: 40, isBold: true,
            effect: AppEffects.DropShadowMedium);
        restoreBackupButton.Margin = new Thickness(10, 5, 10, 5);
        restoreBackupButton.Click += (s, e) =>
        {
            DataHandler.RestoreBackup(currentSelectedPath);
            ((MainWindow)Application.Current.MainWindow).LoadAndShowData();
        };
        Panel.Children.Add(restoreBackupButton);

        createBackupButton = new CustomButton(text: "Create Backup", w: 170, h: 40, isBold: true,
            effect: AppEffects.DropShadowMedium, type: BType.Positive);
        createBackupButton.Margin = new Thickness(10, 5, 10, 20);
        createBackupButton.Click += (s, e) =>
        {
            DataHandler.CreateBackup();
            ShowBackupEntries();
        };
        Panel.Children.Add(createBackupButton);
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
        List<Entry> entryList = new();
        entryList = DataHandler.GetEntriesFromFile(path);
        Console.WriteLine($"Found {entryList.Count} entries from {path}");
        backupContentPanel.Children.Clear();
        foreach (var entry in entryList)
        {
            TextBlock entryBlock = new TextBlock
            {
                Text = entry.Name,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = Common.TitleFontSize,
                Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(appSettings.CurrentTheme.Colors["Font"])),
                Margin = new Thickness(0, 5, 0, 5),
                FontWeight = FontWeights.Bold,
            };
            var entryRun = new Run
            {
                Text = $" - {entry.TotalPlayFormatted}",
                FontWeight = FontWeights.Regular,
            };
            entryBlock.Inlines.Add(entryRun);
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
            AppAnimations.BackupPanelGrowAnimation.To = ContentPanel.ActualHeight;
            ContentPanel.BeginAnimation(HeightProperty, AppAnimations.BackupPanelGrowAnimation);
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