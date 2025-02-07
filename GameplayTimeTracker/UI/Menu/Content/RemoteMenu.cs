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
    public RemoteMenu()
    {
        PrefEntry savingEnabledPref = new PrefEntry("Remote Saving Enabled", Common.Settings.IsRemoteSavingEnabled);
        Binding savingEnabledBinding = new Binding("IsRemoteSavingEnabled")
            { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(savingEnabledPref.toggleButton, CustomToggleButton.IsToggledProperty,
            savingEnabledBinding);
        BindingHelper.SetColorBinding(savingEnabledPref.textBlock, ForegroundProperty, "Font");
        _stackPanel.Children.Add(savingEnabledPref);
    }
}