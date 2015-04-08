using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Strategies
{   
    public class BigMoneyRabbleVsIsland
       : Strategy
    {
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "BigMoneyRabbleVsIsland",
                        purchaseOrder: PurchaseOrder(),
                        trashOrder: TrashOrder());
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Island, gameState) <= 2),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Estate, gameState) <= 6),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Rabble, 2),                       
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.Silver));
        }

        private static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Estate, gameState => CountAllOwned(Cards.Province, gameState) == 0),
                        CardAcceptance.For(Cards.Copper));
        }
    }
}