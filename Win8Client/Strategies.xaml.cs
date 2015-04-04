using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Win8Client
{
    public sealed partial class Strategies : UserControl
    {
        public Strategies()
        {
            this.InitializeComponent();            
        }

        private AppDataContext appDataContext
        {
            get
            {
                return this.AppDataContext;
            }
        }

        internal AppDataContext AppDataContext
        {
            get
            {
                return (AppDataContext)(base.DataContext);
            }

            set
            {
                this.DataContext = value;
            }
        }

        private async void CurrentCardsListView_Drop(object sender, DragEventArgs e)
        {
            string cardNames = (string)await e.Data.GetView().GetTextAsync("text");

            Dominion.Strategy.Description.StrategyDescription strategy = this.appDataContext.currentStrategy.Value.ConvertToDominionStrategy();


            foreach (var cardName in cardNames.Split(','))
            {
                DominionCard card = DominionCard.Create(cardName);

                if (strategy.purchaseOrderDescription.descriptions.Length == 0)
                {
                    strategy = Dominion.Strategy.Description.StrategyDescription.GetDefaultStrategyDescription(card.dominionCard);
                }
                else
                {
                    strategy = strategy.AddCardToPurchaseOrder(card.dominionCard);
                }
            }

            this.appDataContext.currentStrategy.Value.PopulateFrom(strategy);
        }             


        private bool CanSimulateStrategies(StrategyDescription strategyDescription)
        {
            foreach (var descr in strategyDescription.CardAcceptanceDescriptions)
            {
                if (!descr.CanSimulateCard.Value)
                    return false;
            }
            return true;
        }       

        private void ClearStrategyButtonClick(object sender, RoutedEventArgs e)
        {
            this.appDataContext.currentStrategy.Value.CardAcceptanceDescriptions.Clear();
        }

        private void GetStrategyNames(Dominion.Strategy.Description.StrategyDescription player1Descr, Dominion.Strategy.Description.StrategyDescription player2Descr, out string player1Name, out string player2Name)
        {
            var player1Uniques = new HashSet<Dominion.Card>();
            var player2Uniques = new HashSet<Dominion.Card>();

            foreach (Dominion.Card card in player1Descr.purchaseOrderDescription.descriptions.Select(descr => descr.card))
            {
                player1Uniques.Add(card);
            }

            foreach (Dominion.Card card in player2Descr.purchaseOrderDescription.descriptions.Select(descr => descr.card))
            {
                player2Uniques.Add(card);
                player1Uniques.Remove(card);
            }

            foreach (Dominion.Card card in player1Descr.purchaseOrderDescription.descriptions.Select(descr => descr.card))
            {
                player2Uniques.Remove(card);
            }


            player1Name = string.Join(" ", player1Uniques.Select(c => c.name));
            player2Name = string.Join(" ", player2Uniques.Select(c => c.name));

            return;
        }        


        private bool CanSimulateStrategies()
        {
            return CanSimulateStrategies(this.appDataContext.player1Strategy) && CanSimulateStrategies(this.appDataContext.player2Strategy);
        }

        private void SimulateGameButtonClick(object sender, RoutedEventArgs e)
        {
            if (!CanSimulateStrategies())
                return;

            var uiScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();

            Dominion.Strategy.Description.StrategyDescription player1Descr = this.appDataContext.player1Strategy.ConvertToDominionStrategy();
            Dominion.Strategy.Description.StrategyDescription player2Descr = this.appDataContext.player2Strategy.ConvertToDominionStrategy();

            System.Threading.Tasks.Task<string>.Factory.StartNew(() =>
            {

                string player1nameAppend, player2nameAppend;
                GetStrategyNames(player1Descr, player2Descr, out player1nameAppend, out player2nameAppend);

                var playerActions = new Dominion.Strategy.PlayerAction[] 
                {
                    player1Descr.ToPlayerAction("Player 1: " + player1nameAppend),
                    player2Descr.ToPlayerAction("Player 2: " + player2nameAppend)
                };

                var builder = new Dominion.GameConfigBuilder();
                Dominion.Strategy.PlayerAction.SetKingdomCards(builder, playerActions[0], playerActions[1]);

                builder.useColonyAndPlatinum = false;
                builder.useShelters = false;
                builder.CardSplit = Dominion.StartingCardSplit.Split43;

                bool rotateWhoStartsFirst = true;
                int numberOfGames = 1000;

                Dominion.GameConfig gameConfig = builder.ToGameConfig();
                var strategyComparison = new Dominion.Data.StrategyComparison(playerActions, gameConfig, rotateWhoStartsFirst, numberOfGames);

                Dominion.Data.StrategyComparisonResults strategyComparisonResults = strategyComparison.ComparePlayers();

                var htmlGenerator = new HtmlRenderer.HtmlReportGenerator(strategyComparisonResults);

                var stringWriter = new System.IO.StringWriter();
                var textWriter = new Dominion.IndentedTextWriter(stringWriter);
                htmlGenerator.CreateHtmlReport(textWriter);
                stringWriter.Flush();
                string resultHtml = stringWriter.GetStringBuilder().ToString();
                return resultHtml;
            }).ContinueWith(async (continuation) =>
            {
                this.ResultsWebView.NavigateToString(continuation.Result);
            }, uiScheduler);
        }
    }
}
