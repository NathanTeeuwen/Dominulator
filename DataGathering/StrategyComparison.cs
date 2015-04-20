using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominion;
using Dominion.Strategy;

namespace Dominion.Data
{    
    public class StrategyComparison
    {
        public static int totalGameCount;

        public readonly PlayerAction[] playerActions;
        public readonly GameConfig gameConfig;
        public readonly bool rotateWhoStartsFirst;
        public readonly int numberOfGames;

        // state used to populate game orders without recreating every game needed order
        
        static int[][] rotated2PlayerPostions = new int[][] {
            new int[] { 0, 1 },
            new int[] { 1, 0 },
        };

        static int[][] rotated3PlayerPostions = new int[][] {
            new int[] { 0, 1, 2 },
            new int[] { 1, 2, 0 },
            new int[] { 2, 0, 1 },
        };

        static int[][] rotated4PlayerPostions = new int[][] {
            new int[] { 0, 1, 2, 3 },
            new int[] { 1, 2, 3, 0 },
            new int[] { 2, 3, 0, 1 },
            new int[] { 3, 0, 1, 2},
        };

        public int NumberOfPlayers
        {
            get
            {
                return this.playerActions.Length;
            }
        }
        
        public static StrategyComparisonResults Compare(
            PlayerAction player1,
            PlayerAction player2,
            int numberOfGames = 100,
            bool rotateWhoStartsFirst = false,
            bool shouldParalell = false,
            bool gatherStats = true)
        {           
            GameConfigBuilder builder = new GameConfigBuilder();
            PlayerAction.SetKingdomCards(builder, player1, player2);

            var gameConfig = builder.ToGameConfig();
            var strategyComparison = new Dominion.Data.StrategyComparison(player1, player2, gameConfig, rotateWhoStartsFirst, numberOfGames);

            var results = strategyComparison.ComparePlayers(
                gameIndex => null,
                gameIndex => null,
                shouldParallel: shouldParalell,
                gatherStats: gatherStats,
                createGameLog: null);

            return results;
        }

        public StrategyComparison(
            PlayerAction player1,
            PlayerAction player2,
            GameConfig gameConfig,
            bool rotateWhoStartsFirst,
            int numberOfGames)
            : this(new PlayerAction[] { player1, player2}, gameConfig, rotateWhoStartsFirst, numberOfGames)
        {           
        }

        public StrategyComparison(
            PlayerAction player1,
            PlayerAction player2,
            PlayerAction player3,
            GameConfig gameConfig,
            bool rotateWhoStartsFirst,
            int numberOfGames)
            : this(new PlayerAction[] { player1, player2, player3},gameConfig, rotateWhoStartsFirst, numberOfGames )
        {            
        }

        public StrategyComparison(
            PlayerAction[] playerActions,            
            GameConfig gameConfig,
            bool rotateWhoStartsFirst,
            int numberOfGames)
        {
            this.playerActions = playerActions;
            this.gameConfig = gameConfig;
            this.rotateWhoStartsFirst = rotateWhoStartsFirst;
            this.numberOfGames = numberOfGames;
        }

        public StrategyComparisonResults ComparePlayers(
            GetLogForGame getHumanReadableLogWriter = null,
            GetLogForGame getDebugLogWriter = null,
            bool shouldParallel = true,                        
            bool gatherStats = true,            
            CreateGameLog createGameLog = null,
            int randomSeed = 0)
        {
            PlayerAction player1 = playerActions[0];
            PlayerAction player2 = playerActions[1];
            
            var result = new StrategyComparisonResults(this, gatherStats);            

            Action<int> loopBody = delegate(int gameCount)
            {
                System.Threading.Interlocked.Increment(ref totalGameCount);
                using (IndentedTextWriter textWriter = getHumanReadableLogWriter != null ? getHumanReadableLogWriter(gameCount) :  null)
                using (IndentedTextWriter debugWriter = getDebugLogWriter != null ? getDebugLogWriter(gameCount) : null)
                {
                    var gameLogs = new List<IGameLog>();
                    if (gatherStats)
                    {
                        gameLogs.Add(result.statGatherer);
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

                    // swap order every game if needed                    
                    int[] playedPositions = this.GetPlayerOrderForGameNumber(gameCount);

                    Random random = new Random(gameCount + randomSeed);
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

                        lock (result)
                        {
                            result.pointSpreadHistogramData.AddOneToBucket(scoreDifference);
                            result.gameEndOnTurnHistogramData.AddOneToBucket(gameState.players.CurrentPlayer.TurnNumber);
                            result.maxTurnNumber = Math.Max(gameState.players.CurrentPlayer.TurnNumber, result.maxTurnNumber);
                            if (winners.Length == 1)
                            {
                                int winningPlayerIndex = winners[0].Actions == player1 ? 0 : 1;
                                result.winnerCount[winningPlayerIndex]++;                              
                            }
                            else
                            {
                                result.tieCount++;
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

            result.gameEndOnTurnHistogramData.InitializeAllBucketsUpTo(result.maxTurnNumber);

            return result;
        }

        public string GetHumanReadableGameLog(int gameNumber)
        {
            // swap order every game if needed                    
            int[] playedPositions = this.GetPlayerOrderForGameNumber(gameNumber);

            var stringWriter = new System.IO.StringWriter();
            var textWriter = new IndentedTextWriter(stringWriter);
            var readableLog = new HumanReadableGameLog(textWriter);
            var gainSequenceLog = new GainSequenceGameLog(textWriter);
            Random random = new Random(gameNumber);
            using (Game game = new Game(random, gameConfig, new GameLogMultiplexer(readableLog, gainSequenceLog)))
            {
                GameState gameState = new GameState(
                    playerActions,
                    playedPositions,
                    game);
                gameState.PlayGameToEnd();
            }

            return stringWriter.ToString();
        }      

        public int[] GetPlayerOrderForGameNumber(int gameNumber)
        {
            int[][] result =
                this.NumberOfPlayers == 2 ? rotated2PlayerPostions :
                this.NumberOfPlayers == 3 ? rotated3PlayerPostions :
                this.NumberOfPlayers == 4 ? rotated4PlayerPostions :
                null;

            if (!this.rotateWhoStartsFirst)
                return result[0];

            return result[gameNumber % this.NumberOfPlayers];
        }
    }
}
