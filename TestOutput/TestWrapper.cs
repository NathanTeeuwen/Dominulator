using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CardTypes = Dominion.CardTypes;
using Dominion;
using Dominion.Strategy;
using Dominion.Data;

namespace Dominion
{
    public class TestOutput
        : IDisposable
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        HtmlRenderer.DeferredHtmlGenerator deferredHtmlGenerator = new HtmlRenderer.DeferredHtmlGenerator();

        public TestOutput()
        {
            this.stopwatch.Start();
        }

        public void Dispose()
        {
            this.stopwatch.Stop();
            this.deferredHtmlGenerator.Dispose();

            if (StrategyComparison.totalGameCount > 0)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("Elapsed Time: {0}s", (double)this.stopwatch.ElapsedMilliseconds / 1000);
                System.Console.WriteLine("Total Games Played: {0}", StrategyComparison.totalGameCount);
                System.Console.WriteLine("Elapsed Time per game: {0}us", this.stopwatch.ElapsedMilliseconds * 1000 / StrategyComparison.totalGameCount);
                System.Console.WriteLine("Elapsed Time per Players Turn: {0}ns", (int)((double)this.stopwatch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency * 1000 * 1000 * 1000 / GameState.turnTotalCount));
            }

            PauseForeverUnderDebugger();
        }

        public double ComparePlayers(
            PlayerAction[] playerActions,
            GameConfig gameConfig,
            bool rotateWhoStartsFirst = true,
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
            var strategyComparison = new StrategyComparison(playerActions, gameConfig, rotateWhoStartsFirst, numberOfGames);
            var results = strategyComparison.ComparePlayers(
                gameIndex => gameIndex < logGameCount ? GetGameLogWriterForIteration(playerActions, gameIndex) : null,
                gameIndex => debugLogs && gameIndex < logGameCount ? GetDebugLogWriterForIteration(playerActions, gameIndex) : null,
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
                results.ShowDistribution(System.Console.Out);
            }

            if (createHtmlReport)
            {
                deferredHtmlGenerator.AddResults(results, GetOutputFilename);
            }


            return results.WinDifference;
        }

        public double ComparePlayers(
            PlayerAction player1,
            PlayerAction player2,
            GameConfig gameConfig,
            bool rotateWhoStartsFirst = true,
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
            return ComparePlayers(new PlayerAction[] { player1, player2 },
                gameConfig,
                rotateWhoStartsFirst,
                shouldParallel,
                showVerboseScore,
                showCompactScore,
                showDistribution,
                createHtmlReport,
                numberOfGames,
                logGameCount,
                debugLogs,
                createGameLog);
        }

        public double ComparePlayers(
            PlayerAction player1,
            PlayerAction player2,
            bool useShelters = false,
            bool useColonyAndPlatinum = false,
            StartingCardSplit split = StartingCardSplit.Random,
            bool rotateWhoStartsFirst = true,
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
                rotateWhoStartsFirst: rotateWhoStartsFirst,
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

        public double ComparePlayers(
          string player1,
          string player2,
          bool useShelters = false,
          bool useColonyAndPlatinum = false,
          StartingCardSplit split = StartingCardSplit.Random,
          bool rotateWhoStartsFirst = false,
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
            return ComparePlayers(
                BuiltInStrategies.StrategyLoader.PlayerFromString(player1),
                BuiltInStrategies.StrategyLoader.PlayerFromString(player2),
                useShelters,
                useColonyAndPlatinum,
                split,
                rotateWhoStartsFirst,
                startingDeckPerPlayer,
                shouldParallel,
                showVerboseScore,
                showCompactScore,
                showDistribution,
                createHtmlReport,
                numberOfGames,
                logGameCount,
                debugLogs,
                createGameLog);
        }     

    
        private void PauseForeverUnderDebugger() 
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
            }
        }

        private static string GetFileNameForType(PlayerAction[] playerActions, int gameCount, string logType)
        {
            var stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append(playerActions[0].PlayerName);
            for (int i = 1; i < playerActions.Length; ++i)
            {
                stringBuilder.Append(" VS ");
                stringBuilder.Append(playerActions[i].PlayerName);
            }
            stringBuilder.Append(".");
            stringBuilder.Append(logType);
            stringBuilder.Append(gameCount == 0 ? "" : "." + gameCount.ToString());
            stringBuilder.Append(".txt");

            string fileName = GetOutputFilename(stringBuilder.ToString());

            return fileName;
        }

        public static IndentedTextWriter GetGameLogWriterForIteration(PlayerAction[] playerActions, int gameCount)
        {
            string fileName = GetFileNameForType(playerActions, gameCount, "GameLog");

            if (fileName == null)
                return null;
            return new IndentedTextWriter(new System.IO.StreamWriter(fileName));
        }

        public static IndentedTextWriter GetDebugLogWriterForIteration(PlayerAction[] playerActions, int gameCount)
        {
            string fileName = GetFileNameForType(playerActions, gameCount, "DebugLog");

            if (fileName == null)
                return null;
            return new IndentedTextWriter(new System.IO.StreamWriter(fileName));
        }

        static bool checkedPath = false;
        public static string GetOutputFilename(string filename)
        {
            string resultsDirectory = "..\\..\\..\\Results";
            if (!System.IO.Directory.Exists(resultsDirectory))
            {
                if (!checkedPath)
                    System.Console.WriteLine("Warning:  The directory " + System.IO.Path.GetFullPath(resultsDirectory) + " does not exist.  Logs can not be written");
                checkedPath = true;
                return null;
            }                
            return resultsDirectory + "\\" + filename;
        }
    }
}
