using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{    
    public class Lookout
        : Strategy
    {
            
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "Lookout",                            
                        purchaseOrder: PurchaseOrder(),
                        treasurePlayOrder: DefaultStrategies.DefaultTreasurePlayOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashAndDiscardOrder(),
                        discardOrder: TrashAndDiscardOrder());
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, DefaultStrategies.ShouldBuyProvinces),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.Lookout, gameState => CountAllOwned(Cards.Lookout, gameState) < 1),                           
                        CardAcceptance.For(Cards.Silver));
        }            

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Lookout));
        }            

        private static CardPickByPriority TrashAndDiscardOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.OvergrownEstate),
                        CardAcceptance.For(Cards.Hovel),
                        CardAcceptance.For(Cards.Necropolis),                    
                        CardAcceptance.For(Cards.Copper),
                        CardAcceptance.For(Cards.Lookout),
                        CardAcceptance.For(Cards.Silver),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy),
                        CardAcceptance.For(Cards.Estate));
        }        
    }
}
