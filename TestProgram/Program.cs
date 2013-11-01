using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CardTypes = Dominion.CardTypes;
using Dominion;

namespace Program
{
    class Program    
    {        
        static void Main()
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
           
            ComparePlayers(Strategies.BigMoneyWithCard.Player(Cards.Treasury, cardCount:4), Strategies.BigMoneyWithCard.Player(Cards.Scheme), useColonyAndPlatinum: false, createHtmlReport: true, numberOfGames:1000, shouldParallel:false);
            //CompareStrategyVsAllKnownStrategies(Strategies.BigMoney.Player(), numberOfGames: 1000, createHtmlReport: true, debugLogs: true, logGameCount:10);
            //TestAllCardsWithBigMoney();    
            //FindOptimalPlayForEachCardWithBigMoney();                        
            new WebService().Run();

            stopwatch.Stop();

            WaitForAllBackgroundTasks();

            if (totalGameCount > 0)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("Elapsed Time: {0}s", (double)stopwatch.ElapsedMilliseconds / 1000);
                System.Console.WriteLine("Total Games Player: {0}", totalGameCount);
                System.Console.WriteLine("Elapsed Time per game: {0}us", stopwatch.ElapsedMilliseconds * 1000 / totalGameCount);
                System.Console.WriteLine("Elapsed Time per Players Turn: {0}ns", (int)((double)stopwatch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency * 1000 * 1000 * 1000 / GameState.turnTotalCount));
            }
        }

        public static PlayerAction[] AllBuiltInStrategies()
        {
            var result = new List<PlayerAction>();

            var assembly = System.Reflection.Assembly.GetCallingAssembly();
            var type = assembly.GetType("Program.Strategies");
            foreach (Type innerType in type.GetNestedTypes())
            {
                if (!innerType.IsClass)
                    continue;

                System.Reflection.MethodInfo playerMethodInfo = innerType.GetMethod("Player", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (playerMethodInfo == null)
                    continue;

                if (playerMethodInfo.ContainsGenericParameters)
                    continue;

                if (playerMethodInfo.GetParameters().Count() > 0)
                {
                    continue;
                }

                PlayerAction playerAction = playerMethodInfo.Invoke(null, new object[0]) as PlayerAction;
                if (playerAction == null)
                    continue;

                result.Add(playerAction);
            }

            return result.ToArray();
        }

        static void CompareStrategyVsAllKnownStrategies(
            PlayerAction playerAction, 
            bool shouldParallel = true, 
            bool useShelters = false, 
            int numberOfGames = 1000, 
            bool createHtmlReport = false,
            int logGameCount = 0,
            bool debugLogs = false)
        {
            var resultList = new List<System.Tuple<string, double>>();

            foreach (PlayerAction otherPlayerAction in AllBuiltInStrategies())
            {                
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

                resultList.Add( new System.Tuple<string,double>(otherPlayerAction.PlayerName, percentDiff));
            }
            
            foreach(var result in resultList.OrderBy(t => t.Item2))
            {
                if (result.Item1 == playerAction.name)
                    System.Console.Write("=====>");
                System.Console.WriteLine("{0:F1}% difference for {1}", -result.Item2, result.Item1);
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
            var bigMoneyPlayer = Strategies.BigMoney.Player();
            foreach (PlayerAction playerAction in AllBigMoneyWithCard())
            {
                ComparePlayers(playerAction, bigMoneyPlayer, numberOfGames: 1000, shouldParallel: true, createHtmlReport: false, logGameCount: 0);
            }                    
        }

        static void FindOptimalPlayForEachCardWithBigMoney()
        {
            var bigMoneyPlayer = Strategies.BigMoney.Player();
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

                    var playerAction = StrategyOptimizer.FindBestBigMoneyWithCardVsStrategy(Strategies.BigMoney.Player(), card); // Strategies.BigMoneyWithCard.Player(card, "BigMoney<" + card.name + ">");

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

        static int totalGameCount = 0;

        public static double ComparePlayers(
            PlayerAction player1,
            PlayerAction player2,
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
            PlayerAction player1, 
            PlayerAction player2, 
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
