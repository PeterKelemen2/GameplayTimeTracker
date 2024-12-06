using System.Windows.Controls;
using System.Windows.Input;

namespace GameplayTimeTracker;

public class PrefMenu : UserControl
{
    public Grid container { get; set; }
    public StackPanel Panel { get; set; }
    public TextBlock Label { get; set; }

    public Settings CurrentSettings { get; set; }
    public bool StartWithSystem { get; set; }

    public PrefMenu(StackPanel stackPanel, Settings settings)
    {
        Panel = stackPanel;
        CurrentSettings = settings;
        StartWithSystem = CurrentSettings.StartWithSystem;
        // CreateMenu();
    }

    public void CreateMenuMethod()
    {
        Panel.Children.Clear();

        PrefEntry newEntry = new PrefEntry(Panel, "Start with system", StartWithSystem);
        newEntry.checkBox.Checked += (sender, e) => SaveToFile(newEntry.checkBox.IsChecked ?? false);
        newEntry.checkBox.Unchecked += (sender, e) => SaveToFile(newEntry.checkBox.IsChecked ?? false);

        Panel.Children.Add(newEntry);
    }

    private void SaveToFile(bool value)
    {
        JsonHandler jsonHandler = new JsonHandler();
        jsonHandler.WriteStartWithSystemToFile(value);
        StartWithSystem = value;
    }


    public void CreateMenu(object sender, MouseButtonEventArgs e)
    {
        CreateMenuMethod();
    }
}