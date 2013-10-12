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
        public static class ProcessionGraverobber
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "ProcessionGraverobber",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder());
            }

            private static ICardPicker PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(),
                           CardAcceptance.For<CardTypes.Graverobber>(),
                           CardAcceptance.For<CardTypes.Militia>(1),
                           CardAcceptance.For<CardTypes.Feast>(),
                           CardAcceptance.For<CardTypes.Procession>(),
                           CardAcceptance.For<CardTypes.Silver>(1),
                           CardAcceptance.For<CardTypes.Village>());
            }

            private static ICardPicker ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Procession>(),
                           CardAcceptance.For<CardTypes.Graverobber>(gameState => CardBeingPlayedIs<CardTypes.Procession>(gameState)),
                           CardAcceptance.For<CardTypes.Feast>(gameState => CardBeingPlayedIs<CardTypes.Procession>(gameState)),
                           CardAcceptance.For<CardTypes.Village>(),
                           CardAcceptance.For<CardTypes.Graverobber>(),
                           CardAcceptance.For<CardTypes.Militia>(1),
                           CardAcceptance.For<CardTypes.Feast>(),                           
                           CardAcceptance.For<CardTypes.Silver>(1));
            }

            private static ICardPicker TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Graverobber>(),
                           CardAcceptance.For<CardTypes.Feast>(),
                           CardAcceptance.For<CardTypes.Village>(),
                           CardAcceptance.For<CardTypes.Militia>());
            }
        }
    }
}