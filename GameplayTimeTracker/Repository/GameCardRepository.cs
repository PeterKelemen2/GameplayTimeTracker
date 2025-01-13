using System.Collections.Generic;

namespace GameplayTimeTracker;

public class GameCardRepository
{
    public List<GameCard> GameCards { get; set; }

    public GameCardRepository()
    {
        GameCards = new List<GameCard>();
    }
}