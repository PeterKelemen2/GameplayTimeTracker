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
        OpenFileDialog openFileDialog = new OpenFileDialog();
        if (System.IO.Path.Exists(entry.ExePath))
        {
            openFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(entry.ExePath);
        }

        openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
        if (openFileDialog.ShowDialog() == true)
        {
            string filePath = openFileDialog.FileName;
            // HandleNewExePath(filePath);
            entry.ExePath = filePath;
        }
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
}