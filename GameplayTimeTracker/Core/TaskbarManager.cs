using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GameplayTimeTracker.Helper;
using GameplayTimeTracker.Menu;
using GameplayTimeTracker.Settings;
using GameplayTimeTracker.SGDB;
using Gtk;
using Hardcodet.Wpf.TaskbarNotification;
using Shellify;
using Application = System.Windows.Application;
using Grid = System.Windows.Controls.Grid;
using Window = System.Windows.Window;

namespace GameplayTimeTracker;

public static class TaskbarManager
{
    public static void UpdateTrayToolTip(TaskbarIcon taskbarIcon, EntryRepository entryRepository)
    {
        var mainWindow = Application.Current.MainWindow;
        // Access TrayToolTip from resources
        Panel trayToolTip = (Panel)mainWindow.FindResource("TrayToolTip");

        // Modify its content
        TextBlock textBlock = trayToolTip.Children[0] as TextBlock;
        if (textBlock != null)
        {
            textBlock.Text = "Test Text";
        }

        // Update the tray icon tooltip to reflect the change
        taskbarIcon.TrayToolTip = trayToolTip;
    }
}