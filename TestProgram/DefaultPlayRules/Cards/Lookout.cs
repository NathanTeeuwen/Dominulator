using Dominion;
using System;
using System.Linq;

namespace Program.DefaultStrategies
{
    internal class Lookout
    {
        public static bool ShouldPlay(GameState gameState, PlayerAction playerAction)
        {
            int cardCountToTrash = Strategy.CountInDeck(Cards.Copper, gameState);

            if (!playerAction.purchaseOrder.DoesCardPickerMatch(gameState, Cards.Estate))
            {
                cardCountToTrash += Strategy.CountInDeck(Cards.Estate, gameState);
            }

            cardCountToTrash += Strategy.CountInDeck(Cards.Curse, gameState);
            cardCountToTrash += Strategy.CountInDeck(Cards.Hovel, gameState);
            cardCountToTrash += Strategy.CountInDeck(Cards.Necropolis, gameState);
            cardCountToTrash += Strategy.CountInDeck(Cards.OvergrownEstate, gameState);

            if (!playerAction.purchaseOrder.DoesCardPickerMatch(gameState, Cards.Lookout))
            {
                cardCountToTrash += Strategy.CountInDeck(Cards.Lookout, gameState);
            }

            int totalCardsOwned = gameState.Self.CardsInDeck.Count;

            return ((double)cardCountToTrash) / totalCardsOwned > 0.4;
        }
    }
}