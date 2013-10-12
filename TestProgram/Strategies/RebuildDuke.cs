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
                           CardAcceptance.For(CardTypes.Rebuild.card, gameState => CountAllOwned(CardTypes.Rebuild.card, gameState) < 2),
                           CardAcceptance.For(CardTypes.Duchy.card),
                           CardAcceptance.For(CardTypes.Province.card),
                           CardAcceptance.For(CardTypes.Duke.card),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountAllOwned(CardTypes.Estate.card, gameState) == 0 && CountAllOwned(CardTypes.Rebuild.card, gameState) >= 2),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Duchy.card, gameState) == 0),
                           CardAcceptance.For(CardTypes.Rebuild.card),
                           CardAcceptance.For(CardTypes.Silver.card));
            }            
        }
    }
}