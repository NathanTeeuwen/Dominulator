using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

namespace Strategies
{    
    public class BigMoneyWithSilverReplacement
        : Strategy 
    {
        public static PlayerAction Player(Card card, string strategyName, Card secondCard = null, int count = 1)
        {
            return new PlayerAction(
                        strategyName,                            
                        purchaseOrder: PurchaseOrder(card, secondCard, count));
        }

        private static CardPickByPriority PurchaseOrder(Card card, Card withCard, int count)
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, DefaultStrategies.ShouldBuyProvinces),
                        CardAcceptance.For(Cards.Duchy, gameState => gameState.GetPile(Cards.Province).Count <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => gameState.GetPile(Cards.Province).Count <= 2),
                        CardAcceptance.For(Cards.Gold),
                        new CardAcceptance(withCard, gameState => CountAllOwned(withCard, gameState) < count),
                        CardAcceptance.For(Cards.Estate, gameState => gameState.GetPile(Cards.Province).Count < 4),
                        CardAcceptance.For(card));
        }   
    }
   
    public class BigMoneyFishingVillageOverSilver
        : Strategy 
    {
        public static PlayerAction Player(int playerNumber)
        {
            return BigMoneyWithSilverReplacement.Player(
                        Cards.FishingVillage,
                        "BigMoneyFishingVillageOverSilver");
        }
    }

    public class BigMoneyFishingVillageAvailableForDeckCycle
        : Strategy 
    {
        public static PlayerAction Player(int playerNumber)
        {
            return BigMoneyWithSilverReplacement.Player(
                        CardTypes.TestCards.FishingVillageAvailableForDeckCycle.card,
                        "BigMoneyFishingVillageAvailableForDeckCycle");
        }
    }

    public class BigMoneyFishingVillageEmptyDuration
        : Strategy 
    {
        public static PlayerAction Player(int playerNumber)
        {
            return BigMoneyWithSilverReplacement.Player(
                        CardTypes.TestCards.FishingVillageEmptyDuration.card,
                        "BigMoneyFishingVillageEmptyDuration");
        }
    }
}
