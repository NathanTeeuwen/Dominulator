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
