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
        public static PlayerAction Player(Card card)
        {
            return new PlayerAction(
                        string.Format("GardensCounterPlay({0})", card.name),
                        purchaseOrder: PurchaseOrder(card),
                        gainOrder: PurchaseOrder(card));
        }

        private static CardPickByPriority PurchaseOrder(Card card)
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => gameState.GetPile(Cards.Gardens).Count < 8),
                        CardAcceptance.For(Cards.Gardens, gameState => gameState.GetPile(Cards.Gardens).Count < 8 && gameState.Self.AllOwnedCards.CountOf(Cards.Gardens) < 4),
                        CardAcceptance.For(Cards.Province, gameState => gameState.Self.AllOwnedCards.CountOf(Cards.Gold) >= 2),
                        CardAcceptance.For(Cards.Province, gameState => gameState.GetPile(Cards.Gardens).Count < 8),
                        CardAcceptance.For(Cards.Duchy, gameState => gameState.GetPile(Cards.Province).Count <= 4),
                        CardAcceptance.For(Cards.Duchy, gameState => gameState.GetPile(Cards.Estate).Count <= 4),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(card, 2),
                        CardAcceptance.For(Cards.Estate, gameState => gameState.GetPile(Cards.Estate).Count <= 4),
                        CardAcceptance.For(Cards.Silver));
        }
    }
}
