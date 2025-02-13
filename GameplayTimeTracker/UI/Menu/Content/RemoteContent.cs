using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using GameplayTimeTracker.Menu;

namespace GameplayTimeTracker.UI.Menu.Content;

public class RemoteContent : UserControl
{
    StackPanel savesStackPanel;
    StackPanel container;
    CustomButton uploadButton;
    CustomButton downloadButton;
    private Entry _entry;
    private List<string> filesList;
    private double scrollWidth = 300;
    private double scrollHeight = 200;
    private string currentSelectedPath = "";
    private Run countRun;
    private bool isRemoteReachable = true;

    public RemoteContent()
    {
    }

    public RemoteContent(Entry entry)
    {
        _entry = entry;
        filesList = new List<string>();

        container = new StackPanel();

        TextBlock countBlock = UIHelper.CreateTextBlock("Saves found: ", fontSize: 17, hA: HorizontalAlignment.Center);
        countRun = new Run { Text = "Calculating...", FontWeight = FontWeights.Regular };
        countBlock.Inlines.Add(countRun);

        container.Children.Add(countBlock);

        ScrollViewer savesScrollViewer = new ScrollViewer
            { Height = scrollHeight, Width = scrollWidth, VerticalScrollBarVisibility = ScrollBarVisibility.Hidden };
        savesStackPanel = new StackPanel();

        Border savesBorder = new Border
        {
            Height = scrollHeight, Width = scrollWidth,
            CornerRadius = new CornerRadius(Common.BorderRadius),
            Background = new SolidColorBrush(ColorHelper.AdjustBrightness(
                (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]), 1.2)),
            Margin = new Thickness(10),
            Child = savesScrollViewer,
            Effect = AppEffects.DropShadowIcon
        };

        savesScrollViewer.Content = savesStackPanel;

        container.Children.Add(savesBorder);
        Content = container;
        CreateButtons();
        LoadData();
    }

    private void LoadData()
    {
        AddLoadingIndicator();
        LoadSubfoldersAsync();
    }

    private void CreateButtons()
    {
        StackPanel buttonsStackPanel = new StackPanel
            { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
        Binding validPathBinding = new Binding("IsSavePathValid") { Source = _entry, Mode = BindingMode.OneWay };

        uploadButton = GetButton("Upload", UploadButton_Click, validPathBinding);
        downloadButton = GetButton("Download", DownloadButton_Click, validPathBinding);

        buttonsStackPanel.Children.Add(uploadButton);
        buttonsStackPanel.Children.Add(downloadButton);
        container.Children.Add(buttonsStackPanel);
    }

    private CustomButton GetButton(string text, RoutedEventHandler action, Binding binding)
    {
        var button = new CustomButton(w: 120, h: 40, text: text, effect: AppEffects.DropShadowIcon,
            hA: HorizontalAlignment.Center);
        button.Margin = new Thickness(5);
        button.Click += action;
        BindingOperations.SetBinding(button, CustomButton.ActiveProperty, binding);

        return button;
    }

    private async void DownloadButton_Click(object sender, RoutedEventArgs e)
    {
        DisableButtons();
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Restart();

        if (string.IsNullOrEmpty(currentSelectedPath)) return;
        Console.WriteLine($"Downloading selected save from {currentSelectedPath}");

        bool success =
            await RemoteController.DownloadFolderAsync(currentSelectedPath, _entry.LocalSavePath);
        var downloadPrompt = new PromptMenu(width: 300,
            textArray: new[] { success ? "Download successful!" : "Failed to download." }, boldArray: new[] { true },
            toScale: false);
        downloadPrompt.Open();

        int elapsedMs = (int)Math.Round(stopwatch.Elapsed.TotalMilliseconds);
        Console.WriteLine($"############ Download took {stopwatch.Elapsed.TotalMilliseconds.ToString("F2")}ms");
        if (elapsedMs < 1000) await Task.Delay(1000 - elapsedMs);

        EnableButtons();
    }

    private async void UploadButton_Click(object sender, RoutedEventArgs e)
    {
        DisableButtons();
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Restart();

        if (!Directory.Exists(_entry.LocalSavePath)) return;
        Console.WriteLine($"Uploading local save to {_entry.LocalSavePath}");

        string uploadPath =
            $"{Common.Settings.RemoteMachine.RemoteFolder.TrimEnd('/')}/{_entry.Name}/{DateTime.Now:yyyy-MM-dd-HH-mm-ss}";
        bool success = await RemoteController.UploadFolderAsync(_entry.LocalSavePath, uploadPath);
        var uploadPrompt = new PromptMenu(width: 300,
            textArray: new[] { success ? "Upload successful!" : "Failed to upload." }, boldArray: new[] { true },
            toScale: false);
        uploadPrompt.Open();

        LoadData();
        int elapsedMs = (int)Math.Round(stopwatch.Elapsed.TotalMilliseconds);
        Console.WriteLine($"############ Upload took {stopwatch.Elapsed.TotalMilliseconds.ToString("F2")}ms");
        if (elapsedMs < 1000) await Task.Delay(1000 - elapsedMs);
        EnableButtons();
    }

    private void DisableButtons()
    {
        uploadButton.Active = false;
        downloadButton.Active = false;
    }

    private void EnableButtons()
    {
        uploadButton.Active = true;
        downloadButton.Active = true;
    }

    private async void AddLoadingIndicator()
    {
        await FadeOutElement(savesStackPanel);
        savesStackPanel.Children.Clear();
        Image loading = new Image
        {
            Width = 64, Height = 64,
            Margin = new Thickness(0, scrollHeight / 2 - 32, 0, 0),
            Source = new BitmapImage(new Uri(AppFiles.LoadingImage, UriKind.RelativeOrAbsolute)),
            RenderTransformOrigin = new Point(0.5, 0.5)
        };

        RotateTransform rotateTransform = new RotateTransform();
        loading.RenderTransform = rotateTransform;
        rotateTransform.BeginAnimation(RotateTransform.AngleProperty, AppAnimations.RotationAnimation);

        savesStackPanel.Children.Add(loading);
        await FadeInElement(savesStackPanel);
    }

    private async void LoadSubfoldersAsync()
    {
        try
        {
            var remote = Common.Settings.RemoteMachine;
            List<string> remoteContent = new List<string>();

            if (await RemoteController.IsRemoteMachineAvailableAsync())
            {
                remoteContent = await RemoteController
                    .ListGameSubfoldersAsync(remote.RemoteFolder, _entry.Name).ConfigureAwait(false);
            }
            else
            {
                isRemoteReachable = false;
            }

            Dispatcher.Invoke(() =>
            {
                ShowSaves(remoteContent);
                ChangeRunText(remoteContent.Count.ToString());
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async Task ChangeRunText(string text)
    {
        int stepTime = (int)(AppAnimations.fastFadeAnimDuration / countRun.Text.Length * 1000);
        while (countRun.Text.Length > 0)
        {
            countRun.Text = countRun.Text.Remove(countRun.Text.Length - 1, 1);
            await Task.Delay(stepTime);
        }

        stepTime = (int)(AppAnimations.fastFadeAnimDuration / text.Length * 1000);
        while (countRun.Text.Length < text.Length)
        {
            countRun.Text = countRun.Text.Insert(countRun.Text.Length, text[countRun.Text.Length].ToString());
            await Task.Delay(stepTime);
        }
    }

    private async void ShowSaves(List<string> files)
    {
        await FadeOutElement(savesStackPanel);
        savesStackPanel.Children.Clear();

        if (files.Count == 0)
        {
            string message = isRemoteReachable ? "No saves were found!" : "Server unreachable!";

            Console.WriteLine(message);
            TextBlock textBlock = new TextBlock
            {
                Text = message, FontSize = 21, FontWeight = FontWeights.Bold,
                Foreground =
                    new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"])),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, scrollHeight / 2 - 21, 0, 0),
                Padding = new Thickness(5),
            };

            savesStackPanel.Children.Add(textBlock);
        }
        else
        {
            bool isFirst = true;

            foreach (var file in files)
            {
                string? saveName = file.Split("/").LastOrDefault();
                Border textBorder = new Border { CornerRadius = new CornerRadius(8.5), Margin = new Thickness(5) };
                TextBlock textBlock = new TextBlock
                {
                    Text = saveName, FontSize = Common.TitleFontSize,
                    Foreground =
                        new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Font"])),
                    HorizontalAlignment = HorizontalAlignment.Center, Padding = new Thickness(5),
                };
                textBorder.Child = textBlock;
                textBorder.MouseDown += (s, e) => { SelectMouseDown(textBorder, file); };

                savesStackPanel.Children.Add(textBorder);

                if (isFirst)
                {
                    isFirst = false;
                    SelectMouseDown(textBorder, file);
                }
            }
        }

        await FadeInElement(savesStackPanel);
    }

    private void SelectMouseDown(Border border, string file)
    {
        currentSelectedPath = Path.Combine(AppFiles.BackupDataFolder, file);
        HighlightSelected(border);
        Console.WriteLine($"Selected file: {file}");
    }

    private Task FadeOutElement(UIElement element)
    {
        var tcs = new TaskCompletionSource<bool>();
        DoubleAnimation fadeOut = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(AppAnimations.fastFadeAnimDuration));
        fadeOut.Completed += (s, e) => tcs.SetResult(true);
        element.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        return tcs.Task;
    }

    private Task FadeInElement(UIElement element)
    {
        var tcs = new TaskCompletionSource<bool>();
        DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(AppAnimations.fastFadeAnimDuration));
        fadeIn.Completed += (s, e) => tcs.SetResult(true);
        element.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        return tcs.Task;
    }

    private void HighlightSelected(Border border)
    {
        foreach (UIElement element in savesStackPanel.Children)
        {
            if (element is Border b)
            {
                b.Background = b == border
                    ? new SolidColorBrush(ColorHelper.AdjustBrightness(
                        (Color)ColorConverter.ConvertFromString(Common.Settings.CurrentTheme.Colors["Background"]),
                        0.8))
                    : new SolidColorBrush(Colors.Transparent);
            }
        }
    }
}