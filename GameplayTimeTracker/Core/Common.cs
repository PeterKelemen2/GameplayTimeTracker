using System;
using System.Windows;
using System.Windows.Controls;
using GameplayTimeTracker.Settings;
using Microsoft.Win32;

namespace GameplayTimeTracker;

public static class Common
{
    public static AppSettings Settings { get; set; }
    
    public const string RunningText = "Running!";
    public const double CardPadding = 10;
    public const double TitleFontSize = 17;
    public const double EditTitleFontSize = 21;
    public const double TextFontSize = 14;
    public const double BorderRadius = 10;
    public const double TextBoxHeight = 28;

    public static int[] p = { 33, 11, 11, 10, 10, 11, 45, 45, 17 };

    public static string imageFilter =
        "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Executable files (*.exe)|*.exe|All files (*.*)|*.*";

    public static string exeFilter = "Executable files (*.exe, *.lnk)|*.exe;*.lnk|All files (*.*)|*.*";

    public static string Truncate(string value, int length)
    {
        if (value.Length < length)
        {
            // Pad the string with spaces if it's shorter than the desired length
            return value.PadRight(length);
        }

        return value.Substring(0, length); // Otherwise, truncate the string
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


    public static TextBox FindTextBox(Grid parentGrid)
    {
        foreach (UIElement child in parentGrid.Children)
        {
            if (child is TextBox textBox) return textBox;
        }

        return null;
    }
}