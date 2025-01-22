using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace GameplayTimeTracker.Menu;

public class AddMenu : EntryConfigMenu
{
    public AddMenu(Entry entry, EntryRepository entryRepo, GameCardRepository cardRepo, Panel panel,
        double width = 350, double height = 550, bool performanceMode = true)
        : base(entry, width, height, performanceMode)
    {
        TitleTextBlock.Text = "Configure new entry";
        ConfirmButton.Click += (_, _) => { AddConfiguredEntry(entry, entryRepo, cardRepo, panel); };
    }

    private void AddConfiguredEntry(Entry entry, EntryRepository entryRepo, GameCardRepository cardRepo, Panel panel)
    {
        entryRepo.AddEntry(entry);
        GameCard gc = new GameCardVertical(entry, entryRepo, cardRepo, panel);
        cardRepo.GameCards.Add(gc);
        panel.Children.Add(gc);
    }
}