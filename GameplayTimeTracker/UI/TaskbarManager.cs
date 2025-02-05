using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Application = System.Windows.Application;
using Border = System.Windows.Controls.Border;
using Grid = System.Windows.Controls.Grid;

namespace GameplayTimeTracker;

public static class TaskbarManager
{
    public static void UpdateTrayEntries()
    {
        var mainWindow = Application.Current.MainWindow;
        MainWindow mainWin = (MainWindow)Application.Current.MainWindow;
        ContextMenu trayMenu = (ContextMenu)mainWindow.FindResource("TrayMenu");
        trayMenu.Items.Clear();

        List<Entry> sorted = Common.Repository.GetEntriesSortedForTray();
        foreach (var entry in sorted)
        {
            MenuItem entryMenuItem = new MenuItem { Header = $"Launch {entry.Name}" };
            Image entryImage = new Image
            {
                Source = new BitmapImage(new Uri(entry.IconPath)),
                Width = 16,
                Height = 16,
            };
            RenderOptions.SetBitmapScalingMode(entryImage, BitmapScalingMode.HighQuality);
            entryMenuItem.Icon = entryImage;
            entryMenuItem.Click += (s, e) => { Launcher.Launch(entry); };
            trayMenu.Items.Add(entryMenuItem);
        }

        MenuItem openItem = new MenuItem { Header = "Open" };
        Image openImage = new Image
        {
            Source = new BitmapImage(new Uri(AppFiles.ArrowIcon, UriKind.Relative)),
            Width = 16,
            Height = 16,
        };
        RenderOptions.SetBitmapScalingMode(openImage, BitmapScalingMode.HighQuality);
        openItem.Icon = openImage;
        openItem.Click += mainWin.TrayMenu_Open_Click;
        trayMenu.Items.Insert(0, openItem);
        Separator sep1 = new Separator();
        trayMenu.Items.Insert(1, sep1);

        Separator sep2 = new Separator();
        trayMenu.Items.Add(sep2);
        MenuItem exitItem = new MenuItem { Header = "Exit" };
        exitItem.Click += mainWin.TrayMenu_Exit_Click;
        trayMenu.Items.Add(exitItem);
    }

    public static void UpdateTrayToolTip()
    {
        var mainWindow = Application.Current.MainWindow;
        Border toolTipBorder = (Border)mainWindow.FindResource("TrayToolTipBorder");

        if (toolTipBorder == null)
        {
            Console.WriteLine("TrayToolTipBorder not found!");
            return;
        }

        Grid trayToolTipGrid = FindChild<Grid>(toolTipBorder, "TrayToolTipGrid");
        if (trayToolTipGrid == null)
        {
            Console.WriteLine("TrayToolTipGrid not found!");
            return;
        }

        BindingExpression gridBinding =
            BindingOperations.GetBindingExpression(toolTipBorder, Border.BackgroundProperty);
        if (gridBinding == null)
        {
            BindingHelper.SetGradientColorBinding(toolTipBorder, Border.BackgroundProperty, "Card 1", "Card 2", true);
            BindingHelper.SetColorBinding(toolTipBorder, Border.BorderBrushProperty, "Font");
        }

        TextBlock appTitleBlock = FindChild<TextBlock>(trayToolTipGrid, "AppTitleBlock");
        BindingExpression appTitleBinding =
            BindingOperations.GetBindingExpression(appTitleBlock, TextBlock.ForegroundProperty);
        if (appTitleBinding == null)
        {
            BindingHelper.SetColorBinding(appTitleBlock, TextBlock.ForegroundProperty, "Font");
        }

        Run versionRun = appTitleBlock?.Inlines.FirstOrDefault(i => i is Run && ((Run)i).Name == "VersionRun") as Run;
        if (versionRun != null)
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            string versionString = $"{version.Major}.{version.Minor}.{version.Build}";
            versionRun.Text = versionString;
        }

        TextBlock runningBlock = FindChild<TextBlock>(trayToolTipGrid, "RunningBlock");
        BindingExpression runningBinding =
            BindingOperations.GetBindingExpression(runningBlock, TextBlock.ForegroundProperty);
        if (runningBinding == null)
        {
            BindingHelper.SetColorBinding(runningBlock, TextBlock.ForegroundProperty, "Font");
        }

        Run countRun = runningBlock?.Inlines.FirstOrDefault(i => i is Run && ((Run)i).Name == "RunningCountRun") as Run;
        BindingExpression runningCountBinding =
            BindingOperations.GetBindingExpression(countRun, TextBlock.TextProperty);
        if (runningCountBinding == null)
        {
            Binding countBinding = new Binding("RunningEntryCount")
            {
                Source = Common.Repository, Mode = BindingMode.OneWay,
            };
            countRun.SetBinding(Run.TextProperty, countBinding);
        }

        TextBlock timeBlock = FindChild<TextBlock>(trayToolTipGrid, "TotalTimeBlock");
        BindingExpression timeColorBinding =
            BindingOperations.GetBindingExpression(timeBlock, TextBlock.ForegroundProperty);
        if (timeColorBinding == null)
        {
            BindingHelper.SetColorBinding(timeBlock, TextBlock.ForegroundProperty, "Font");
        }

        Run totalTimeRun = timeBlock?.Inlines.FirstOrDefault(i => i is Run && ((Run)i).Name == "TotalTimeRun") as Run;
        BindingExpression timeExistingBinding =
            BindingOperations.GetBindingExpression(totalTimeRun, TextBlock.TextProperty);
        if (timeExistingBinding == null)
        {
            Binding timeBinding = new Binding("TotalRuntimeFormatted")
            {
                Source = Common.Repository, Mode = BindingMode.OneWay,
            };
            totalTimeRun.SetBinding(Run.TextProperty, timeBinding);
        }

        Common.TaskbarIcon.TrayToolTip = toolTipBorder;
    }

    private static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
    {
        // Iterate through the visual tree of the parent element to find the child
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);

            if (child is T && ((T)child).GetValue(FrameworkElement.NameProperty).ToString() == childName)
            {
                return (T)child;
            }

            // Recursively search through children
            T result = FindChild<T>(child, childName);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}