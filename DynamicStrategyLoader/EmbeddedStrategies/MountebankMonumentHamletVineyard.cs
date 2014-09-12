using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    public class MountebankMonumentHamletVineyard
        : Strategy
    {
        public static PlayerAction Player()
        {
            return new MyPlayerAction();
        }

        class MyPlayerAction
            : PlayerAction
        {
            public MyPlayerAction()
                : base("MountebankMonumentHamletVineyard",
                    purchaseOrder: PurchaseOrder(),
                    actionOrder: ActionOrder())
            {
            }
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Vineyard, gameState => CountOfPile(Cards.Province, gameState) < 4 || CountOfPile(Cards.Duchy, gameState) < 4 || CountOfPile(Cards.Vineyard, gameState) < 4),
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.ScryingPool),
                        CardAcceptance.For(Cards.Vineyard),
                        CardAcceptance.For(Cards.GrandMarket),
                        CardAcceptance.For(Cards.Mountebank, 2),
                        CardAcceptance.For(Cards.Monument, 1),
                        CardAcceptance.For(Cards.Silver, 1),
                        CardAcceptance.For(Cards.Potion, 4),
                        CardAcceptance.For(Cards.Mountebank),
                        CardAcceptance.For(Cards.Hamlet, gameState => CountOfPile(Cards.Hamlet, gameState) >= 2),
                //CardAcceptance.For(Cards.Hamlet),                           
                        CardAcceptance.For(Cards.FarmingVillage));
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.ScryingPool),
                        CardAcceptance.For(Cards.GrandMarket),
                        CardAcceptance.For(Cards.Hamlet),
                        CardAcceptance.For(Cards.Mountebank),
                        CardAcceptance.For(Cards.Monument));
        }
    }
}