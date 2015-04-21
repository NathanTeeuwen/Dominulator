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

        public void SetSplit(Dominion.StartingCardSplit split)
        {
            switch (split)
            {
                case Dominion.StartingCardSplit.Random: this.OpenRandomRadioButton.IsChecked = true; break;
                case Dominion.StartingCardSplit.Split25: this.Open2RadioButton.IsChecked = true; break;
                case Dominion.StartingCardSplit.Split34: this.Open3RadioButton.IsChecked = true; break;
                case Dominion.StartingCardSplit.Split43: this.Open4RadioButton.IsChecked = true; break;
                case Dominion.StartingCardSplit.Split52: this.Open5RadioButton.IsChecked = true; break;
            }
        }

        public static void PrepareDragAndDrop(DragItemsStartingEventArgs e)
        {                        
            var cardListAsString = string.Join(",", e.Items.Select(card => ((DominionCard)card).dominionCard.name));

            e.Data.SetData("text", cardListAsString);
        }

        private async void CurrentCardsListView_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetView().AvailableFormats.Contains("text"))
                return;

            string cardNames = (string)await e.Data.GetView().GetTextAsync("text");

            Dominion.Strategy.Description.StrategyDescription strategy = this.appDataContext.currentStrategy.Value.ConvertToDominionStrategy();


            foreach (var cardName in cardNames.Split(','))
            {
                DominionCard card = DominionCard.Create(cardName);

                if (strategy.purchaseOrderDescription.descriptions.Length == 0)
                {
                    strategy = Dominion.Strategy.Description.StrategyDescription.GetDefaultStrategyDescription();
                }
                
                strategy = strategy.AddCardToPurchaseOrder(card.dominionCard);
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

            this.appDataContext.StrategyResultsAvailable.Value = false;

            var uiScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();

            Dominion.Strategy.Description.StrategyDescription player1Descr = this.appDataContext.player1Strategy.ConvertToDominionStrategy();
            Dominion.Strategy.Description.StrategyDescription player2Descr = this.appDataContext.player2Strategy.ConvertToDominionStrategy();

            Dominion.GameConfig gameConfig = this.appDataContext.GetGameConfig();

            System.Diagnostics.Debug.WriteLine("Player 1: ");
            System.Diagnostics.Debug.WriteLine(player1Descr.ToString());
            System.Diagnostics.Debug.WriteLine("Player 2: ");
            System.Diagnostics.Debug.WriteLine(player2Descr.ToString());

            System.Threading.Tasks.Task<StrategyUIResults>.Factory.StartNew(() =>
            {

                string player1nameAppend, player2nameAppend;
                GetStrategyNames(player1Descr, player2Descr, out player1nameAppend, out player2nameAppend);

                string player1Name = "Player 1: " + player1nameAppend;
                string player2Name = "Player 2: " + player2nameAppend;

                var playerActions = new Dominion.Strategy.PlayerAction[] 
                {
                    player1Descr.ToPlayerAction(player1Name),
                    player2Descr.ToPlayerAction(player2Name)
                };

                bool rotateWhoStartsFirst = true;
                int numberOfGames = 1000;

                var strategyComparison = new Dominion.Data.StrategyComparison(playerActions, gameConfig, rotateWhoStartsFirst, numberOfGames);

                Dominion.Data.StrategyComparisonResults strategyComparisonResults = strategyComparison.ComparePlayers(randomSeed: MainPage.random.Next());               
                return new StrategyUIResults()
                {
                    strategyComparisonResults = strategyComparisonResults,
                    Player1Name = player1Name,
                    Player2Name = player2Name,
                    Player1WinPercent = strategyComparisonResults.PlayerWinPercent(0),
                    Player2WinPercent = strategyComparisonResults.PlayerWinPercent(1),
                    TiePercent = strategyComparisonResults.TiePercent,
                };
            }).ContinueWith(async (continuation) =>
            {
                var results = (StrategyUIResults )continuation.Result;

                this.appDataContext.strategyReportDirty = true;
                this.appDataContext.strategyComparisonResults = results.strategyComparisonResults;
                this.appDataContext.Player1Name.Value = results.Player1Name;
                this.appDataContext.Player2Name.Value = results.Player2Name;
                this.appDataContext.Player1WinPercent.Value = results.Player1WinPercent;
                this.appDataContext.Player2WinPercent.Value = results.Player2WinPercent;
                this.appDataContext.TiePercent.Value = results.TiePercent;                
                this.appDataContext.StrategyResultsAvailable.Value = true;
            }, uiScheduler);
        }

        private class StrategyUIResults
        {
            public Dominion.Data.StrategyComparisonResults strategyComparisonResults;
            public string Player1Name;
            public string Player2Name;
            public double Player1WinPercent;
            public double Player2WinPercent;
            public double TiePercent;
        }

        private void OpenSplitRandom_Checked(object sender, RoutedEventArgs e)
        {
            this.appDataContext.currentStrategy.Value.StartingCardSplit.Value = Dominion.StartingCardSplit.Random;
        }

        private void OpenSplit2_Checked(object sender, RoutedEventArgs e)
        {
            this.appDataContext.currentStrategy.Value.StartingCardSplit.Value = Dominion.StartingCardSplit.Split25;
        }

        private void OpenSplit3_Checked(object sender, RoutedEventArgs e)
        {
            this.appDataContext.currentStrategy.Value.StartingCardSplit.Value = Dominion.StartingCardSplit.Split34;
        }

        private void OpenSplit4_Checked(object sender, RoutedEventArgs e)
        {
            this.appDataContext.currentStrategy.Value.StartingCardSplit.Value = Dominion.StartingCardSplit.Split43;
        }

        private void OpenSplit5_Checked(object sender, RoutedEventArgs e)
        {
            this.appDataContext.currentStrategy.Value.StartingCardSplit.Value = Dominion.StartingCardSplit.Split52;
        }

        private void PlayerRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            if (this.appDataContext.IsPlayer1StrategyChecked.Value)
            {
                this.appDataContext.CurrentStrategy.Value = this.appDataContext.player1Strategy;
                SetSplit(this.appDataContext.currentStrategy.Value.StartingCardSplit.Value);                
            }
        }

        private void PlayerRadioButtonChecked2(object sender, RoutedEventArgs e)
        {
            if (this.appDataContext.IsPlayer2StrategyChecked.Value)
            {
                this.appDataContext.CurrentStrategy.Value = this.appDataContext.player2Strategy;
                SetSplit(this.appDataContext.currentStrategy.Value.StartingCardSplit.Value);
            }
        }
    }
}
