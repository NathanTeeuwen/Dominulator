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

            ComparePlayers(StrategyOptimizer.FindBestBigMoneyWithCardVsStrategy(Strategies.BigMoneyWithCard.Player(Cards.Witch), Cards.Bishop), Strategies.BigMoneyWithCard.Player(Cards.Witch), useColonyAndPlatinum: false, createHtmlReport: true, split:StartingCardSplit.Split43);                    
            //CompareStrategyVsAllKnownStrategies(Strategies.BigMoney.Player(), numberOfGames:1000, createHtmlReport:true);
            //TestAllCardsWithBigMoney();    
            //FindOptimalPlayForEachCardWithBigMoney();
            
            stopwatch.Stop();

            if (totalGameCount > 0)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("Elapsed Time per game: {0}us", stopwatch.ElapsedMilliseconds * 1000 / totalGameCount);
                System.Console.WriteLine("Elapsed Time per Players Turn: {0}ns", (int)((double)stopwatch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency * 1000 * 1000 * 1000 / GameState.turnTotalCount));
            }
        }

        static void CompareStrategyVsAllKnownStrategies(PlayerAction playerAction, bool shouldParallel = true, bool useShelters = false, int numberOfGames = 1000, bool createHtmlReport = false)
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

                double percentDiff = ComparePlayers(playerAction, otherPlayerAction, shouldParallel: shouldParallel, useShelters: useShelters, logGameCount: 0, numberOfGames: numberOfGames, useColonyAndPlatinum: true, createHtmlReport: createHtmlReport);

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
        
        static IGameLog GetGameLogForIteration(int gameCount)
        {
            return new HumanReadableGameLog(GetOuputFilename("GameLog" + (gameCount == 0 ? "" : gameCount.ToString()) + ".txt"));
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
            int logGameCount = 100,
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
            bool createHtmlReport = true,
            int numberOfGames = 1000, 
            int logGameCount = 100,            
            CreateGameLog createGameLog = null)
        {
            PlayerAction[] playerActions = new PlayerAction[] { player1, player2 };
            int[] originalPositions = new int[] { 0, 1 };
            int[] swappedPlayerPositions = new int[] { 1, 0 };
            int[] winnerCount = new int[2];
            int tieCount = 0;

            var statGatherer = createHtmlReport ? new StatsPerTurnGameLog(2, gameConfig.cardGameSubset) : null;            

            var pointSpreadHistogramData = new HistogramData();
            var gameEndOnTurnHistogramData = new HistogramData();
            int maxTurnNumber = -1;

            Action<int> loopBody = delegate(int gameCount)
            {
                System.Threading.Interlocked.Increment(ref totalGameCount);
                using (IGameLog gameLog = createGameLog != null ? createGameLog() :
                                          gameCount < logGameCount ? GetGameLogForIteration(gameCount) :
                                          new EmptyGameLog())
                {
                    IGameLog gameLogMultiplexer = createHtmlReport ? new GameLogMultiplexer(statGatherer, gameLog) : gameLog;

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
                // write out HTML report summary
                //Task.Factory.StartNew(delegate()
                {
                    CreateHtmlReport(
                        player1,
                        player2,
                        gameConfig,
                        firstPlayerAdvantage,
                        numberOfGames,
                        playerActions,
                        winnerCount,
                        tieCount,
                        statGatherer,
                        pointSpreadHistogramData,
                        gameEndOnTurnHistogramData);
                }
                //);
            }

            double diff = PlayerWinPercent(0, winnerCount, numberOfGames) - PlayerWinPercent(1, winnerCount, numberOfGames);
            return diff;
        }

        private static void CreateHtmlReport(
            PlayerAction player1, 
            PlayerAction player2, 
            GameConfig gameConfig, 
            bool firstPlayerAdvantage, 
            int numberOfGames, 
            PlayerAction[] playerActions, 
            int[] winnerCount, 
            int tieCount, 
            StatsPerTurnGameLog statGatherer, 
            HistogramData pointSpreadHistogramData, 
            HistogramData gameEndOnTurnHistogramData)
        {
            int maxTurn = gameEndOnTurnHistogramData.GetXAxisValueCoveringUpTo(97);

            using (var textWriter = new IndentedTextWriter(GetOuputFilename(player1.PlayerName + " VS " + player2.PlayerName + ".html")))
            {
                var htmlWriter = new HtmlRenderer(textWriter);
                htmlWriter.Begin();
                htmlWriter.Header1(player1.PlayerName + " VS " + player2.PlayerName);
                htmlWriter.WriteLine("Number of Games: " + numberOfGames);
                htmlWriter.WriteLine(firstPlayerAdvantage ? player1.PlayerName + " always started first" : "Players took turns going first");

                var pieLabels = new List<string>();
                var pieData = new List<float>();

                for (int index = 0; index < winnerCount.Length; ++index)
                {
                    pieLabels.Add(playerActions[index].name);
                    pieData.Add((float)PlayerWinPercent(index, winnerCount, numberOfGames));
                }
                if (tieCount > 0)
                {
                    pieLabels.Add("Tie");
                    pieData.Add((float)TiePercent(tieCount, numberOfGames));
                }

                htmlWriter.InsertExpander("Who Won?", delegate()
                {
                    InsertPieChart(htmlWriter, "Game Breakdown", "Player", "Percent", pieLabels.ToArray(), pieData.ToArray(), colllapsebyDefault: false);
                    InsertHistogram(htmlWriter, "Point Spread:  " + player1.PlayerName + " score <= 0 >= " + player2.PlayerName + " score", "Percentage", pointSpreadHistogramData, int.MaxValue, content: delegate()
                    {
                        htmlWriter.WriteLine("To the left of 0 are games won by " + player1.PlayerName + ".  To the right are games won by " + player2.PlayerName + ".  The xaxis (absolute value) indicates how many points the game was won by.  The area under the curve indicates the win rate for the corresponding player.");
                    });
                    InsertLineGraph(htmlWriter, "Probability player is ahead in points at end of round ", player1, player2, statGatherer.oddsOfBeingAheadOnRoundEnd, maxTurn);
                    InsertLineGraph(htmlWriter, "Victory Point Total Per Turn", player1, player2, statGatherer.victoryPointTotal, maxTurn);                    
                }, collapseByDefault: false);
                htmlWriter.InsertExpander("When does the game end?", delegate()
                {
                    InsertHistogram(htmlWriter, "Probablity of Game ending on Turn", "Percentage", gameEndOnTurnHistogramData, maxTurn, colllapsebyDefault: false);
                    InsertHistogramIntegrated(htmlWriter, "Probablity of Game being over by turn", "Percentage", gameEndOnTurnHistogramData, maxTurn);
                });
                htmlWriter.InsertExpander("Deck Strength", delegate()
                {
                    InsertCardData(htmlWriter, statGatherer.endOfGameCardCount, gameConfig.cardGameSubset, player1, player2);
                    InsertLineGraph(htmlWriter, "Coin To Spend Per Turn", player1, player2, statGatherer.coinToSpend, maxTurn, content: delegate()
                    {
                        for (int i = 4; i < statGatherer.oddsOfHittingAtLeastACoinAmount.Length; ++i)
                        {
                            InsertLineGraph(htmlWriter, "Odds of Hitting at Least " + i + " coin", player1, player2, statGatherer.oddsOfHittingAtLeastACoinAmount[i], maxTurn);
                        }
                    });
                    InsertLineGraph(htmlWriter, "Number of cards Gained Per Turn", player1, player2, statGatherer.cardsGained, maxTurn);                    
                    htmlWriter.InsertExpander(player1.PlayerName, delegate()
                    {
                        InsertCardData(htmlWriter, "Total Count Of Card", gameConfig.cardGameSubset, statGatherer.cardsTotalCount, 0, maxTurn);
                        InsertCardData(htmlWriter, "Gain Of Card", gameConfig.cardGameSubset, statGatherer.carsGainedOnTurn, 0, maxTurn);
                    });
                    htmlWriter.InsertExpander(player2.PlayerName, delegate()
                    {
                        InsertCardData(htmlWriter, "Total Count Of Card", gameConfig.cardGameSubset, statGatherer.cardsTotalCount, 1, maxTurn);
                        InsertCardData(htmlWriter, "Gain Of Card", gameConfig.cardGameSubset, statGatherer.carsGainedOnTurn, 1, maxTurn);
                    });                                                            
                    InsertLineGraph(htmlWriter, "Shuffles Per Turn", player1, player2, statGatherer.deckShuffleCount, maxTurn);

                    InsertLineGraph(htmlWriter, "Ruins Gained Per Turn", player1, player2, statGatherer.ruinsGained, maxTurn);
                    InsertLineGraph(htmlWriter, "Curses Gained Per Turn", player1, player2, statGatherer.cursesGained, maxTurn);
                    InsertLineGraph(htmlWriter, "Curses Trashed Per Turn", player1, player2, statGatherer.cursesTrashed, maxTurn);
                });

                htmlWriter.InsertExpander("Individual Card Graphs", delegate()
                {
                    foreach (Card card in gameConfig.cardGameSubset.OrderBy(c => c.DefaultCoinCost))
                    {
                        if (statGatherer.cardsTotalCount[card].forwardTotal.HasNonZeroData ||
                            statGatherer.carsGainedOnTurn[card].forwardTotal.HasNonZeroData)
                        {
                            htmlWriter.InsertExpander(card.name, delegate()
                            {
                                InsertLineGraph(htmlWriter, "Card Total At Turn", player1, player2, statGatherer.cardsTotalCount[card], maxTurn, colllapsebyDefault: false);
                                InsertLineGraph(htmlWriter, "Card Gained At Turn", player1, player2, statGatherer.carsGainedOnTurn[card], maxTurn, colllapsebyDefault: true);
                            });
                        }
                    }
                });

                htmlWriter.End();
            }
        }

        private static void InsertCardData(
            HtmlRenderer htmlWriter, 
            string title,
            CardGameSubset gameSubset,
            MapOfCardsForGameSubset<ForwardAndReversePerTurnPlayerCounters> statsPerCard,             
            int playerIndex, 
            int throughTurn)
        {            
            Card[] cards = gameSubset.OrderBy(card => card.DefaultCoinCost).ToArray();
            string[] seriesLabel = new string[cards.Length];
            int[] xAxis = Enumerable.Range(1, throughTurn).ToArray();
            float[][] seriesData = new float [cards.Length][]; 

            for (int i=0; i < cards.Length; i++)
            {
                seriesLabel[i] = cards[i].name;
                seriesData[i] = statsPerCard[cards[i]].forwardTotal.GetAveragePerTurn(playerIndex, throughTurn);
            }

            htmlWriter.InsertExpander(title, delegate()
            {
                htmlWriter.InsertLineGraph(title, "Turn", seriesLabel, xAxis, seriesData);
            }, collapseByDefault:true);
        }

        private static void InsertCardData(HtmlRenderer htmlWriter, MapOfCardsForGameSubset<PlayerCounterSeparatedByGame> map, CardGameSubset gameSubset, PlayerAction player1, PlayerAction player2)
        {
            var cards = gameSubset.OrderBy(c => c.DefaultCoinCost);
            var player1Data = new List<float>();
            var player2Data = new List<float>();

            foreach (Card card in cards)
            {
                player1Data.Add(map[card].GetAverage(playerIndex:0));
                player2Data.Add(map[card].GetAverage(playerIndex:1));
            }

            htmlWriter.InsertExpander("Cards Report", delegate()
            {
                htmlWriter.InsertColumnChart(
                    "Average Count of Cards Owned at End of Game",
                    "Card",
                    new string[] { player1.name, player2.name },
                    cards.Select(c => c.name).ToArray(),
                    new float[][] { player1Data.ToArray(), player2Data.ToArray() });
            }, collapseByDefault:false);
        }

        private static void InsertLineGraph(
            HtmlRenderer htmlWriter,            
            string title,
            PlayerAction player1,
            PlayerAction player2,
            ForwardAndReversePerTurnPlayerCounters forwardAndReverseCounters,
            int throughTurn,
            bool colllapsebyDefault = true,
            HtmlContentInserter content = null)
        {            
            if (forwardAndReverseCounters.forwardTotal.HasNonZeroData)
            {
                htmlWriter.InsertExpander(title, delegate()
                {                    
                    htmlWriter.InsertLineGraph(
                                title,
                                "Turn",
                                player1.PlayerName,
                                player2.PlayerName,
                                Enumerable.Range(1, throughTurn).ToArray(),
                                forwardAndReverseCounters.forwardTotal.GetAveragePerTurn(0, throughTurn),
                                forwardAndReverseCounters.forwardTotal.GetAveragePerTurn(1, throughTurn)
                                );
                    htmlWriter.InsertExpander("Counting back from the end of the Game ...", delegate()
                    {
                        htmlWriter.InsertLineGraph(
                                title,
                                "Turn",
                                player1.PlayerName,
                                player2.PlayerName,
                                Enumerable.Range(0, throughTurn).Select( turn => -turn).ToArray(),
                                forwardAndReverseCounters.reverseTotal.GetAveragePerTurn(0, throughTurn),
                                forwardAndReverseCounters.reverseTotal.GetAveragePerTurn(1, throughTurn)
                                );
                    });

                    if (content != null)
                    {
                        content();
                    }
                },
                collapseByDefault: colllapsebyDefault);
            }
        }

        private static void InsertPieChart(
           HtmlRenderer htmlWriter,
           string title,
           string labelTitle,
           string dataTitle,
           string[] labels,
           float[] data,
           bool colllapsebyDefault = true)
        {
            htmlWriter.InsertExpander(title, delegate()
            {
                htmlWriter.InsertPieChart(
                            title,
                            labelTitle,
                            dataTitle,
                            labels,
                            data);
            },
            collapseByDefault: colllapsebyDefault);
        }


        private static void InsertHistogram(
           HtmlRenderer htmlWriter,
           string title,           
           string xAxisLabel,
           HistogramData data,
           int xAxisMaxValue,
           bool colllapsebyDefault = true,
           HtmlContentInserter content = null)
        {                     
            htmlWriter.InsertExpander(title, delegate()
            {
                htmlWriter.InsertLineGraph(
                            title,
                            "Turn",
                            xAxisLabel,
                            data.GetXAxis(xAxisMaxValue),
                            data.GetYAxis(xAxisMaxValue)                        
                            );
                if (content != null)
                    content();
            },
            collapseByDefault: colllapsebyDefault);
        }

        private static void InsertHistogramIntegrated(
           HtmlRenderer htmlWriter,
           string title,
           string xAxisLabel,
           HistogramData data,
           int xAxisMaxValue)
        {
            htmlWriter.InsertExpander(title, delegate()
            {
                htmlWriter.InsertLineGraph(
                            title,
                            "Turn",
                            xAxisLabel,
                            data.GetXAxis(xAxisMaxValue),
                            data.GetYAxisIntegrated(xAxisMaxValue)
                            );
            },
            collapseByDefault: true);
        }

        static double TiePercent(int tieCount, int numberOfGames)
        {
            return tieCount / (double)numberOfGames * 100;
        }

        static double PlayerWinPercent(int player, int[] winnerCount, int numberOfGames)
        {
            return winnerCount[player] / (double)numberOfGames * 100;
        }              
    }            
}
