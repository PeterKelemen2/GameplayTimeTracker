using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using GameplayTimeTracker.Settings;
using Hardcodet.Wpf.TaskbarNotification;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using TextBox = System.Windows.Controls.TextBox;

namespace GameplayTimeTracker;

public static class Common
{
    public static AppSettings Settings { get; set; }
    public static EntryRepository Repository { get; set; }
    public static GameCardRepository CardRepository { get; set; }
    public static TaskbarIcon TaskbarIcon { get; set; }

    public const string RunningText = "Running!";
    public const double CardPadding = 10;
    public const double TitleFontSize = 17;
    public const double EditTitleFontSize = 21;
    public const double TextFontSize = 14;
    public const double BorderRadius = 10;
    public const double TextBoxHeight = 28;
    public const double ScrollDurationsMs = 200;
    public static System.Drawing.Size HeroSize = new System.Drawing.Size(960, 310);

    public static int[] p = { 33, 11, 11, 10, 10, 11, 45, 45, 17 };
    public static int[] saveFreqArray = { 1, 5, 10, 15, 30 };

    public static string imageFilter =
        "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Executable files (*.exe)|*.exe|All files (*.*)|*.*";

    public static string exeFilter = "Executable files (*.exe, *.lnk)|*.exe;*.lnk|All files (*.*)|*.*";

    public static string folderFilter = "Folder files (*.*)|*.*";

    public static string Truncate(string value, int length, bool toDot = false)
    {
        if (value.Length < length)
        {
            // Pad the string with spaces if it's shorter than the desired length
            return value.PadRight(length);
        }

        string dots = toDot ? "..." : "";
        return value.Substring(0, length) + dots; // Otherwise, truncate the string
    }

    public static string Trim(string value, int length, bool toDot = false)
    {
        string dots = toDot ? "..." : "";
        if (value.Length < length)
        {
            return value;
        }
        else
        {
            return value.Substring(0, length) + dots;
        }
    }

    public static void CheckForOldTime(ObservableCollection<Entry> entries)
    {
        int[] empty = { 0, 0, 0 };

        foreach (var entry in entries)
        {
            if (IsArrayEqual(entry.TotalPlay, empty) && entry.TotalTime > 0.0)
            {
                entry.TotalPlay = Common.GetArrayFromDoubleTime(entry.TotalTime);
            }

            if (IsArrayEqual(entry.LastPlay, empty) && entry.LastTime > 0.0)
            {
                entry.LastPlay = Common.GetArrayFromDoubleTime(entry.LastTime);
            }
        }
    }

    private static bool IsArrayEqual(int[] array1, int[] array2)
    {
        return array1 != null && array2 != null && array1.SequenceEqual(array2);
    }

    public static int[] NormalizeTimeArray(int[] arr)
    {
        if (arr.Length == 3)
        {
            if (arr[2] >= 60)
            {
                arr[1] += arr[2] / 60;
                arr[2] %= 60;

                if (arr[1] >= 60)
                {
                    arr[0] += arr[1] / 60;
                    arr[1] %= 60;
                }
            }
        }

        // return arr;
        return new[] { arr[0], arr[1], arr[2] };
    }

    public static int[] AddTimeArrays(int[] array1, int[] array2)
    {
        int[] result = new int[3];
        for (int i = 0; i < 3; i++)
        {
            result[i] = array1[i] + array2[i];
        }

        return result;
    }

    public static int[] GetArrayFromDoubleTime(double totalHours)
    {
        int hours = (int)totalHours; // Extract the whole number part for hours
        double totalMinutes = (totalHours - hours) * 60; // Convert remaining fraction to minutes
        int minutes = (int)totalMinutes; // Extract the whole number part for minutes
        int seconds = (int)((totalMinutes - minutes) * 60); // Convert remaining fraction to seconds

        return new[] { hours, minutes, seconds };
    }

    public static double GetDoubleTimeFromArray(int[] arr)
    {
        return Math.Round(arr[0] + arr[1] / 60.0 + arr[2] / 3600.0, 2);
    }

    public static string GetPrettyTimeFromDouble(double totalHours)
    {
        int[] t = GetArrayFromDoubleTime(totalHours);
        return $"{t[0]}h {t[1]}m {t[2]}s";
    }

    public static string GetPrettyTimeFromArray(int[] arr)
    {
        return $"{arr[0]}h {arr[1]}m {arr[2]}s";
    }

    public static bool TimeArraysEqual(int[] arr1, int[] arr2)
    {
        if (arr1.Length != arr2.Length) return false;

        for (int i = 0; i < arr1.Length; i++)
        {
            if (arr1[i] != arr2[i]) return false;
        }

        return true;
    }

    public static string GetDialogPath(string filter)
    {
        string filePath = "";
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = filter;

        if (openFileDialog.ShowDialog() == true)
        {
            filePath = openFileDialog.FileName;
        }

        return filePath;
    }

    public static string GetFolderDialogPath()
    {
        using (var folderDialog = new FolderBrowserDialog())
        {
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                return folderDialog.SelectedPath;
            }
        }

        return null;
    }


    public static TextBox FindTextBox(Grid parentGrid)
    {
        foreach (UIElement child in parentGrid.Children)
        {
            if (child is TextBox textBox) return textBox;
        }

        return null;
    }
}