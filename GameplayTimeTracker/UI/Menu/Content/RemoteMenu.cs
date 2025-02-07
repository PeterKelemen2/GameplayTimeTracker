using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace GameplayTimeTracker.Menu.Content;

public class RemoteMenu : UserControl
{
    public StackPanel Panel;

    public RemoteMenu()
    {
        Panel = new StackPanel();
        PrefEntry savingEnabledPref = new PrefEntry("Remote Saving Enabled", Common.Settings.IsRemoteSavingEnabled);
        Binding savingEnabledBinding = new Binding("IsRemoteSavingEnabled")
            { Source = Common.Settings, Mode = BindingMode.TwoWay, };
        BindingOperations.SetBinding(savingEnabledPref.toggleButton, CustomToggleButton.IsToggledProperty,
            savingEnabledBinding);
        BindingHelper.SetColorBinding(savingEnabledPref.textBlock, ForegroundProperty, "Font");
        Panel.Children.Add(savingEnabledPref);
    }
}