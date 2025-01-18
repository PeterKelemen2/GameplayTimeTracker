using Microsoft.Win32;

namespace GameplayTimeTracker;

public static class EditHelper
{
    public static void UpdateIcon(Entry entry)
    {
    }

    public static void UpdateHero(Entry entry)
    {
    }

    public static void UpdateExe(Entry entry)
    {
        entry.ExePath = GetDialogPath(entry, "Executable files (*.exe)|*.exe|All files (*.*)|*.*");
    }

    public static void UpdateName(Entry entry)
    {
    }

    public static void UpdateTime(Entry entry)
    {
    }

    public static void UpdateArguments(Entry entry)
    {
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