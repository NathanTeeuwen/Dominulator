using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Strategies
{    
    public class BigMoneySimple
        : Strategy 
    {
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "BigMoneySimple",                            
                        purchaseOrder: PurchaseOrder());
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),                           
                        CardAcceptance.For(Cards.Gold),                           
                        CardAcceptance.For(Cards.Silver));
        }
    }
}
