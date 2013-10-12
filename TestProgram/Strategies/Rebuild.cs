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
            // big money smithy player
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
                    
                    if (CountOfPile(CardTypes.Duchy.card, gameState) == 0)
                    {
                        return CardTypes.Estate.card;
                    }

                    if (CountInDeckAndDiscard(CardTypes.Province.card, gameState) > 0)
                    {
                        return CardTypes.Province.card;
                    }

                    if (CountInDeckAndDiscard(CardTypes.Estate.card, gameState) > 0)
                    {
                        return CardTypes.Duchy.card;
                    }                    
                    
                    return CardTypes.Province.card;
                }
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Province.card),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountAllOwned(CardTypes.Estate.card, gameState) < CountAllOwned(CardTypes.Rebuild.card, gameState)),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 2 || CountOfPile(CardTypes.Duchy.card, gameState) == 0),
                           CardAcceptance.For(CardTypes.Rebuild.card, gameState => CountAllOwned(CardTypes.Rebuild.card, gameState) < 3),
                           CardAcceptance.For(CardTypes.Gold.card),                           
                           CardAcceptance.For(CardTypes.Silver.card));
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Rebuild.card));
            }                        
        }
    }
}
