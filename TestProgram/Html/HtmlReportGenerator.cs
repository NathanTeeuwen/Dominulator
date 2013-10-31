using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardTypes = Dominion.CardTypes;
using Dominion;

namespace Program
{
    class HtmlReportGenerator
    {
        private StrategyComparisonResults comparisonResults;        

        public HtmlReportGenerator(StrategyComparisonResults comparisonResults)
        {
            this.comparisonResults = comparisonResults;
        }

        public string CreateHtmlReport()
        {
            var stringWriter = new System.IO.StringWriter();
            var indentedWriter = new IndentedTextWriter(stringWriter);
            CreateHtmlReport(indentedWriter);
            return stringWriter.ToString();
        }

        public void CreateHtmlReport(string filename)
        {
            using (var textWriter = new IndentedTextWriter(filename))
            {
                CreateHtmlReport(textWriter);
            }
        }
               
        public void CreateHtmlReport(IndentedTextWriter textWriter)
        {
            int numberOfGamesToLog = 10;
            PlayerAction player1 = this.comparisonResults.comparison.playerActions[0];
            PlayerAction player2 = this.comparisonResults.comparison.playerActions[1];

            int maxTurn = this.comparisonResults.gameEndOnTurnHistogramData.GetXAxisValueCoveringUpTo(97);
            
            var htmlWriter = new HtmlRenderer(textWriter);
            htmlWriter.Begin();
            string game0Text = null;
            for (int gameIndex = 0; gameIndex < numberOfGamesToLog; ++gameIndex)
            {
                string currentGame = this.comparisonResults.comparison.GetHumanReadableGameLog(gameIndex);                    
                htmlWriter.InsertDataDiv("gamelog" + (gameIndex + 1), currentGame);
                if (gameIndex == 0)
                    game0Text = currentGame;
            }
            htmlWriter.Header1(player1.PlayerName + " VS " + player2.PlayerName);
            htmlWriter.WriteLine("Number of Games: " + this.comparisonResults.comparison.numberOfGames);
            htmlWriter.WriteLine(this.comparisonResults.comparison.firstPlayerAdvantage ? player1.PlayerName + " always started first" : "Players took turns going first");

            var pieLabels = new List<string>();
            var pieData = new List<float>();

            for (int index = 0; index < this.comparisonResults.winnerCount.Length; ++index)
            {
                pieLabels.Add(this.comparisonResults.comparison.playerActions[index].name);
                pieData.Add((float)this.comparisonResults.PlayerWinPercent(index));
            }
            if (this.comparisonResults.tieCount > 0)
            {
                pieLabels.Add("Tie");
                pieData.Add((float)this.comparisonResults.TiePercent);
            }

            var statGatherer = this.comparisonResults.statGathererGameLog;
            var gameConfig = this.comparisonResults.comparison.gameConfig;
            var gameEndOnTurnHistogramData = this.comparisonResults.gameEndOnTurnHistogramData;

            htmlWriter.InsertExpander("Who Won?", delegate()
            {
                InsertPieChart(htmlWriter, "Game Breakdown", "Player", "Percent", pieLabels.ToArray(), pieData.ToArray(), colllapsebyDefault: false);
                InsertHistogram(htmlWriter, "Point Spread:  " + player1.PlayerName + " score <= 0 >= " + player2.PlayerName + " score", "Percentage", this.comparisonResults.pointSpreadHistogramData, int.MaxValue, content: delegate()
                {
                    htmlWriter.WriteLine("To the left of 0 are games won by " + player1.PlayerName + ".  To the right are games won by " + player2.PlayerName + ".  The xaxis (absolute value) indicates how many points the game was won by.  The area under the curve indicates the win rate for the corresponding player.");
                });
                InsertLineGraph(htmlWriter, "Probability player is ahead in points at end of round ", player1, player2, statGatherer.oddsOfBeingAheadOnRoundEnd, statGatherer.turnCounters, maxTurn);
                InsertLineGraph(htmlWriter, "Victory Point Total Per Turn", player1, player2, statGatherer.victoryPointTotal, statGatherer.turnCounters, maxTurn);
            }, collapseByDefault: false);
            htmlWriter.InsertExpander("Game Logs", delegate()
            {
                htmlWriter.InsertPaginationControl(numberOfGamesToLog);
                htmlWriter.Write("<textarea id='gameLogTextArea', rows='30' cols='100'>");
                htmlWriter.Write(game0Text);
                htmlWriter.WriteLine("</textarea>");
            });
            htmlWriter.InsertExpander("When does the game end?", delegate()
            {
                InsertHistogram(htmlWriter, "Probablity of Game ending on Turn", "Percentage", gameEndOnTurnHistogramData, maxTurn, colllapsebyDefault: false);
                InsertHistogramIntegrated(htmlWriter, "Probablity of Game being over by turn", "Percentage", gameEndOnTurnHistogramData, maxTurn);
            });
            htmlWriter.InsertExpander("Deck Strength", delegate()
            {
                InsertCardData(htmlWriter, statGatherer.endOfGameCardCount, gameConfig.cardGameSubset, player1, player2);
                InsertLineGraph(htmlWriter, "Coin To Spend Per Turn", player1, player2, statGatherer.coinToSpend, statGatherer.turnCounters, maxTurn, content: delegate()
                {
                    for (int i = 4; i < statGatherer.oddsOfHittingAtLeastACoinAmount.Length; ++i)
                    {
                        InsertLineGraph(htmlWriter, "Odds of Hitting at Least " + i + " coin", player1, player2, statGatherer.oddsOfHittingAtLeastACoinAmount[i], statGatherer.turnCounters, maxTurn);
                    }
                });
                InsertLineGraph(htmlWriter, "Number of cards Gained Per Turn", player1, player2, statGatherer.cardsGained, statGatherer.turnCounters, maxTurn);
                htmlWriter.InsertExpander(player1.PlayerName, delegate()
                {
                    InsertCardData(htmlWriter, "Total Count Of Card", gameConfig.cardGameSubset, statGatherer.cardsTotalCount, statGatherer.turnCounters, 0, maxTurn);
                    InsertCardData(htmlWriter, "Gain Of Card", gameConfig.cardGameSubset, statGatherer.carsGainedOnTurn, statGatherer.turnCounters, 0, maxTurn);
                });
                htmlWriter.InsertExpander(player2.PlayerName, delegate()
                {
                    InsertCardData(htmlWriter, "Total Count Of Card", gameConfig.cardGameSubset, statGatherer.cardsTotalCount, statGatherer.turnCounters, 1, maxTurn);
                    InsertCardData(htmlWriter, "Gain Of Card", gameConfig.cardGameSubset, statGatherer.carsGainedOnTurn, statGatherer.turnCounters, 1, maxTurn);
                });
                InsertLineGraph(htmlWriter, "Shuffles Per Turn", player1, player2, statGatherer.deckShuffleCount, statGatherer.turnCounters, maxTurn);

                InsertLineGraph(htmlWriter, "Ruins Gained Per Turn", player1, player2, statGatherer.ruinsGained, statGatherer.turnCounters, maxTurn);
                InsertLineGraph(htmlWriter, "Curses Gained Per Turn", player1, player2, statGatherer.cursesGained, statGatherer.turnCounters, maxTurn);
                InsertLineGraph(htmlWriter, "Curses Trashed Per Turn", player1, player2, statGatherer.cursesTrashed, statGatherer.turnCounters, maxTurn);
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
                            InsertLineGraph(htmlWriter, "Card Total At Turn", player1, player2, statGatherer.cardsTotalCount[card], statGatherer.turnCounters, maxTurn, colllapsebyDefault: false);
                            InsertLineGraph(htmlWriter, "Card Gained At Turn", player1, player2, statGatherer.carsGainedOnTurn[card], statGatherer.turnCounters, maxTurn, colllapsebyDefault: true);
                        });
                    }
                }
            });

            htmlWriter.End();            
        }

        static string GetHumanReadableGameLog(
            PlayerAction[] playerActions,
            GameConfig gameConfig,
            bool firstPlayerAdvantage,
            int gameNumber)
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

        private static void InsertCardData(
            HtmlRenderer htmlWriter,
            string title,
            CardGameSubset gameSubset,
            MapOfCardsForGameSubset<ForwardAndReversePerTurnPlayerCounters> statsPerCard,
            ForwardAndReversePerTurnPlayerCounters turnCounts,
            int playerIndex,
            int throughTurn)
        {
            Card[] cards = gameSubset.OrderBy(card => card.DefaultCoinCost).ToArray();
            string[] seriesLabel = new string[cards.Length];
            int[] xAxis = Enumerable.Range(1, throughTurn).ToArray();
            float[][] seriesData = new float[cards.Length][];

            for (int i = 0; i < cards.Length; i++)
            {
                seriesLabel[i] = cards[i].name;
                seriesData[i] = statsPerCard[cards[i]].forwardTotal.GetAveragePerTurn(playerIndex, throughTurn, turnCounts.forwardTotal);
            }

            htmlWriter.InsertExpander(title, delegate()
            {
                htmlWriter.InsertLineGraph(title, "Turn", seriesLabel, xAxis, seriesData);
            }, collapseByDefault: true);
        }

        private static void InsertCardData(HtmlRenderer htmlWriter, MapOfCardsForGameSubset<PlayerCounterSeparatedByGame> map, CardGameSubset gameSubset, PlayerAction player1, PlayerAction player2)
        {
            var cards = gameSubset.OrderBy(c => c.DefaultCoinCost);
            var player1Data = new List<float>();
            var player2Data = new List<float>();

            foreach (Card card in cards)
            {
                player1Data.Add(map[card].GetAverage(playerIndex: 0));
                player2Data.Add(map[card].GetAverage(playerIndex: 1));
            }

            htmlWriter.InsertExpander("Cards Report", delegate()
            {
                htmlWriter.InsertColumnChart(
                    "Average Count of Cards Owned at End of Game",
                    "Card",
                    new string[] { player1.name, player2.name },
                    cards.Select(c => c.name).ToArray(),
                    new float[][] { player1Data.ToArray(), player2Data.ToArray() });
            }, collapseByDefault: false);
        }

        private static void InsertLineGraph(
            HtmlRenderer htmlWriter,
            string title,
            PlayerAction player1,
            PlayerAction player2,
            ForwardAndReversePerTurnPlayerCounters forwardAndReverseCounters,
            ForwardAndReversePerTurnPlayerCounters turnCounters,
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
                                forwardAndReverseCounters.forwardTotal.GetAveragePerTurn(0, throughTurn, turnCounters.forwardTotal),
                                forwardAndReverseCounters.forwardTotal.GetAveragePerTurn(1, throughTurn, turnCounters.forwardTotal)
                                );
                    htmlWriter.InsertExpander("Counting back from the end of the Game ...", delegate()
                    {
                        htmlWriter.InsertLineGraph(
                                title,
                                "Turn",
                                player1.PlayerName,
                                player2.PlayerName,
                                Enumerable.Range(0, throughTurn).Select(turn => -turn).ToArray(),
                                forwardAndReverseCounters.reverseTotal.GetAveragePerTurn(0, throughTurn, turnCounters.reverseTotal),
                                forwardAndReverseCounters.reverseTotal.GetAveragePerTurn(1, throughTurn, turnCounters.reverseTotal)
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
    }
}
