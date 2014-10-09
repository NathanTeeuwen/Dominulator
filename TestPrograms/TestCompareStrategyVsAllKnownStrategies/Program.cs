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
                CompareStrategyVsAllKnownStrategies(Strategies.RatsScryingPoolVillagePoorHouseSeahag.Player(), testOutput);
            }
        }

        static void CompareStrategyVsAllKnownStrategies(
           PlayerAction playerAction,
           TestOutput testOutput,
           bool shouldParallel = true,
           bool useShelters = false,
           int numberOfGames = 1000,
           bool createHtmlReport = false,
           int logGameCount = 0,
           bool debugLogs = false)
        {            
            var resultList = new List<System.Tuple<string, double>>();

            foreach (PlayerAction otherPlayerAction in BuiltInStrategies.StrategyLoader.GetAllPlayerActions())
            {
                if (playerAction == otherPlayerAction)
                    continue;

                double percentDiff = testOutput.ComparePlayers(
                    playerAction,
                    otherPlayerAction,
                    shouldParallel: shouldParallel,
                    useShelters: useShelters,
                    logGameCount: logGameCount,
                    debugLogs: debugLogs,
                    numberOfGames: numberOfGames,
                    useColonyAndPlatinum: true,
                    createHtmlReport: createHtmlReport);

                resultList.Add(new System.Tuple<string, double>(otherPlayerAction.PlayerName, -percentDiff));
            }

            bool firstNegative = true;
            foreach (var result in resultList.OrderByDescending(t => t.Item2))
            {
                if (result.Item2 < 0 && firstNegative)
                {
                    firstNegative = false;
                    System.Console.WriteLine("=====>");
                }
                System.Console.WriteLine("{0:F1}% difference for {1}", result.Item2, result.Item1);
            }            
        }   
    }
}
