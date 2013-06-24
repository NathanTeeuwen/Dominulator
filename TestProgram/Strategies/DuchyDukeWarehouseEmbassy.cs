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
        public static class DuchyDukeWarehouseEmbassy
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "DuchyDukeWarehouseEmbassy",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            treasurePlayOrder: Default.TreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: Default.EmptyPickOrder(),
                            discardOrder: DiscardOrder());
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                     CardAcceptance.For<CardTypes.Embassy>(gameState => CountAllOwned<CardTypes.Embassy>(gameState) < 1),
                     CardAcceptance.For<CardTypes.Duchy>(),
                     CardAcceptance.For<CardTypes.Duke>());

                var buildOrder = new CardPickByBuildOrder(
                    new CardTypes.Silver(),
                    new CardTypes.Warehouse(),
                    new CardTypes.Silver(),
                    new CardTypes.Silver(),
                    new CardTypes.Silver(),
                    new CardTypes.Silver(),
                    new CardTypes.Warehouse(),
                    new CardTypes.Silver(),
                    new CardTypes.Silver(),
                    new CardTypes.Silver(),
                    new CardTypes.Silver(),
                    new CardTypes.Warehouse());

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Silver>());

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Warehouse>(),
                           CardAcceptance.For<CardTypes.Embassy>());
            }

            private static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Duchy>(),
                    CardAcceptance.For<CardTypes.Duke>(),
                    CardAcceptance.For<CardTypes.Estate>(),
                    CardAcceptance.For<CardTypes.Copper>(),
                    CardAcceptance.For<CardTypes.Warehouse>(),
                    CardAcceptance.For<CardTypes.Copper>(),
                    CardAcceptance.For<CardTypes.Silver>(),
                    CardAcceptance.For<CardTypes.Embassy>(),
                    CardAcceptance.For<CardTypes.Gold>());
            }
        }
    }
}
