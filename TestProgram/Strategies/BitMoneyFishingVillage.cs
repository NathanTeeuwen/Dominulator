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
        public static class BigMoneyWithSilverReplacement<T>
            where T : Card, new()
        {
            public static PlayerAction Player(string strategyName, int playerNumber, Card secondCard = null, int count = 1)
            {
                return new PlayerAction(
                            strategyName,
                            playerNumber,                            
                            purchaseOrder: PurchaseOrder(secondCard, count));
            }

            private static CardPickByPriority PurchaseOrder(Card withCard, int count)
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(Default.ShouldBuyProvinces),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => gameState.GetPile<CardTypes.Province>().Count <= 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count <= 2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           new CardAcceptance(withCard, gameState => CountAllOwned(withCard.GetType(), gameState) < count),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count < 4),
                           CardAcceptance.For<T>());
            }   
        }
   
        public static class BigMoneyFishingVillageOverSilver
        {
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithSilverReplacement<CardTypes.FishingVillage>.Player(
                            "BigMoneyFishingVillageOverSilver",
                            playerNumber);
            }
        }

        public static class BigMoneyFishingVillageAvailableForDeckCycle
        {
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithSilverReplacement<CardTypes.TestCards.FishingVillageAvailableForDeckCycle>.Player(
                            "BigMoneyFishingVillageAvailableForDeckCycle",
                            playerNumber);
            }
        }

        public static class BigMoneyFishingVillageEmptyDuration
        {
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithSilverReplacement<CardTypes.TestCards.FishingVillageEmptyDuration>.Player(
                            "BigMoneyFishingVillageEmptyDuration",
                            playerNumber);
            }
        }
    }
}
