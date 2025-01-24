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
using System.Reflection;
using System.Text.Json;

namespace GameplayTimeTracker.UI.Menu.Content;

public class BackupMenu : UserControl
{
    public StackPanel Panel = new();
    private AppSettings appSettings;
    private ScrollViewer colorEntryScrollViewer;
    private StackPanel colorEntryPanel;

    public BackupMenu(AppSettings settings)
    {
        appSettings = settings;

        Panel = new StackPanel();

        if (!Path.Exists(AppFiles.BackupDataFolder))
        {
            Directory.CreateDirectory(AppFiles.BackupDataFolder);
        }
    }
}