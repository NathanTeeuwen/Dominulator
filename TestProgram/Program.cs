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

            ComparePlayers(Strategies.HermitMarketSquare.Player(1), Strategies.BigMoney.Player(2));
            CompareStrategyVsAllKnownStrategies(Strategies.BigMoneyCultist.Player(1));
            
            stopwatch.Stop();
            System.Console.WriteLine("");
            System.Console.WriteLine("Elapsed Time per 1000 games: {0}ms", stopwatch.ElapsedMilliseconds * 1000 / totalGameCount);
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

                double percentDiff = ComparePlayers(playerAction, otherPlayerAction, shouldParallel:shouldParallel, useShelters: useShelters);

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
            bool firstPlayerAdvantage = false, 
            bool showVerboseScore = true,
            bool showCompactScore = false, 
            bool showDistribution = false,
            bool shouldParallel = true,
            bool showPlayer2Wins = false,
            int numberOfGames = 1000, 
            int logGameCount = 100,
            CreateGameLog createGameLog = null,
            IEnumerable<CardCountPair>[] startingDeckPerPlayer = null)
        {            
            PlayerAction[] players = new PlayerAction[] { player1, player2 };
            int[] winnerCount = new int[2];
            int tieCount = 0;

            var swappedStartingDeckPerPlayer = SwapTwoElementArray(startingDeckPerPlayer);

            var countbyBucket = new CountByBucket();

            Card[] supplyPiles = PlayerAction.GetKingdomCards(player1, player2);

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

                    IEnumerable<CardCountPair>[] startingDecksToUse = 
                        startingDeckPerPlayer == null ? null :
                        swappedOrder ? swappedStartingDeckPerPlayer :
                        startingDeckPerPlayer;

                    Random random = new Random(gameCount);

                    var gameConfig = new GameConfig(
                        useShelters,                        
                        useColonyAndPlatinum: false,
                        supplyPiles: supplyPiles);

                    GameState gameState = new GameState(
                        gameLog,
                        new PlayerAction[] { startPlayer, otherPlayer },
                        gameConfig,
                        random,
                        startingDeckPerPlayer: startingDecksToUse);

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
