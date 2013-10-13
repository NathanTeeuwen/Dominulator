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
        public static class Lookout
        {
            
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "Lookout",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashAndDiscardOrder(),
                            discardOrder: TrashAndDiscardOrder());
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Province, Default.ShouldBuyProvinces),
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
                           CardAcceptance.For(Cards.Lookout, Default.ShouldPlayLookout(Default.ShouldBuyProvinces)));
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
}
