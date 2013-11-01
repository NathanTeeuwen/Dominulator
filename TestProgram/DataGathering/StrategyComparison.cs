using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominion;
using Dominion.Strategy;

namespace Program
{    

    class StrategyComparison
    {
        public static int totalGameCount;

        public PlayerAction[] playerActions;        
        public GameConfig gameConfig;
        public bool firstPlayerAdvantage;
        public int numberOfGames;

        public StrategyComparison(
            PlayerAction player1,
            PlayerAction player2,
            GameConfig gameConfig,
            bool firstPlayerAdvantage,
            int numberOfGames)
        {
            this.playerActions = new PlayerAction[] { player1, player2 };
            this.gameConfig = gameConfig;
            this.firstPlayerAdvantage = firstPlayerAdvantage;
            this.numberOfGames = numberOfGames;
        }

        public StrategyComparisonResults ComparePlayers(
            GetLogForGame getHumanReadableLogWriter,
            GetLogForGame getDebugLogWriter,
            bool shouldParallel = true,                        
            bool gatherStats = true,            
            CreateGameLog createGameLog = null)
        {
            PlayerAction player1 = playerActions[0];
            PlayerAction player2 = playerActions[1];
            int[] originalPositions = new int[] { 0, 1 };
            int[] swappedPlayerPositions = new int[] { 1, 0 };

            var result = new StrategyComparisonResults(this, gatherStats);            

            Action<int> loopBody = delegate(int gameCount)
            {
                System.Threading.Interlocked.Increment(ref totalGameCount);
                using (IndentedTextWriter textWriter = getHumanReadableLogWriter(gameCount))
                using (IndentedTextWriter debugWriter = getDebugLogWriter(gameCount))
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
            int[] originalPositions = new int[] { 0, 1 };
            int[] swappedPlayerPositions = new int[] { 1, 0 };

            bool swappedOrder = !firstPlayerAdvantage && (gameNumber % 2 == 1);
            int[] playedPositions = swappedOrder ? swappedPlayerPositions : originalPositions;

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
    }
}
