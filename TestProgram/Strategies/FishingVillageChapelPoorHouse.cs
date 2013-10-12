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
                            treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder(),
                            discardOrder: Default.EmptyPickOrder());
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Province.card),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(CardTypes.FishingVillage.card),
                    CardAcceptance.For(CardTypes.Chapel.card));

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(CardTypes.FishingVillage.card, gameState => CountAllOwned(CardTypes.FishingVillage.card, gameState) < 2),
                           CardAcceptance.For(CardTypes.PoorHouse.card));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.FishingVillage.card),
                           CardAcceptance.For(CardTypes.Chapel.card, ShouldPlayChapel),
                           CardAcceptance.For(CardTypes.PoorHouse.card));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Estate.card),
                           CardAcceptance.For(CardTypes.Copper.card));
            }

            private static bool ShouldPlayChapel(GameState gameState)
            {
                return HasCardFromInHand(TrashOrder(), gameState);
            }
        }
    }
}
