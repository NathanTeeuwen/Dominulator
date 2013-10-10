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
                    
                    if (CountOfPile<CardTypes.Duchy>(gameState) == 0)
                    {
                        return Card.Type<CardTypes.Estate>();
                    }

                    if (CountInDeckAndDiscard<CardTypes.Province>(gameState) > 0)
                    {
                        return Card.Type<CardTypes.Province>();
                    }

                    if (CountInDeckAndDiscard<CardTypes.Estate>(gameState) > 0)
                    {
                        return Card.Type<CardTypes.Duchy>();
                    }                    
                    
                    return Card.Type<CardTypes.Province>();
                }
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountAllOwned<CardTypes.Estate>(gameState) < CountAllOwned<CardTypes.Rebuild>(gameState)),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 2 || CountOfPile<CardTypes.Duchy>(gameState) == 0),
                           CardAcceptance.For<CardTypes.Rebuild>(gameState => CountAllOwned<CardTypes.Rebuild>(gameState) < 3),
                           CardAcceptance.For<CardTypes.Gold>(),                           
                           CardAcceptance.For<CardTypes.Silver>());
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Rebuild>());
            }                        
        }
    }
}
