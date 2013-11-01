using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Strategies
{
    public class BigMoneyCartographer
            : Strategy
    {

        public static PlayerAction Player(Card card, int cardCount = 1)
        {
            return new PlayerAction(
                        "BigMoneyCartographer",
                        purchaseOrder: PurchaseOrder(card, cardCount),
                        actionOrder: ActionOrder(card));
        }

        private static ICardPicker PurchaseOrder(Card card, int cardCount)
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                        CardAcceptance.For(card, gameState => CountAllOwned(card, gameState) < cardCount),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Cartographer),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.Silver));

        }

        private static ICardPicker ActionOrder(Card card)
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Cartographer),
                        CardAcceptance.For(card));
        }
    }
}