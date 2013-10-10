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
        private static class MountebankGovernor
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
                    : base("MountebankGovernor",
                        playerNumber,
                        purchaseOrder: PurchaseOrder(),                        
                        trashOrder: TrashOrder())
                {
                }

                public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    if (HasCardInHand<CardTypes.Gold>(gameState))
                        return PlayerActionChoice.Trash;
                    else
                        return PlayerActionChoice.PlusCard;
                }
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 2),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 2),                           
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Mountebank>(gameState => CountAllOwned<CardTypes.Mountebank>(gameState) < 2),
                           CardAcceptance.For<CardTypes.Governor>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 4),
                           CardAcceptance.For<CardTypes.Silver>(CardAcceptance.AlwaysMatch, CardAcceptance.OverPayZero));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Gold>());
            }
        }
    }
}