using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GameplayTimeTracker.Settings;
using GameplayTimeTracker.SGDB;


namespace GameplayTimeTracker.Menu;

public class AddMenu : EntryConfigMenu
{
    public AddMenu(Entry entry, EntryRepository entryRepo, GameCardRepository cardRepo, Panel panel,
        double width = 350, double height = 550, bool performanceMode = true)
        : base(entry, width, height, performanceMode)
    {
        TitleTextBlock.Text = "Configure new entry";
        ConfirmButton.Click += (_, _) =>
        {
            AddConfiguredEntry(entry, entryRepo, cardRepo, panel);
            Close();
        };

        AppSettings settings = DataHandler.GetSettingsFromFile(AppFiles.SettingsFilePath);
        Dictionary<string, string> iconFiles = SGDBFileHandler.GetSGDBFiles();

        if (!settings.SGDBApiKey.Equals(string.Empty))
        {
            Task.Run(async () => await SGDBFetch.FetchSGDBAsync(settings.SGDBApiKey, entry.Name, iconFiles)).Wait();
        }

        entry.IconPath = iconFiles["icon"];
        entry.HeroPath = iconFiles["hero"];
    }

    private void AddConfiguredEntry(Entry entry, EntryRepository entryRepo, GameCardRepository cardRepo, Panel panel)
    {
        entryRepo.AddEntry(entry);
        GameCard gc = new GameCardVertical(entry, entryRepo, cardRepo, panel);
        cardRepo.GameCards.Add(gc);
        panel.Children.Add(gc);
    }
}