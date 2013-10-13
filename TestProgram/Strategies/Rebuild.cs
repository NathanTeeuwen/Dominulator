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
        public static class Rebuild
        {
            
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("Rebuild",
                        playerNumber,
                        purchaseOrder: PurchaseOrder(),
                        treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                        actionOrder: ActionOrder())
                {
                }

                public override Card NameACard(GameState gameState)
                {

                    PlayerState self = gameState.Self;
                    
                    if (CountOfPile(Cards.Duchy, gameState) == 0)
                    {
                        return Cards.Estate;
                    }

                    if (CountInDeckAndDiscard(Cards.Province, gameState) > 0)
                    {
                        return Cards.Province;
                    }

                    if (CountInDeckAndDiscard(Cards.Estate, gameState) > 0)
                    {
                        return Cards.Duchy;
                    }                    
                    
                    return Cards.Province;
                }
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Province),
                           CardAcceptance.For(Cards.Duchy, gameState => CountAllOwned(Cards.Estate, gameState) < CountAllOwned(Cards.Rebuild, gameState)),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2 || CountOfPile(Cards.Duchy, gameState) == 0),
                           CardAcceptance.For(Cards.Rebuild, gameState => CountAllOwned(Cards.Rebuild, gameState) < 3),
                           CardAcceptance.For(Cards.Gold),                           
                           CardAcceptance.For(Cards.Silver));
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Rebuild));
            }                        
        }
    }
}
