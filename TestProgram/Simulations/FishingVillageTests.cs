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
            Program.ComparePlayers(Strategies.BigMoney.Player(1), Strategies.BigMoneyFishingVillageOverSilver.Player(2));
            Program.ComparePlayers(Strategies.BigMoney.Player(1), Strategies.BigMoneyFishingVillageAvailableForDeckCycle.Player(2));
            Program.ComparePlayers(Strategies.BigMoney.Player(1), Strategies.BigMoneyFishingVillageEmptyDuration.Player(2));
            Program.ComparePlayers(Strategies.BigMoneyFishingVillageAvailableForDeckCycle.Player(1), Strategies.BigMoneyFishingVillageEmptyDuration.Player(2));

            Program.ComparePlayers(Strategies.BigMoneyDoubleJack.Player(1), Strategies.BigMoney.Player(2));
            Program.ComparePlayers(Strategies.BigMoneyWithSilverReplacement.Player(Cards.FishingVillage, "BigMoneyWithFishingDoubleJack", 1, Cards.JackOfAllTrades, count: 2), Strategies.BigMoney.Player(2));
            Program.ComparePlayers(Strategies.BigMoneyDoubleJack.Player(1), Strategies.BigMoneyWithSilverReplacement.Player(Cards.FishingVillage, "BigMoneyWithFishingDoubleJack", 2, Cards.JackOfAllTrades, count:2));
        }
    }
}