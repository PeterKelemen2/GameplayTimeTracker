using System;
using Microsoft.Win32;

namespace GameplayTimeTracker;

public static class EditHelper
{
    public static void UpdateIcon(Entry entry)
    {
        entry.IconPath = GetDialogPath(entry,
            "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Executable files (*.exe)|*.exe|All files (*.*)|*.*");
        Console.WriteLine($"New icon for {entry.Name}: {entry.IconPath}");
    }

    public static void UpdateHero(Entry entry)
    {
        entry.HeroPath = GetDialogPath(entry,
            "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Executable files (*.exe)|*.exe|All files (*.*)|*.*");
        Console.WriteLine($"New hero for {entry.Name}: {entry.HeroPath}");
    }

    public static void UpdateExe(Entry entry)
    {
        entry.ExePath = GetDialogPath(entry, "Executable files (*.exe)|*.exe|All files (*.*)|*.*");
    }

    private static string GetDialogPath(Entry entry, string filter)
    {
        string filePath = "";
        OpenFileDialog openFileDialog = new OpenFileDialog();
        if (System.IO.Path.Exists(entry.ExePath))
        {
            openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(entry.ExePath);
        }

        openFileDialog.Filter = filter;
        if (openFileDialog.ShowDialog() == true)
        {
            filePath = openFileDialog.FileName;
        }

        return filePath;
    }
}