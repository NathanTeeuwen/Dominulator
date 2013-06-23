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
        public static class FishingVillageChapelPoorHouseTalisman
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "FishingVillageChapelPoorHouseTalisman",
                            playerNumber,                            
                            purchaseOrder: PurchaseOrder(),
                            treasurePlayOrder: Default.TreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder(),
                            discardOrder: Default.EmptyPickOrder());
            }

            private static IGetMatchingCard PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(ShouldBuyProvince),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 4));

                var buildOrder = new CardPickByBuildOrder(
                    new CardTypes.Talisman(),
                    new CardTypes.Chapel());

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For<CardTypes.FishingVillage>(ShouldBuyFishingVillage),
                           CardAcceptance.For<CardTypes.PoorHouse>(ShouldBuyAnything));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.FishingVillage>(),
                           CardAcceptance.For<CardTypes.Chapel>(ShouldPlayChapel),
                           CardAcceptance.For<CardTypes.PoorHouse>());            
            }

            private static bool ShouldBuyAnything(GameState gameState)
            {
                return CountAllOwned<CardTypes.Estate>(gameState) + CountAllOwned<CardTypes.Copper>(gameState) <= 2;
            }

            private static bool ShouldBuyFishingVillage(GameState gameState)
            {
                return ShouldBuyAnything(gameState) && CountAllOwned<CardTypes.FishingVillage>(gameState) < 2;
            }

            private static bool ShouldBuyProvince(GameState gameState)
            {
                return CountAllOwned<CardTypes.PoorHouse>(gameState) >= 4 && CountAllOwned<CardTypes.FishingVillage>(gameState) >= 2;
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
