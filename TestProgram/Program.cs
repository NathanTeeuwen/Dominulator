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

            //Kingdoms.ShouldRemakeOrHorseTradersIntoSoothayer.Run();
            //ComparePlayers(Strategies.BigMoneyCultist.Player(1), Strategies.MountebankGovernorMaurader.Player(2), useShelters:true);            
            //CompareStrategyVsAllKnownStrategies(Strategies.HorseTraderSoothsayerMinionGreatHall.Player(1), useShelters: true);            
            ComparePlayers(Strategies.BigMoneyWithCard<CardTypes.Envoy>.Player(1), Strategies.BigMoney.Player(2));            
            stopwatch.Stop();

            System.Console.WriteLine("");
            System.Console.WriteLine("Elapsed Time per game: {0}us", stopwatch.ElapsedMilliseconds * 1000 / totalGameCount);
            System.Console.WriteLine("Elapsed Time per Players Turn: {0}ns", (int)((double) stopwatch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency * 1000 * 1000 * 1000 / GameState.turnTotalCount));
        }
        
        static void CompareStrategyVsAllKnownStrategies(PlayerAction playerAction, bool shouldParallel = true, bool useShelters = false)
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

                PlayerAction otherPlayerAction = playerMethodInfo.Invoke(null, new object[]{2}) as PlayerAction;
                if (otherPlayerAction == null)
                    continue;

                double percentDiff = ComparePlayers(playerAction, otherPlayerAction, shouldParallel:shouldParallel, useShelters: useShelters, logGameCount:0, numberOfGames:1000);

                resultList.Add( new System.Tuple<string,double>(otherPlayerAction.PlayerName, percentDiff));
            }            

            foreach(var result in resultList.OrderBy(t => t.Item2))
            {
                if (result.Item1 == playerAction.name)
                    System.Console.Write("=====>");
                System.Console.WriteLine("{0:F1}% difference for {1}", -result.Item2, result.Item1);
            }
        }                                                 
        
        static IGameLog GetGameLogForIteration(int gameCount)
        {
            return new HumanReadableGameLog("..\\..\\Results\\GameLog" + (gameCount == 0 ? "" : gameCount.ToString()) + ".txt");
        }

        public delegate IGameLog CreateGameLog();

        static int totalGameCount = 0;

        public static double ComparePlayers(
            PlayerAction player1,
            PlayerAction player2,
            bool useShelters = false,
            bool useColonyAndPlatinum = false,
            bool firstPlayerAdvantage = false,
            IEnumerable<CardCountPair>[] startingDeckPerPlayer = null,
            bool shouldParallel = true,
            bool showVerboseScore = true,
            bool showCompactScore = false,
            bool showDistribution = false,
            bool showPlayer2Wins = false,
            int numberOfGames = 1000,
            int logGameCount = 100,
            CreateGameLog createGameLog = null)
        {            

            GameConfigBuilder builder = new GameConfigBuilder();
            PlayerAction.SetKingdomCards(builder, player1, player2);

            builder.useColonyAndPlatinum = useColonyAndPlatinum;
            builder.useShelters = useShelters;

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
                numberOfGames: numberOfGames,
                logGameCount: logGameCount,
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
            int numberOfGames = 1000, 
            int logGameCount = 100,            
            CreateGameLog createGameLog = null)
        {            
            PlayerAction[] players = new PlayerAction[] { player1, player2 };
            int[] winnerCount = new int[2];
            int tieCount = 0;

            GameConfig swappedPlayersConfig = GameConfigBuilder.CreateFromWithPlayPositionsSwapped(gameConfig);            

            var countbyBucket = new CountByBucket();                        

            Action<int> loopBody = delegate(int gameCount)                    
            {
                System.Threading.Interlocked.Increment(ref totalGameCount);
                using (IGameLog gameLog = createGameLog != null ? createGameLog() :
                                          gameCount < logGameCount ? GetGameLogForIteration(gameCount) : 
                                          new EmptyGameLog())                
                {
                    // swap order every other game
                    bool swappedOrder = !firstPlayerAdvantage && (gameCount % 2 == 1);
                    PlayerAction startPlayer = !swappedOrder ? player1 : player2;
                    PlayerAction otherPlayer = !swappedOrder ? player2 : player1;

                    var gameConfigToUse = swappedOrder ? swappedPlayersConfig : gameConfig;                    

                    Random random = new Random(gameCount);                    

                    GameState gameState = new GameState(
                        gameLog,
                        new PlayerAction[] { startPlayer, otherPlayer },
                        gameConfigToUse,
                        random);

                    gameState.PlayGameToEnd();

                    PlayerState[] winners = gameState.WinningPlayers;

                    int startPlayerScore = gameState.players[0].TotalScore();
                    int otherPlayerScore = gameState.players[1].TotalScore();
                    int scoreDifference = startPlayerScore - otherPlayerScore;
                    if (swappedOrder)
                        scoreDifference = -scoreDifference;

                    lock (winnerCount)
                    {
                        countbyBucket.AddOneToBucket(scoreDifference);
                        if (winners.Length == 1)
                        {
                            int winningPlayerIndex = ((PlayerAction)winners[0].Actions).playerIndex - 1;
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

            if (showVerboseScore) 
            {
                for (int index = 0; index < winnerCount.Length; ++index)
                {
                    System.Console.WriteLine("{1}% win for {0}", players[index].name, PlayerWinPercent(index, winnerCount, numberOfGames));
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
                countbyBucket.WriteBuckets(System.Console.Out);
            }

            double diff = PlayerWinPercent(0, winnerCount, numberOfGames) - PlayerWinPercent(1, winnerCount, numberOfGames);
            return diff;
        }

        static T[] SwapTwoElementArray<T>(T[] array)
        {
            if (array == null)
                return null;

            T[] result = new T[2];
            result[0] = array[1];
            result[1] = array[0];
            return result;
        }

        static double TiePercent(int tieCount, int numberOfGames)
        {
            return tieCount / (double)numberOfGames * 100;
        }

        static double PlayerWinPercent(int player, int[] winnerCount, int numberOfGames)
        {
            return winnerCount[player] / (double)numberOfGames * 100;
        }

        class CountByBucket
        {
            int totalCount = 0;
            Dictionary<int, int> mapBucketToCount = new Dictionary<int, int>();

            public void AddOneToBucket(int bucket)
            {
                this.totalCount++;

                int value = 0;
                this.mapBucketToCount.TryGetValue(bucket, out value);
                value += 1;
                this.mapBucketToCount[bucket] = value;
            }

            public void WriteBuckets(System.IO.TextWriter writer)
            {
                foreach (var pair in this.mapBucketToCount.OrderByDescending(keyValuePair => keyValuePair.Key))
                {
                    writer.WriteLine("{0} points:   {2}% = {1}", pair.Key, pair.Value, (double)pair.Value / this.totalCount * 100);
                }
            }
        }       
    }            
}
