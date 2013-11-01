using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CardTypes = Dominion.CardTypes;
using Dominion;
using Dominion.Strategy;

namespace Program
{
    class Program    
    {        
        static void Main()
        {
            System.Console.WriteLine("Loading strategies ...");
            if (!strategyLoader.Load())
                return;
            System.Console.WriteLine("Done");

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            
            //ComparePlayers("BigMoney", "BigMoneyDoubleJack", useColonyAndPlatinum: false, createHtmlReport: true, numberOfGames: 1000, shouldParallel: false);
            //CompareStrategyVsAllKnownStrategies("BigMoney", numberOfGames: 1000, createHtmlReport: true, debugLogs: true, logGameCount:0);
            //TestAllCardsWithBigMoney();    
            //FindOptimalPlayForEachCardWithBigMoney();

            stopwatch.Stop();

            WaitForAllBackgroundTasks();            

            if (StrategyComparison.totalGameCount > 0)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("Elapsed Time: {0}s", (double)stopwatch.ElapsedMilliseconds / 1000);
                System.Console.WriteLine("Total Games Player: {0}", StrategyComparison.totalGameCount);
                System.Console.WriteLine("Elapsed Time per game: {0}us", stopwatch.ElapsedMilliseconds * 1000 / StrategyComparison.totalGameCount);
                System.Console.WriteLine("Elapsed Time per Players Turn: {0}ns", (int)((double)stopwatch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency * 1000 * 1000 * 1000 / GameState.turnTotalCount));
            }

            System.Console.WriteLine("Running Web Service ...");
            new WebService().Run();
        }

        public static IEnumerable<PlayerAction> AllBuiltInStrategies()
        {
            
            foreach (PlayerAction player in strategyLoader.AllStrategies())
                yield return player;
            
            foreach (PlayerAction player in StrategyLoader.GetAllPlayerActions(System.Reflection.Assembly.GetExecutingAssembly()))
                yield return player;    
        }

        static void CompareStrategyVsAllKnownStrategies(
            object playerActionOrString, 
            bool shouldParallel = true, 
            bool useShelters = false, 
            int numberOfGames = 1000, 
            bool createHtmlReport = false,
            int logGameCount = 0,
            bool debugLogs = false)
        {
            PlayerAction playerAction = strategyLoader.GetPlayerAction(playerActionOrString);
            var resultList = new List<System.Tuple<string, double>>();

            foreach (PlayerAction otherPlayerAction in AllBuiltInStrategies())
            {
                if (playerAction == otherPlayerAction)
                    continue;

                double percentDiff = ComparePlayers(
                    playerAction, 
                    otherPlayerAction, 
                    shouldParallel: shouldParallel, 
                    useShelters: useShelters, 
                    logGameCount: logGameCount, 
                    debugLogs: debugLogs,
                    numberOfGames: numberOfGames, 
                    useColonyAndPlatinum: true, 
                    createHtmlReport: createHtmlReport);

                resultList.Add( new System.Tuple<string,double>(otherPlayerAction.PlayerName, -percentDiff));
            }

            bool firstNegative = true;
            foreach(var result in resultList.OrderByDescending(t => t.Item2))
            {
                if (result.Item2 < 0 && firstNegative)
                {
                    firstNegative = false;
                    System.Console.WriteLine("=====>");
                }
                System.Console.WriteLine("{0:F1}% difference for {1}", result.Item2, result.Item1);
            }
        }

        static PlayerAction[] AllBigMoneyWithCard()
        {
            var result = new List<PlayerAction>();

            foreach (var member in typeof(Cards).GetMembers(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
            {
                if (member.MemberType == System.Reflection.MemberTypes.Field)                
                {
                    Card card = (Card)(typeof(Cards).GetField(member.Name).GetValue(null));
                    if (!GameConfigBuilder.IsKingdomCard(card))
                    {
                        continue;
                    }

                    if (notImplementedCards.Contains(card))
                        continue;

                    var playerAction = Strategies.BigMoneyWithCard.Player(card);
                    result.Add(playerAction);
                }
            }

            return result.ToArray();
        }


        static void TestAllCardsWithBigMoney()
        {
            var bigMoneyPlayer = "BigMoney";
            foreach (PlayerAction playerAction in AllBigMoneyWithCard())
            {
                ComparePlayers(playerAction, bigMoneyPlayer, numberOfGames: 1000, shouldParallel: true, createHtmlReport: false, logGameCount: 0);
            }                    
        }

        static void FindOptimalPlayForEachCardWithBigMoney()
        {
            var bigMoneyPlayer = "BigMoney";
            foreach (var member in typeof(Cards).GetMembers(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public))
            {
                if (member.MemberType == System.Reflection.MemberTypes.Field)
                {
                    Card card = (Card)(typeof(Cards).GetField(member.Name).GetValue(null));
                    if (!GameConfigBuilder.IsKingdomCard(card))
                    {
                        continue;
                    }

                    if (notImplementedCards.Contains(card))
                        continue;

                    var playerAction = StrategyOptimizer.FindBestBigMoneyWithCardVsStrategy("BigMoney", card); // Strategies.BigMoneyWithCard.Player(card, "BigMoney<" + card.name + ">");

                    ComparePlayers(playerAction, bigMoneyPlayer, numberOfGames: 1000, shouldParallel: true, createHtmlReport: true, logGameCount: 0);
                }
            }
        }

        static Card[] notImplementedCards = new Card[]
        {
            // implemented cards that require default behaviors            
            Cards.Doctor,
            Cards.Embargo,
            Cards.Explorer,
            Cards.Governor,
            Cards.Graverobber,
            Cards.Haven,
            Cards.Herald,
            Cards.Herbalist,
            Cards.Inn,
            Cards.Island,            
            Cards.Mandarin,
            Cards.Masquerade,
            Cards.Masterpiece,
            Cards.Minion,
            Cards.Mint,            
            Cards.NativeVillage,
            Cards.Navigator,
            Cards.NomadCamp,
            Cards.Oracle,
            Cards.Pawn,
            Cards.PearlDiver,
            Cards.PirateShip,
            Cards.Scavenger,
            Cards.Scout,
            Cards.SpiceMerchant,
            Cards.Squire,            
            Cards.Steward,
            Cards.StoneMason,
            Cards.Torturer,
            Cards.Tournament,            
            Cards.Vault,            

            // unimplemented cards.            
            Cards.Knights,
            Cards.Stash,      
            Cards.BandOfMisfits,
            Cards.BlackMarket,
            Cards.Possession
        };

        static IndentedTextWriter GetGameLogWriterForIteration(PlayerAction player1, PlayerAction player2, int gameCount)
        {
            return new IndentedTextWriter(GetOuputFilename(player1.PlayerName + " VS " + player2.PlayerName + ".gamelog" + (gameCount == 0 ? "" : "." + gameCount.ToString()) + ".txt"));            
        }

        static IndentedTextWriter GetDebugLogWriterForIteration(PlayerAction player1, PlayerAction player2, int gameCount)
        {
            return new IndentedTextWriter(GetOuputFilename(player1.PlayerName + " VS " + player2.PlayerName + ".DebugLog" + (gameCount == 0 ? "" : "." + gameCount.ToString()) + ".txt"));
        }
        
        static string GetOuputFilename(string filename)
        {
            return "..\\..\\Results\\" + filename;
        }                

        public static StrategyLoader strategyLoader = new StrategyLoader();        

        public static double ComparePlayers(
            object player1OrString,
            object player2OrString,
            bool useShelters = false,
            bool useColonyAndPlatinum = false,
            StartingCardSplit split = StartingCardSplit.Random,
            bool firstPlayerAdvantage = false,
            IEnumerable<CardCountPair>[] startingDeckPerPlayer = null,
            bool shouldParallel = true,
            bool showVerboseScore = true,
            bool showCompactScore = false,
            bool showDistribution = false,            
            bool createHtmlReport = true,            
            int numberOfGames = 1000,
            int logGameCount = 10,
            bool debugLogs = false,
            CreateGameLog createGameLog = null)
        {            
            PlayerAction player1 = strategyLoader.GetPlayerAction(player1OrString);
            PlayerAction player2 = strategyLoader.GetPlayerAction(player2OrString);

            GameConfigBuilder builder = new GameConfigBuilder();
            PlayerAction.SetKingdomCards(builder, player1, player2);

            builder.useColonyAndPlatinum = useColonyAndPlatinum;
            builder.useShelters = useShelters;
            builder.CardSplit = split;

            if (startingDeckPerPlayer != null)
                builder.SetStartingDeckPerPlayer(startingDeckPerPlayer);

            var gameConfig = builder.ToGameConfig();

            return ComparePlayers(
                player1,
                player2,
                gameConfig,
                firstPlayerAdvantage: firstPlayerAdvantage,
                shouldParallel: shouldParallel,
                showVerboseScore: showVerboseScore,
                showCompactScore: showCompactScore,
                showDistribution: showDistribution,                
                createHtmlReport: createHtmlReport,
                logGameCount: logGameCount,
                debugLogs: debugLogs,
                numberOfGames: numberOfGames,                
                createGameLog: createGameLog);
        }

        public static double ComparePlayers(
            object player1OrString, 
            object player2OrString, 
            GameConfig gameConfig,
            bool firstPlayerAdvantage = false, 
            bool shouldParallel = true,
            bool showVerboseScore = true,
            bool showCompactScore = false, 
            bool showDistribution = false,                        
            bool createHtmlReport = true,            
            int numberOfGames = 1000, 
            int logGameCount = 100,            
            bool debugLogs = false,
            CreateGameLog createGameLog = null)
        {
            PlayerAction player1 = strategyLoader.GetPlayerAction(player1OrString);
            PlayerAction player2 = strategyLoader.GetPlayerAction(player2OrString);

            var strategyComparison = new StrategyComparison(player1, player2, gameConfig, firstPlayerAdvantage, numberOfGames);
            var results = strategyComparison.ComparePlayers(
                gameIndex => gameIndex < logGameCount ? GetGameLogWriterForIteration(player1, player2, gameIndex) : null,
                gameIndex => debugLogs && gameIndex < logGameCount ? GetDebugLogWriterForIteration(player1, player2, gameIndex) : null,
                shouldParallel: shouldParallel,
                gatherStats: createHtmlReport,
                createGameLog: createGameLog);               

            if (showVerboseScore)
            {
                results.WriteVerboseScore(System.Console.Out);                
            }

            if (showCompactScore)
            {
                results.WriteCompactScore(System.Console.Out);                
            }

            if (showDistribution)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("Player 1 Score Delta distribution");
                System.Console.WriteLine("=================================");
                results.pointSpreadHistogramData.WriteBuckets(System.Console.Out);
            }            

            if (createHtmlReport)
            {
                System.Threading.Interlocked.Increment(ref outstandingTasks);
                // write out HTML report summary
                var thread = new System.Threading.Thread( delegate()
                {
                    var generator = new HtmlReportGenerator(results);

                    generator.CreateHtmlReport(GetOuputFilename(player1.PlayerName + " VS " + player2.PlayerName + ".html"));
                    System.Threading.Interlocked.Decrement(ref outstandingTasks);
                });
                thread.Start();
            }

            
            return results.WinDifference;
        }

        private static int outstandingTasks = 0;

        private static void WaitForAllBackgroundTasks()
        {
            while (outstandingTasks > 0)
            {
                System.Threading.Thread.Sleep(1);
            }
        }                   
    }            
}
