using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Author: SheCantSayNo

namespace Program
{
    public static partial class Strategies
    {
        public static class RebuildDuke
        {
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("RebuildDuke",
                        playerNumber,
                        purchaseOrder: PurchaseOrder())
                {
                }

                public override Card NameACard(GameState gameState)
                {
                    return Cards.Duchy;
                }
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Rebuild, gameState => CountAllOwned(Cards.Rebuild, gameState) < 2),
                           CardAcceptance.For(Cards.Duchy),
                           CardAcceptance.For(Cards.Province),
                           CardAcceptance.For(Cards.Duke),
                           CardAcceptance.For(Cards.Estate, gameState => CountAllOwned(Cards.Estate, gameState) == 0 && CountAllOwned(Cards.Rebuild, gameState) >= 2),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Duchy, gameState) == 0),
                           CardAcceptance.For(Cards.Rebuild),
                           CardAcceptance.For(Cards.Silver));
            }            
        }
    }
}