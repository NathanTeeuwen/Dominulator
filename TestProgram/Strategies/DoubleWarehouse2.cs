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
            
            public static PlayerAction Player()
            {
                return new PlayerAction(
                            "DoubleWarehouse2",
                            purchaseOrder: PurchaseOrder(),
                            treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: Default.EmptyPickOrder(),
                            discardOrder: DiscardOrder());
            }

            static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                         CardAcceptance.For(Cards.Province, gameState => gameState.players.CurrentPlayer.AllOwnedCards.CountOf(Cards.Gold) > 2),
                         CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) < 5),
                         CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                         CardAcceptance.For(Cards.Gold));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Warehouse),
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Warehouse));

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(Cards.Silver));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(Cards.Warehouse));
            }

            static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(Cards.Province),
                    CardAcceptance.For(Cards.Duchy),
                    CardAcceptance.For(Cards.Estate),
                    CardAcceptance.For(Cards.Copper),
                    CardAcceptance.For(Cards.Silver),
                    CardAcceptance.For(Cards.Warehouse),
                    CardAcceptance.For(Cards.Gold));
            }
        }
    }
}
