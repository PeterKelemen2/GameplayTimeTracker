using System;
using System.Windows.Controls;

namespace GameplayTimeTracker;

public class GameCardHorizontal : GameCard
{
    public GameCardHorizontal(Entry dataEntry, GameCardRepository gameCardRepository, StackPanel parentStackPanel) :
        base(dataEntry, gameCardRepository, parentStackPanel)
    {
    }
}