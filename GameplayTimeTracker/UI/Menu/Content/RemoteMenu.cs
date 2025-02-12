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
        PrefEntry savingEnabledPref = new PrefEntry("Remote Save Enabled", Common.Settings.IsRemoteSavingEnabled);
        Binding savingEnabledBinding = new Binding("IsRemoteSavingEnabled")
            { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(savingEnabledPref.toggleButton, CustomToggleButton.IsToggledProperty,
            savingEnabledBinding);
        BindingHelper.SetColorBinding(savingEnabledPref.textBlock, ForegroundProperty, "Font");
        _stackPanel.Children.Add(savingEnabledPref);

        RemoteConfigPanel = new StackPanel();
        _stackPanel.Children.Add(RemoteConfigPanel);

        TextBlock RemoteMachineTitle =
            UIHelper.CreateTextBlock("Remote Machine", hA: HorizontalAlignment.Center, fontSize: 17);
        RemoteMachineTitle.Margin = new Thickness(0, 5, 0, 5);
        RemoteConfigPanel.Children.Add(RemoteMachineTitle);
        Thickness prefMargin = new Thickness(10, 0, 10, 0);
        double prefBoxWidth = 140;
        StackPanel row1 = new StackPanel
            { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
        CreatePrefEntry(row1, "Address", "RemoteMachine.Address", margin: prefMargin, boxWidth: prefBoxWidth);
        CreatePrefEntry(row1, "Port", "RemoteMachine.Port", margin: prefMargin, boxWidth: prefBoxWidth);
        RemoteConfigPanel.Children.Add(row1);

        StackPanel row2 = new StackPanel
            { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
        CreatePrefEntry(row2, "User", "RemoteMachine.User", margin: prefMargin, boxWidth: prefBoxWidth);
        CreatePrefEntry(row2, "Password", "RemoteMachine.Password", margin: prefMargin, boxWidth: prefBoxWidth);
        RemoteConfigPanel.Children.Add(row2);

        CreatePrefEntry(RemoteConfigPanel, "Remote folder", "RemoteMachine.RemoteFolder", new Thickness(0, 0, 0, 10));

        TextBlock PreferencesTitle =
            UIHelper.CreateTextBlock("Preferences", hA: HorizontalAlignment.Center, fontSize: 17);
        PreferencesTitle.Margin = new Thickness(0, 0, 0, 5);
        RemoteConfigPanel.Children.Add(PreferencesTitle);
        CreatePrefEntry(RemoteConfigPanel, "Retain saves for days", "RemoteMachine.RetainForDays",
            new Thickness(0, 0, 0, 5));
        CreatePrefEntry(RemoteConfigPanel, "Save if session longer (m)", "RemoteMachine.BackupIfSessionLonger",
            new Thickness(0, 0, 0, 20));
    }

    private void CreatePrefEntry(Panel parent, string blockText = "", string bindPath = "", Thickness margin = new(),
        double boxWidth = 200)
    {
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
        Binding boxBinding = new Binding(bindPath) { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(prefTextBox, TextBox.TextProperty, boxBinding);
        prefTextBox.Margin = new Thickness(0, 0, 0, 10);

        prefPanel.Children.Add(prefTextBlock);
        prefPanel.Children.Add(prefTextBox);
        parent.Children.Add(prefPanel);
    }
}