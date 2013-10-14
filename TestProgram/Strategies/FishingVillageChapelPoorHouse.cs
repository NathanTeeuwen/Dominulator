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
            
            public static PlayerAction Player()
            {
                return new PlayerAction(
                            "FishingVillageChapelPoorHouse",                            
                            purchaseOrder: PurchaseOrder(),
                            treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder(),
                            discardOrder: Default.EmptyPickOrder());
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                           CardAcceptance.For(Cards.Province),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(Cards.FishingVillage),
                    CardAcceptance.For(Cards.Chapel));

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(Cards.FishingVillage, gameState => CountAllOwned(Cards.FishingVillage, gameState) < 2),
                           CardAcceptance.For(Cards.PoorHouse));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.FishingVillage),
                           CardAcceptance.For(Cards.Chapel, ShouldPlayChapel),
                           CardAcceptance.For(Cards.PoorHouse));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Estate),
                           CardAcceptance.For(Cards.Copper));
            }

            private static bool ShouldPlayChapel(GameState gameState)
            {
                return HasCardFromInHand(TrashOrder(), gameState);
            }
        }
    }
}
