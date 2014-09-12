using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{    
    public class FishingVillageChapelPoorHouseTalisman
        : Strategy
    {
            
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "FishingVillageChapelPoorHouseTalisman",            
                        purchaseOrder: PurchaseOrder(),
                        treasurePlayOrder: DefaultStrategies.DefaultTreasurePlayOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder(),
                        discardOrder: DefaultStrategies.EmptyPickOrder());
        }

        private static ICardPicker PurchaseOrder()
        {
            var highPriority = new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, ShouldBuyProvince),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 4));

            var buildOrder = new CardPickByBuildOrder(
                CardAcceptance.For(Cards.Talisman),
                CardAcceptance.For(Cards.Chapel));

            var lowPriority = new CardPickByPriority(
                        CardAcceptance.For(Cards.FishingVillage, ShouldBuyFishingVillage),
                        CardAcceptance.For(Cards.PoorHouse, ShouldBuyAnything));

            return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.FishingVillage),
                        CardAcceptance.For(Cards.Chapel, ShouldPlayChapel),
                        CardAcceptance.For(Cards.PoorHouse));            
        }

        private static bool ShouldBuyAnything(GameState gameState)
        {
            return CountAllOwned(Cards.Estate, gameState) + CountAllOwned(Cards.Copper, gameState) <= 2;
        }

        private static bool ShouldBuyFishingVillage(GameState gameState)
        {
            return ShouldBuyAnything(gameState) && CountAllOwned(Cards.FishingVillage, gameState) < 2;
        }

        private static bool ShouldBuyProvince(GameState gameState)
        {
            return CountAllOwned(Cards.PoorHouse, gameState) >= 4 && CountAllOwned(Cards.FishingVillage, gameState) >= 2;
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
