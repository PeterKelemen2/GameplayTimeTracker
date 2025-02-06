using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace GameplayTimeTracker;

public class GameCardRepository
{
    public List<GameCard> GameCards { get; set; }

    public GameCardRepository()
    {
        GameCards = new List<GameCard>();
    }

    public void LoadCards(EntryRepository entryRepository, Panel ParentPanel)
    {
        ParentPanel.Children.Clear();
        GameCards = new List<GameCard>();
        foreach (var entry in entryRepository.EntriesList)
        {
            GameCard gc = new GameCard();
            switch (Common.Settings.Display)
            {
                case GameDisplay.Vertical:
                    gc = new GameCardVertical(entry, entryRepository, this, ParentPanel);
                    break;
                case GameDisplay.Horizontal:
                    gc = new GameCardHorizontal(entry, entryRepository, this, ParentPanel);
                    break;
                case GameDisplay.Compact:
                    gc = new GameCardCompact(entry, entryRepository, this, ParentPanel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            GameCards.Add(gc);
            ParentPanel.Children.Add(gc);
        }
    }

    public void RemoveCard(GameCard card)
    {
        GameCards.Remove(card);
    }
}