using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    public class GardensWorkshop
        : Strategy
    {
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "GardensWorkshop",
                        purchaseOrder: PurchaseOrder(),
                        gainOrder: PurchaseOrder());
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Workshop, 2),
                        //                        CardAcceptance.For(Cards.Workshop, gameState => gameState.Self.AllOwnedCards.CountOf(Cards.Workshop) * 5 <= gameState.Self.AllOwnedCards.Count),
                        CardAcceptance.For(Cards.Workshop),

                        CardAcceptance.For(Cards.Gardens),
                        CardAcceptance.For(Cards.Workshop),
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.Copper));
        }
    }
}
