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
           
            ComparePlayers(Strategies.LookoutSalvagerLibraryHighwayFestival.Player(), Strategies.BigMoneySingleWitch.Player(), useColonyAndPlatinum: false, createHtmlReport: true, numberOfGames:1000);
            CompareStrategyVsAllKnownStrategies(Strategies.MountebankMonumentHamletVineyard.Player(), numberOfGames: 1000, createHtmlReport: true, debugLogs: true);
            //TestAllCardsWithBigMoney();    
            //FindOptimalPlayForEachCardWithBigMoney();                        
            //new WebService().Run();

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

                if (playerMethodInfo.GetParameters().Count() > 0) {
                    continue;
                }

                PlayerAction otherPlayerAction = playerMethodInfo.Invoke(null, new object[0]) as PlayerAction;
                if (otherPlayerAction == null)
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

                resultList.Add( new System.Tuple<string,double>(otherPlayerAction.PlayerName, percentDiff));
            }            

            foreach(var result in resultList.OrderBy(t => t.Item2))
            {
                if (result.Item1 == playerAction.name)
                    System.Console.Write("=====>");
                System.Console.WriteLine("{0:F1}% difference for {1}", -result.Item2, result.Item1);
            }
        }

        static void TestAllCardsWithBigMoney()
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

                    var playerAction = Strategies.BigMoneyWithCard.Player(card);

                    ComparePlayers(playerAction, bigMoneyPlayer, numberOfGames:1000, shouldParallel:true, createHtmlReport:false, logGameCount:0);
                }
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
            Cards.Scheme,
            Cards.Scout,
            Cards.SpiceMerchant,
            Cards.Squire,            
            Cards.Steward,
            Cards.StoneMason,
            Cards.Torturer,
            Cards.Tournament,            
            Cards.Vault,            

            // unimplemented cards.
            Cards.WalledVillage,
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

        public delegate IGameLog CreateGameLog();

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
            bool showPlayer2Wins = false,
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
                showPlayer2Wins: showPlayer2Wins,
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
            bool showPlayer2Wins = false,
            bool createHtmlReport = true,            
            int numberOfGames = 1000, 
            int logGameCount = 100,            
            bool debugLogs = false,
            CreateGameLog createGameLog = null)
        {
            PlayerAction[] playerActions = new PlayerAction[] { player1, player2 };
            int[] originalPositions = new int[] { 0, 1 };
            int[] swappedPlayerPositions = new int[] { 1, 0 };
            int[] winnerCount = new int[2];
            int tieCount = 0;

            var statGathererGameLog = createHtmlReport ? new StatsPerTurnGameLog(2, gameConfig.cardGameSubset) : null;            

            var pointSpreadHistogramData = new HistogramData();
            var gameEndOnTurnHistogramData = new HistogramData();
            int maxTurnNumber = -1;

            Action<int> loopBody = delegate(int gameCount)
            {
                System.Threading.Interlocked.Increment(ref totalGameCount);
                using (IndentedTextWriter textWriter = gameCount < logGameCount ? GetGameLogWriterForIteration(player1, player2, gameCount) : null)
                using (IndentedTextWriter debugWriter = debugLogs && gameCount < logGameCount ? GetDebugLogWriterForIteration(player1, player2, gameCount) : null)
                {
                    var gameLogs = new List<IGameLog>();
                    if (createHtmlReport)
                    {
                        gameLogs.Add(statGathererGameLog);
                    }
                    if (createGameLog != null)
                    {
                        gameLogs.Add(createGameLog());
                    }
                    if (textWriter != null)
                    {
                        var humanReadableGameLog = new HumanReadableGameLog(textWriter);
                        gameLogs.Add(humanReadableGameLog);
                        var gainSequenceGameLog = new GainSequenceGameLog(textWriter);
                        gameLogs.Add(gainSequenceGameLog);
                    }
                    if (debugWriter != null)
                    {
                        var debugLog = new DebugGameLog(debugWriter);
                        gameLogs.Add(debugLog);
                        var gainSequenceGameLog = new GainSequenceGameLog(debugWriter);
                        gameLogs.Add(gainSequenceGameLog);
                    }

                    var gameLogMultiplexer = new GameLogMultiplexer(gameLogs.ToArray());

                    // swap order every other game
                    bool swappedOrder = !firstPlayerAdvantage && (gameCount % 2 == 1);

                    int[] playedPositions = swappedOrder ? swappedPlayerPositions : originalPositions;                    

                    Random random = new Random(gameCount);
                    using (Game game = new Game(random, gameConfig, gameLogMultiplexer))
                    {

                        GameState gameState = new GameState(
                            playerActions,
                            playedPositions,
                            game);

                        gameState.PlayGameToEnd();
                        PlayerState[] winners = gameState.WinningPlayers;

                        int player1Score = gameState.players.OriginalPlayerOrder[playedPositions[0]].TotalScore();
                        int player2Score = gameState.players.OriginalPlayerOrder[playedPositions[1]].TotalScore();
                        int scoreDifference = player2Score - player1Score;
                        pointSpreadHistogramData.AddOneToBucket(scoreDifference);
                        gameEndOnTurnHistogramData.AddOneToBucket(gameState.players.CurrentPlayer.TurnNumber);                        

                        lock (winnerCount)
                        {
                            maxTurnNumber = Math.Max(gameState.players.CurrentPlayer.TurnNumber, maxTurnNumber);
                            if (winners.Length == 1)
                            {
                                int winningPlayerIndex = winners[0].Actions == player1 ? 0 : 1;
                                winnerCount[winningPlayerIndex]++;

                                if (winningPlayerIndex == 1 && showPlayer2Wins)
                                {
                                    System.Console.WriteLine("Player 2 won game {0}. ", gameCount);
                                }
                            }
                            else
                            {
                                tieCount++;
                            }
                        }
                    }
                }
            };

            if (shouldParallel)
            {
                Parallel.ForEach(Enumerable.Range(0, numberOfGames), loopBody);
            }
            else
            {
                for (int gameCount = 0; gameCount < numberOfGames; ++gameCount)
                    loopBody(gameCount);
            }

            gameEndOnTurnHistogramData.InitializeAllBucketsUpTo(maxTurnNumber);

            if (showVerboseScore)
            {
                for (int index = 0; index < winnerCount.Length; ++index)
                {
                    System.Console.WriteLine("{1}% win for {0}", playerActions[index].name, PlayerWinPercent(index, winnerCount, numberOfGames));
                }
                if (tieCount > 0)
                {
                    System.Console.WriteLine("{0}% there is a tie.", TiePercent(tieCount, numberOfGames));
                }
                System.Console.WriteLine();
            }

            if (showCompactScore)
            {
                System.Console.WriteLine("{0}, {1}, {2}",
                    PlayerWinPercent(0, winnerCount, numberOfGames),
                    PlayerWinPercent(1, winnerCount, numberOfGames),
                    TiePercent(tieCount, numberOfGames));
            }

            if (showDistribution)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("Player 1 Score Delta distribution");
                System.Console.WriteLine("=================================");
                pointSpreadHistogramData.WriteBuckets(System.Console.Out);
            }            

            if (createHtmlReport)
            {
                System.Threading.Interlocked.Increment(ref outstandingTasks);
                // write out HTML report summary
                var thread = new System.Threading.Thread( delegate()
                {
                    var generator = new HtmlReportGenerator(                        
                        gameConfig,
                        firstPlayerAdvantage,
                        numberOfGames,
                        playerActions,                        
                        winnerCount,
                        tieCount,
                        statGathererGameLog,
                        pointSpreadHistogramData,
                        gameEndOnTurnHistogramData);

                    generator.CreateHtmlReport(GetOuputFilename(player1.PlayerName + " VS " + player2.PlayerName + ".html"));
                    System.Threading.Interlocked.Decrement(ref outstandingTasks);
                });
                thread.Start();
            }

            double diff = PlayerWinPercent(0, winnerCount, numberOfGames) - PlayerWinPercent(1, winnerCount, numberOfGames);
            return diff;
        }

        private static int outstandingTasks = 0;

        private static void WaitForAllBackgroundTasks()
        {
            while (outstandingTasks > 0)
            {
                System.Threading.Thread.Sleep(1);
            }
        }
        
        public static double TiePercent(int tieCount, int numberOfGames)
        {
            return tieCount / (double)numberOfGames * 100;
        }

        public static double PlayerWinPercent(int player, int[] winnerCount, int numberOfGames)
        {
            return winnerCount[player] / (double)numberOfGames * 100;
        }
    }            
}
