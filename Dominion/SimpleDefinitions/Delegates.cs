using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public delegate int VictoryPointCounter(PlayerState player);
    public delegate DeckPlacement MapCardToPlacement(Card card);
    public delegate CardPlacementPair PlaceCardsFromList(BagOfCards cards);
    public delegate bool IsValidChoice(PlayerActionChoice availableChoice);

    public delegate bool CardPredicate(Card card);
    public delegate bool GameStatePredicate(GameState gameState);
    public delegate int GameStateIntValue(GameState gameState);        
}
