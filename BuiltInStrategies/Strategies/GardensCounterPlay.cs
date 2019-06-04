using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    public class GardensCounterPlay
        : Strategy
    {
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "GardensCounterPlay",
                        purchaseOrder: PurchaseOrder(),
                        gainOrder: PurchaseOrder());
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => gameState.GetPile(Cards.Gardens).Count < 8),
                        CardAcceptance.For(Cards.Gardens, gameState => gameState.GetPile(Cards.Gardens).Count < 8 && gameState.Self.AllOwnedCards.CountOf(Cards.Gardens) < 4),
                        CardAcceptance.For(Cards.Province, gameState => gameState.Self.AllOwnedCards.CountOf(Cards.Gold) >= 2),
                        CardAcceptance.For(Cards.Province, gameState => gameState.GetPile(Cards.Gardens).Count < 8),
                        CardAcceptance.For(Cards.Duchy, gameState => gameState.GetPile(Cards.Province).Count <= 4),
                        CardAcceptance.For(Cards.Duchy, gameState => gameState.GetPile(Cards.Estate).Count <= 4),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Smithy, 2),
                        CardAcceptance.For(Cards.Estate, gameState => gameState.GetPile(Cards.Estate).Count <= 4),
                        CardAcceptance.For(Cards.Silver));
        }
    }
}
