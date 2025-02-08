using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameplayTimeTracker.UI.Menu.Content;

public class RemoteContent : UserControl
{
    StackPanel stackPanel;
    private Entry _entry;
    private List<string> filesList = new List<string>();

    public RemoteContent(Entry entry)
    {
        _entry = entry;
        stackPanel = new StackPanel { Width = 100, Height = 200, Background = Brushes.Black };
        Content = stackPanel;

        Border entriesBorder = new Border();
        StackPanel entriesStackPanel = new StackPanel();

        LoadSubfoldersAsync();
    }
    
    private async void LoadSubfoldersAsync()
    {
        try
        {
            filesList = await RemoteController.ListGameSubfoldersAsync(Common.Settings.RemoteMachine.RemoteFolder, _entry.Name);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}