using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CardTypes = Dominion.CardTypes;
using Dominion;
using Dominion.Strategy;
using Dominion.Data;

namespace TestCompareStrategyVsAllKnownStrategies
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var testOutput = new TestOutput())
            {
                var playerAction = Strategies.BigMoneyWithCard.Player(Cards.Magpie, cardCount:2, playerName: "DoubleMagpie");
                foreach (PlayerAction otherPlayerAction in BuiltInStrategies.StrategyLoader.GetAllPlayerActions())
                {
                    if (playerAction == otherPlayerAction)
                        continue;

                    testOutput.ComparePlayers(
                        playerAction,
                        otherPlayerAction,
                        shouldParallel: true,                        
                        logGameCount: 0,
                        debugLogs: false,
                        numberOfGames: 1000,
                        useColonyAndPlatinum: true,
                        createHtmlReport: false,
                        createRankingReport: true);
                }                
            }
        }        
    }
}
