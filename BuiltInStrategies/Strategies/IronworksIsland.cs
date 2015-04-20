using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Strategies
{
    public class IronworksIsland
        : Strategy
    {
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "IronworksIsland",
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder());
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Island, gameState => gameState.CurrentContext.CurrentCard == Cards.IronWorks),
                        CardAcceptance.For(Cards.IronWorks),
                        CardAcceptance.For(Cards.Island),
                        CardAcceptance.For(Cards.Duchy),
                        CardAcceptance.For(Cards.Silver, 1),
                        CardAcceptance.For(Cards.Estate));
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.IronWorks),
                        CardAcceptance.For(Cards.Island));
        }
    }
}