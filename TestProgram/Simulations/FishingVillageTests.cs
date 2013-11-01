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
        public static void Run()
        {
            Program.ComparePlayers("BigMoney", Strategies.BigMoneyFishingVillageOverSilver.Player(2));
            Program.ComparePlayers("BigMoney", Strategies.BigMoneyFishingVillageAvailableForDeckCycle.Player(2));
            Program.ComparePlayers("BigMoney", Strategies.BigMoneyFishingVillageEmptyDuration.Player(2));
            Program.ComparePlayers(Strategies.BigMoneyFishingVillageAvailableForDeckCycle.Player(1), Strategies.BigMoneyFishingVillageEmptyDuration.Player(2));

            Program.ComparePlayers("BigMoneyDoubleJack", "BigMoney");
            Program.ComparePlayers(Strategies.BigMoneyWithSilverReplacement.Player(Cards.FishingVillage, "BigMoneyWithFishingDoubleJack", Cards.JackOfAllTrades, count: 2), "BigMoney");
            Program.ComparePlayers("BigMoneyDoubleJack", Strategies.BigMoneyWithSilverReplacement.Player(Cards.FishingVillage, "BigMoneyWithFishingDoubleJack", Cards.JackOfAllTrades, count: 2));
        }
    }
}