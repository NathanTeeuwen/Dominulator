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
                            treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: Default.EmptyPickOrder(),
                            discardOrder: DiscardOrder());
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                     CardAcceptance.For(CardTypes.Embassy.card, gameState => CountAllOwned(CardTypes.Embassy.card, gameState) < 1),
                     CardAcceptance.For(CardTypes.Duchy.card),
                     CardAcceptance.For(CardTypes.Duke.card));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Warehouse.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Warehouse.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Warehouse.card));

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Silver.card));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Warehouse.card),
                           CardAcceptance.For(CardTypes.Embassy.card));
            }

            private static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.Duchy.card),
                    CardAcceptance.For(CardTypes.Duke.card),
                    CardAcceptance.For(CardTypes.Estate.card),
                    CardAcceptance.For(CardTypes.Copper.card),
                    CardAcceptance.For(CardTypes.Warehouse.card),
                    CardAcceptance.For(CardTypes.Copper.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Embassy.card),
                    CardAcceptance.For(CardTypes.Gold.card));
            }
        }
    }
}
