using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public static partial class Strategies
    {
        public static class BigMoneySimple
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
}
