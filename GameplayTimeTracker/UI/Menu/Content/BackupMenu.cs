using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GameplayTimeTracker.Settings;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Documents;

namespace GameplayTimeTracker.UI.Menu.Content;

public class BackupMenu : MenuContent
{
    private ScrollViewer backupEntryScrollViewer;
    private ScrollViewer backupContentScrollViewer;
    private TextBlock backupContentBlock;
    private StackPanel backupEntriesPanel;
    private StackPanel backupContentPanel;
    private Border backupContentBorder;
    private StackPanel ContentPanel;
    private CustomButton createBackupButton;
    private CustomButton restoreBackupButton;
    private List<string?> backupFilesList = new();
    private string currentSelectedPath = "";
    private double scrollWidth = 350;
    private double scrollHeight = 200;

    public BackupMenu()
    {
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
                (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]), 1.2)),
            Margin = new Thickness(10), Effect = AppEffects.DropShadowIcon,
            Child = backupEntryScrollViewer,
        };

        Directory.CreateDirectory(AppFiles.BackupDataFolder);

        backupEntriesPanel = new StackPanel();
        backupEntryScrollViewer.Content = backupEntriesPanel;
        _stackPanel.Children.Add(backupEntryBorder);
        ShowBackupEntries();

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
                (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]), 1.2)),
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
        backupFilesList = GetAllFiles(AppFiles.BackupDataFolder)
            .Where(file => Path.GetExtension(file).Equals(".json", StringComparison.OrdinalIgnoreCase) &&
                           IsBackupValid(Path.Combine(AppFiles.BackupDataFolder, file)))
            .OrderByDescending(file => File.GetLastWriteTime(Path.Combine(AppFiles.BackupDataFolder, file)))
            .ToList();

        foreach (var file in backupFilesList)
        {
            Console.WriteLine(file);
            Border textBorder = new Border
            {
                CornerRadius = new CornerRadius(8.5), Margin = new Thickness(5),
                Background = new SolidColorBrush(Colors.Transparent),
            };
            TextBlock bEntryBlock = new TextBlock
            {
                Text = file, FontSize = Common.TitleFontSize,
                Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"])),
                HorizontalAlignment = HorizontalAlignment.Center, Padding = new Thickness(5),
            };
            textBorder.Child = bEntryBlock;

            textBorder.MouseDown += (s, e) =>
            {
                currentSelectedPath = Path.Combine(AppFiles.BackupDataFolder, file);
                Console.WriteLine(currentSelectedPath);
                HighlightSelected(textBorder);
                ShowBackupFileContents(file);
            };

            backupEntriesPanel.Children.Add(textBorder);
        }
    }

    private void ShowBackupFileContents(string backupPath)
    {
        string path = Path.Combine(AppFiles.BackupDataFolder, backupPath);

        List<EntryProxy> entryList = DataHandler.GetEntryProxiesFromFile(path);
        Common.CheckForOldTimeProxy(entryList);
        Console.WriteLine($"Found {entryList.Count} entries from {path}");
        backupContentPanel.Children.Clear();
        foreach (var entry in entryList)
        {
            AddEntryToBackupContents(entry);
        }
    }

    private void AddEntryToBackupContents(EntryProxy entry)
    {
        TextBlock entryBlock = new TextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            FontSize = Common.TitleFontSize,
            Foreground =
                new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"])),
            Margin = new Thickness(10, 5, 0, 5),
            FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Left,
        };

        bool success = false;
        try
        {
            string? totalPlayFormatted = new TimeArrayConverter().Convert(entry.TotalPlay) as string;
            var entryNameRun = new Run { Text = Common.Trim(entry.Name, 25, true), FontWeight = FontWeights.Bold, };
            var entryTimeRun = new Run { Text = $" - {totalPlayFormatted}", FontWeight = FontWeights.Regular, };
            entryBlock.Inlines.Add(entryNameRun);
            entryBlock.Inlines.Add(entryTimeRun);

            success = true;
        }
        catch (NullReferenceException ex)
        {
            Console.WriteLine(ex.Message);
        }

        if (!success)
        {
            entryBlock.Text = "No data found.";
            entryBlock.FontWeight = FontWeights.SemiBold;
            entryBlock.HorizontalAlignment = HorizontalAlignment.Center;
            entryBlock.Margin = new Thickness(0, scrollHeight / 2 - entryBlock.FontSize, 0, 0);
        }

        backupContentPanel.Children.Add(entryBlock);
    }

    private void HighlightSelected(Border border)
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
            if (element is Border b)
            {
                b.Background = b == border
                    ? new SolidColorBrush(ColorHelper.AdjustBrightness(
                        (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]),
                        0.8))
                    : new SolidColorBrush(Colors.Transparent);
            }
        }
    }

    private bool IsBackupValid(string backupPath)
    {
        bool isValid = false;
        if (Path.GetExtension(backupPath) != ".json") return false;

        try
        {
            List<EntryProxy> entryList = DataHandler.GetEntryProxiesFromFile(backupPath);
            if (entryList.Count > 0) return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return isValid;
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