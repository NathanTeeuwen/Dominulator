using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program.Simulations
{
    static public class FishingVillageTests
    {
        public static void Run(TestOutput testOutput)
        {
            testOutput.ComparePlayers(Strategies.BigMoney.Player(), Strategies.BigMoneyFishingVillageOverSilver.Player(2));
            testOutput.ComparePlayers(Strategies.BigMoney.Player(), Strategies.BigMoneyFishingVillageAvailableForDeckCycle.Player(2));
            testOutput.ComparePlayers(Strategies.BigMoney.Player(), Strategies.BigMoneyFishingVillageEmptyDuration.Player(2));
            testOutput.ComparePlayers(Strategies.BigMoneyFishingVillageAvailableForDeckCycle.Player(1), Strategies.BigMoneyFishingVillageEmptyDuration.Player(2));

            testOutput.ComparePlayers(Strategies.BigMoneyDoubleJack.Player(), Strategies.BigMoney.Player());
            testOutput.ComparePlayers(Strategies.BigMoneyWithSilverReplacement.Player(Cards.FishingVillage, "BigMoneyWithFishingDoubleJack", Cards.JackOfAllTrades, count: 2), Strategies.BigMoney.Player());
            testOutput.ComparePlayers(Strategies.BigMoneyDoubleJack.Player(), Strategies.BigMoneyWithSilverReplacement.Player(Cards.FishingVillage, "BigMoneyWithFishingDoubleJack", Cards.JackOfAllTrades, count: 2));
        }
    }
}