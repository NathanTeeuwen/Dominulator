using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Lookout
    {
        public static bool ShouldPlay(GameState gameState, PlayerAction playerAction)
        {
            int cardCountToTrash = Strategy.CountInDeck(Dominion.Cards.Copper, gameState);

            if (!playerAction.purchaseOrder.DoesCardPickerMatch(gameState, Dominion.Cards.Estate))
            {
                cardCountToTrash += Strategy.CountInDeck(Dominion.Cards.Estate, gameState);
            }

            cardCountToTrash += Strategy.CountInDeck(Dominion.Cards.Curse, gameState);
            cardCountToTrash += Strategy.CountInDeck(Dominion.Cards.Hovel, gameState);
            cardCountToTrash += Strategy.CountInDeck(Dominion.Cards.Necropolis, gameState);
            cardCountToTrash += Strategy.CountInDeck(Dominion.Cards.OvergrownEstate, gameState);

            if (!playerAction.purchaseOrder.DoesCardPickerMatch(gameState, Dominion.Cards.Lookout))
            {
                cardCountToTrash += Strategy.CountInDeck(Dominion.Cards.Lookout, gameState);
            }

            int totalCardsOwned = gameState.Self.CardsInDeck.Count;

            return ((double)cardCountToTrash) / totalCardsOwned > 0.4;
        }
    }
}