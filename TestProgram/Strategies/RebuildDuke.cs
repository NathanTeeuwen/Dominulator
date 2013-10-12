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
                    return CardTypes.Duchy.card;
                }
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Rebuild>(gameState => CountAllOwned<CardTypes.Rebuild>(gameState) < 2),
                           CardAcceptance.For<CardTypes.Duchy>(),
                           CardAcceptance.For<CardTypes.Province>(),
                           CardAcceptance.For<CardTypes.Duke>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountAllOwned<CardTypes.Estate>(gameState) == 0 && CountAllOwned<CardTypes.Rebuild>(gameState) >= 2),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Duchy>(gameState) == 0),
                           CardAcceptance.For<CardTypes.Rebuild>(),
                           CardAcceptance.For<CardTypes.Silver>());
            }            
        }
    }
}