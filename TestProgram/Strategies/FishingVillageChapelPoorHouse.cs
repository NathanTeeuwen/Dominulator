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
        public static class FishingVillageChapelPoorHouse
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "FishingVillageChapelPoorHouse",
                            playerNumber,                            
                            purchaseOrder: PurchaseOrder(),
                            treasurePlayOrder: Default.TreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder(),
                            discardOrder: Default.EmptyPickOrder());
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 4));

                var buildOrder = new CardPickByBuildOrder(
                    new CardTypes.FishingVillage(),
                    new CardTypes.Chapel());

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For<CardTypes.FishingVillage>(gameState => CountAllOwned<CardTypes.FishingVillage>(gameState) < 2),
                           CardAcceptance.For<CardTypes.PoorHouse>());

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.FishingVillage>(),
                           CardAcceptance.For<CardTypes.Chapel>(ShouldPlayChapel),
                           CardAcceptance.For<CardTypes.PoorHouse>());
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Estate>(),
                           CardAcceptance.For<CardTypes.Copper>());
            }

            private static bool ShouldPlayChapel(GameState gameState)
            {
                return HasCardFromInHand(TrashOrder(), gameState);
            }
        }
    }
}
