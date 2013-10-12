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
        public static class BigMoneyWithSilverReplacement
        {
            public static PlayerAction Player(Card card, string strategyName, int playerNumber, Card secondCard = null, int count = 1)
            {
                return new PlayerAction(
                            strategyName,
                            playerNumber,                            
                            purchaseOrder: PurchaseOrder(card, secondCard, count));
            }

            private static CardPickByPriority PurchaseOrder(Card card, Card withCard, int count)
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Province.card, Default.ShouldBuyProvinces),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => gameState.GetPile(CardTypes.Province.card).Count <= 4),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => gameState.GetPile(CardTypes.Province.card).Count <= 2),
                           CardAcceptance.For(CardTypes.Gold.card),
                           new CardAcceptance(withCard, gameState => CountAllOwned(withCard, gameState) < count),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => gameState.GetPile(CardTypes.Province.card).Count < 4),
                           CardAcceptance.For(card));
            }   
        }
   
        public static class BigMoneyFishingVillageOverSilver
        {
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithSilverReplacement.Player(
                            CardTypes.FishingVillage.card,
                            "BigMoneyFishingVillageOverSilver",
                            playerNumber);
            }
        }

        public static class BigMoneyFishingVillageAvailableForDeckCycle
        {
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithSilverReplacement.Player(
                            CardTypes.TestCards.FishingVillageAvailableForDeckCycle.card,
                            "BigMoneyFishingVillageAvailableForDeckCycle",
                            playerNumber);
            }
        }

        public static class BigMoneyFishingVillageEmptyDuration
        {
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithSilverReplacement.Player(
                            CardTypes.TestCards.FishingVillageEmptyDuration.card,
                            "BigMoneyFishingVillageEmptyDuration",
                            playerNumber);
            }
        }
    }
}
