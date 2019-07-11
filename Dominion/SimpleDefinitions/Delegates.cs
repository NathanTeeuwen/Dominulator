using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public delegate int VictoryPointCounter(PlayerState player);
    public delegate void GameStateMethod(PlayerState currentPlayer, GameState gameState);
    public delegate void GameStateCardMethod(PlayerState currentPlayer, GameState gameState, Card card);
    public delegate bool GameStateCardPredicate(PlayerState currentPlayer, GameState gameState, Card card);
    public delegate DeckPlacement GameStateCardToPlacement(PlayerState currentPlayer, GameState gameState, Card card);
    public delegate int CardIntValue(Card card);    

    public delegate DeckPlacement MapCardToPlacement(Card card);
    public delegate CardPlacementPair PlaceCardsFromList(BagOfCards cards);
    public delegate bool IsValidChoice(PlayerActionChoice availableChoice);

    public delegate bool CardPredicate(Card card);
    public delegate bool EventPredicate(Event card);
    public delegate bool GameStatePredicate(GameState gameState);
    public delegate int GameStateIntValue(GameState gameState);

    public delegate IEnumerable<CardCountPair> MapPlayerGameConfigToCardSet(int playerPosition, GameConfig gameConfig);

    static class Delegates
    {
        public static bool AlwaysTrueCardPredicate(Card card)
        {
            return true;
        }

        public static bool IsActionCardPredicate(Card card)
        {
            return card.isAction;
        }

        public static bool IsTreasureCardPredicate(Card card)
        {
            return card.isTreasure;
        }
    }
}
