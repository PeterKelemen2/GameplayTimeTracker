using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using GameplayTimeTracker.UI.Menu.Content;

namespace GameplayTimeTracker.Menu.Content;

public class RemoteMenu : MenuContent
{
    private TextBlock ConnectionBlock;

    public RemoteMenu()
    {
        TextBlock MainTitle =
            UIHelper.CreateTextBlock("Remote Saving", hA: HorizontalAlignment.Center, fontSize: 17);
        MainTitle.Margin = new Thickness(0, 15, 0, -5);
        _stackPanel.Children.Add(MainTitle);

        PrefEntry savingEnabledPref = new PrefEntry("Remote Save Enabled", Common.Settings.IsRemoteSavingEnabled, 270);
        Binding savingEnabledBinding = new Binding("IsRemoteSavingEnabled")
            { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(savingEnabledPref.toggleButton, CustomToggleButton.IsToggledProperty,
            savingEnabledBinding);
        BindingHelper.SetColorBinding(savingEnabledPref.textBlock, ForegroundProperty, "Font");
        _stackPanel.Children.Add(savingEnabledPref);

        TextBlock RemoteMachineTitle =
            UIHelper.CreateTextBlock("Remote Machine", hA: HorizontalAlignment.Center, fontSize: 17);
        RemoteMachineTitle.Margin = new Thickness(0, 5, 0, 5);
        _stackPanel.Children.Add(RemoteMachineTitle);
        Thickness prefMargin = new Thickness(10, 0, 10, 0);
        double prefBoxWidth = 140;
        StackPanel row1 = new StackPanel
            { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
        CreatePrefEntry(row1, "Address", "RemoteMachine.Address", margin: prefMargin, boxWidth: prefBoxWidth);
        CreatePrefEntry(row1, "Port", "RemoteMachine.Port", margin: prefMargin, boxWidth: prefBoxWidth);
        _stackPanel.Children.Add(row1);

        StackPanel row2 = new StackPanel
            { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
        CreatePrefEntry(row2, "User", "RemoteMachine.User", margin: prefMargin, boxWidth: prefBoxWidth);
        CreatePrefEntry(row2, "Password", "RemoteMachine.Password", margin: prefMargin, boxWidth: prefBoxWidth);
        _stackPanel.Children.Add(row2);

        CreatePrefEntry(_stackPanel, "Remote folder", "RemoteMachine.RemoteFolder", new Thickness(0, 0, 0, 0));

        ConnectionBlock = UIHelper.CreateTextBlock("", hA: HorizontalAlignment.Center, fontSize: 17);
        ConnectionBlock.Margin = new Thickness(0, 5, 0, 5);
        BindingHelper.SetColorBinding(ConnectionBlock, ForegroundProperty, "Font");
        _stackPanel.Children.Add(ConnectionBlock);

        CustomButton testConnectionButton = new CustomButton(text: "Test Connection", w: 140, h: 40);
        testConnectionButton.Margin = new Thickness(0, 0, 0, 15);
        testConnectionButton.Click += ShowConnectedText;
        _stackPanel.Children.Add(testConnectionButton);
    }

    private async void ShowConnectedText(object sender, RoutedEventArgs e)
    {
        if (sender is CustomButton button)
        {
            ConnectionBlock.Foreground = new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(
                    Common.Settings.CurrentTheme.Colors["Font"]));

            DispatcherTimer dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            dispatcherTimer.Tick += (_, _) =>
            {
                ConnectionBlock.Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(
                        Common.Settings.CurrentTheme.Colors["Font"]));
                ConnectionBlock.Text = string.Empty;
                dispatcherTimer.Stop();
            };
            button.Disable();
            try
            {
                ConnectionBlock.Text = "Testing connection...";

                bool success = await Task.Run(() => RemoteController.TestSftpConnection());

                dispatcherTimer.Start();
                string message = success ? "Connection successful!" : "Connection could not be established!";
                ConnectionBlock.Text = message;
                ConnectionBlock.Foreground =
                    success
                        ? new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString(
                                Common.Settings.CurrentTheme.Colors["Positive Button"]))
                        : new SolidColorBrush(ColorHelper.AdjustBrightness(
                            (Color)ColorConverter.ConvertFromString(
                                Common.Settings.CurrentTheme.Colors["Negative Button"]),
                            1.2));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                button.Enable();
            }
        }
    }


    private void CreatePrefEntry(Panel parent, string blockText = "", string bindPath = "", Thickness margin = new(),
        object? bindSource = null, double boxWidth = 200)
    {
        bindSource ??= Common.Settings;

        StackPanel prefPanel = new StackPanel
        {
            Margin = margin,
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        TextBlock prefTextBlock =
            UIHelper.CreateTextBlock(text: blockText, margin: new Thickness(5, 0, 0, 5), isBold: false,
                vA: VerticalAlignment.Center, fontSize: 15);
        BindingHelper.SetColorBinding(prefTextBlock, ForegroundProperty, "Font");

        TextBox prefTextBox = UIHelper.CreateTextBox(width: boxWidth);
        Binding boxBinding = new Binding(bindPath)
        {
            Source = bindSource, Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        };
        BindingOperations.SetBinding(prefTextBox, TextBox.TextProperty, boxBinding);
        prefTextBox.Margin = new Thickness(0, 0, 0, 10);

        prefPanel.Children.Add(prefTextBlock);
        prefPanel.Children.Add(prefTextBox);
        parent.Children.Add(prefPanel);
    }
}