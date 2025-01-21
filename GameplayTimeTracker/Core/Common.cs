using Microsoft.Win32;

namespace GameplayTimeTracker;

public static class Common
{
    public const string RunningText = "Running!";
    public const double CardPadding = 10;
    public const double TitleFontSize = 17;
    public const double EditTitleFontSize = 21;
    public const double TextFontSize = 14;
    public const double BorderRadius = 10;
    public const double TextBoxHeight = 28;

    public static int[] p = { 33, 11, 11, 10, 10, 11, 45, 45, 17 };
    
    public static string imageFilter = "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Executable files (*.exe)|*.exe|All files (*.*)|*.*";
    public static string exeFilter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";

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
}