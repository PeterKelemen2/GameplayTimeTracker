using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using GameplayTimeTracker.UI.Menu.Content;

namespace GameplayTimeTracker.Menu.Content;

public class RemoteMenu : MenuContent
{
    private StackPanel RemoteConfigPanel;

    public RemoteMenu()
    {
        PrefEntry savingEnabledPref = new PrefEntry("Remote Saving Enabled", Common.Settings.IsRemoteSavingEnabled);
        Binding savingEnabledBinding = new Binding("IsRemoteSavingEnabled")
            { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(savingEnabledPref.toggleButton, CustomToggleButton.IsToggledProperty,
            savingEnabledBinding);
        BindingHelper.SetColorBinding(savingEnabledPref.textBlock, ForegroundProperty, "Font");
        _stackPanel.Children.Add(savingEnabledPref);

        RemoteConfigPanel = new StackPanel();
        _stackPanel.Children.Add(RemoteConfigPanel);

        CreatePrefEntry("Address", "RemoteMachine.Address");
        CreatePrefEntry("Port", "RemoteMachine.Port");
        CreatePrefEntry("User", "RemoteMachine.User");
        CreatePrefEntry("Password", "RemoteMachine.Password");
        CreatePrefEntry("Remote Folder", "RemoteMachine.RemoteFolder", 20);
    }

    private void CreatePrefEntry(string blockText = "", string bindPath = "", double bottomMargin = 0)
    {
        StackPanel prefPanel = new StackPanel
        {
            Margin = new Thickness(0, 0, 0, bottomMargin),
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        TextBlock prefTextBlock =
            UIHelper.CreateTextBlock(text: blockText, margin: new Thickness(5, 0, 0, 2), isBold: false,
                vA: VerticalAlignment.Center);
        BindingHelper.SetColorBinding(prefTextBlock, ForegroundProperty, "Font");

        TextBox prefTextBox = UIHelper.CreateTextBox();
        Binding boxBinding = new Binding(bindPath) { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(prefTextBox, TextBox.TextProperty, boxBinding);
        prefTextBox.Margin = new Thickness(0, 0, 0, 10);

        prefPanel.Children.Add(prefTextBlock);
        prefPanel.Children.Add(prefTextBox);
        RemoteConfigPanel.Children.Add(prefPanel);
    }
}