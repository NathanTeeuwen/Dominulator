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
        public static class DoubleWarehouse2
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "DoubleWarehouse2",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: Default.EmptyPickOrder(),
                            discardOrder: DiscardOrder());
            }

            static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                         CardAcceptance.For(CardTypes.Province.card, gameState => gameState.players.CurrentPlayer.AllOwnedCards.CountOf(CardTypes.Gold.card) > 2),
                         CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 5),
                         CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 2),
                         CardAcceptance.For(CardTypes.Gold.card));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Warehouse.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Warehouse.card));

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Silver.card));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.Warehouse.card));
            }

            static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.Province.card),
                    CardAcceptance.For(CardTypes.Duchy.card),
                    CardAcceptance.For(CardTypes.Estate.card),
                    CardAcceptance.For(CardTypes.Copper.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Warehouse.card),
                    CardAcceptance.For(CardTypes.Gold.card));
            }
        }
    }
}
