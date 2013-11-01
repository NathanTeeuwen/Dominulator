using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

namespace Strategies
{    
    public class TreasureMap
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
                : base("TreasureMap",                        
                    purchaseOrder: PurchaseOrder(),                        
                    actionOrder: ActionOrder())
            {
            }                
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 5),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.TreasureMap, gameState => CountAllOwned(Cards.Gold, gameState) == 0),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.Silver));
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.TreasureMap, gameState => CountInHand(Cards.TreasureMap, gameState) == 2 || CountAllOwned(Cards.Gold, gameState) > 0));
        }                        
    }
}
