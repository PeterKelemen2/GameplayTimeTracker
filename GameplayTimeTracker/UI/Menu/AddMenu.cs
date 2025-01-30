using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GameplayTimeTracker.Settings;
using GameplayTimeTracker.SGDB;
using Gdk;


namespace GameplayTimeTracker.Menu;

public class AddMenu : EntryConfigMenu
{
    public AddMenu(Entry entry, EntryRepository entryRepo, GameCardRepository cardRepo,
        Panel panel,
        double width = 350, bool toScale = true)
        : base(entry, width, toScale)
    {
        TitleTextBlock.Text = "Configure new entry";
        ConfirmButton.Margin = new Thickness(0, 20, 0, 20);
        ConfirmButton.Click += (_, _) =>
        {
            AddConfiguredEntry(entry, entryRepo, cardRepo, panel);
            Close();
        };

        Dictionary<string, string> iconFiles = SGDBFileHandler.GetSGDBFiles(entry.Name);

        if (!Common.Settings.SGDBApiKey.Equals(string.Empty))
        {
            Task.Run(async () => await SGDBFetch.FetchSGDBAsync(Common.Settings.SGDBApiKey, entry.Name, iconFiles))
                .Wait();
        }

        entry.IconPath = iconFiles["icon"];
        entry.HeroPath = iconFiles["hero"];
    }

    private void AddConfiguredEntry(Entry entry, EntryRepository entryRepo, GameCardRepository cardRepo, Panel panel)
    {
        entryRepo.AddEntry(entry);
        GameCard gc = new GameCard();

        switch (Common.Settings.Display)
        {
            case GameDisplay.Horizontal:
                gc = new GameCardHorizontal(entry, entryRepo, cardRepo, panel);
                break;
            case GameDisplay.Vertical:
                gc = new GameCardVertical(entry, entryRepo, cardRepo, panel);
                break;
        }

        cardRepo.GameCards.Add(gc);
        panel.Children.Add(gc);
    }
}