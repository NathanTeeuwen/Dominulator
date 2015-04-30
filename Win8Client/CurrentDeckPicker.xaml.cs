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
    public sealed partial class CurrentDeckPicker : UserControl
    {
        public event Action CurrentCardsChanged;

        public CurrentDeckPicker()
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

        public IList<object> SelectedItems
        {
            get
            {
                return this.CurrentCardsListView.SelectedItems;
            }
        }

        private void CurrentCardsListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            Win8Client.SideBySideStrategyView.PrepareDragAndDrop(e);
            this.CurrentCardsListView.SelectedItems.Clear();
        }     

        public void Randomize10Cards()
        {
            var currentDeckSelectedItems = this.CurrentCardsListView.SelectedItems.Select(item => (DominionCard)item).ToArray<DominionCard>();
            var baneSelectedItems = this.BaneCardsListView.SelectedItems.Select(item => (DominionCard)item).ToArray<DominionCard>();
            var eventsSelectedItems = this.EventCardsListView.SelectedItems.Select(item => (DominionCard)item).ToArray<DominionCard>();

            bool isReplacing = currentDeckSelectedItems.Any() || baneSelectedItems.Any() || eventsSelectedItems.Any();
            bool isReducing = this.appDataContext.CurrentDeck.CurrentCards.Count() > 10 && currentDeckSelectedItems.Any();
            bool isGrowing = this.appDataContext.CurrentDeck.CurrentCards.Count() < 10 && this.appDataContext.CurrentDeck.CurrentCards.Any();

            if (!isReplacing && !isGrowing && !isReducing)
            {
                // clean roll
                this.appDataContext.player1Strategy.PurchaseOrderDescriptions.Clear();
                this.appDataContext.player2Strategy.PurchaseOrderDescriptions.Clear();
                this.appDataContext.UpdateSimulationStep();
                this.appDataContext.CurrentDeck.ReapplySortOrder();

                if (this.appDataContext.GetDecksFromWeb.Value)
                {
                    var uiScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();
                    WebService.GetGameConfigFomServer(this.appDataContext).ContinueWith( descrTask =>
                        {
                            GameDescriptionAndRating gameDescriptionAndRating = descrTask.Result;
                            if (gameDescriptionAndRating != null)
                            {
                                this.appDataContext.PopulateFromGameDescription(gameDescriptionAndRating.gameDescription);
                                this.appDataContext.DeckRating.Value = (int)(gameDescriptionAndRating.rating + 0.5);
                            }
                            else
                            {
                                GenerateRandomDeck();
                            }
                        }, uiScheduler);
                }
                else
                {
                    GenerateRandomDeck();
                }
            }
            else
            {
                this.appDataContext.CurrentDeck.CopyOrder();
                DominionCard baneCard = this.appDataContext.BaneCard.CurrentCards.FirstOrDefault();
                bool isCleanRoll = this.appDataContext.CurrentDeck.GenerateRandom(10, ref baneCard, this.appDataContext.AllCards.CurrentCards, itemsToReplace: currentDeckSelectedItems);
                this.appDataContext.BaneCard.PopulateBaneCard(baneCard);
                this.appDataContext.DeckRating.Value = 0;
            }

            string jsonDescription = WebService.ToJson(this.appDataContext.GetGameConfig().gameDescription, 5).Stringify();
            System.Diagnostics.Debug.WriteLine("New Kingdom");
            System.Diagnostics.Debug.WriteLine("===========");
            System.Diagnostics.Debug.WriteLine(jsonDescription);
            
            if (this.CurrentCardsChanged != null)
                this.CurrentCardsChanged();            
        }        

        private void GenerateRandomDeck()
        {
            this.appDataContext.DeckRating.Value = 0;
            var kingdomBuilder = new Dominion.GameConfigBuilder();
            kingdomBuilder.GenerateCompletelyRandomKingdom(this.appDataContext.AllCards.CurrentCards.Select(c => c.dominionCard), MainPage.random);
            Dominion.GameDescription gameDescription = kingdomBuilder.ToGameDescription();
            this.appDataContext.PopulateFromGameDescription(gameDescription);
        }

        private void RefreshButtonClick(object sender, RoutedEventArgs e)
        {
            this.appDataContext.SendRatingToWebIfNecessary();
            Randomize10Cards();
        }        

        public Dominion.Card[] GetSelectedCardsAndClear(bool fShouldClear = true)
        {
            var result = new List<Dominion.Card>();
            
            foreach(DominionCard card in this.CurrentCardsListView.SelectedItems)
            {
                result.Add(card.dominionCard);
            }
            if (fShouldClear)
                this.CurrentCardsListView.SelectedItems.Clear();            
            
            if (this.appDataContext.IsBaneCardVisible.Value)
            {                
                foreach (DominionCard card in this.BaneCardsListView.SelectedItems)
                {
                    result.Add(card.dominionCard);
                }
                if (fShouldClear)
                {
                    this.BaneCardsListView.SelectedItem = null;
                }
            }

            foreach (DominionCard card in this.CommonCardsListView.SelectedItems)
            {
                result.Add(card.dominionCard);
            }
            
            if (fShouldClear)
                this.CommonCardsListView.SelectedItems.Clear();

            foreach (DominionCard card in this.EventCardsListView.SelectedItems)
            {
                result.Add(card.dominionCard);
            }

            if (fShouldClear)
                this.EventCardsListView.SelectedItem = null;

            return result.ToArray();
        }
        
        private void AddSelectedCardsToStrateyDescription(StrategyDescription strategyDescription)
        {
            Dominion.Card[] cards = GetSelectedCardsAndClear();
            var originalDescription = strategyDescription.ConvertToDominionStrategy();
            if (originalDescription.IsEmptyPurchaseOrder)
            {
                originalDescription = Dominion.Strategy.Description.StrategyDescription.GetDefaultDescription(this.appDataContext.GetGameConfig());
            }
            var newDescription = originalDescription.AddCardsToPurchaseOrder(cards);
            strategyDescription.PopulateFrom(newDescription);
        }

        private void AddToStrategy1Click(object sender, RoutedEventArgs e)
        {
            AddSelectedCardsToStrateyDescription(this.appDataContext.player1Strategy);
            this.appDataContext.IsPlayer1StrategyChecked.Value = true;
            this.appDataContext.IsPlayer2StrategyChecked.Value = false;
            this.appDataContext.UpdateSimulationStep();
        }

        private void AddToStrategy2Click(object sender, RoutedEventArgs e)
        {
            AddSelectedCardsToStrateyDescription(this.appDataContext.player2Strategy);
            this.appDataContext.IsPlayer1StrategyChecked.Value = false;
            this.appDataContext.IsPlayer2StrategyChecked.Value = true;
            this.appDataContext.UpdateSimulationStep();
        }

        private void SortCurrentByName(object sender, RoutedEventArgs e)
        {
            this.appDataContext.CurrentDeck.SortByName();
            this.appDataContext.CurrentDeck.UpdateUIFromUIThread();
        }

        private void SortCurrentByCost(object sender, RoutedEventArgs e)
        {
            this.appDataContext.CurrentDeck.SortByCost();
            this.appDataContext.CurrentDeck.UpdateUIFromUIThread();
        }

        private void SortCurrentByExpansion(object sender, RoutedEventArgs e)
        {
            this.appDataContext.CurrentDeck.SortByExpansion();
            this.appDataContext.CurrentDeck.UpdateUIFromUIThread();
        }

        private void SelectedCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.appDataContext.selectedCards.Clear();
            foreach (var item in this.GetSelectedCardsAndClear(fShouldClear:false))
            {
                this.appDataContext.selectedCards.Add(DominionCard.Create(item));
            }
            this.appDataContext.IsSelectionPresentOnCurrentDeck.Value = this.appDataContext.selectedCards.Any();
            this.appDataContext.UpdateSimulationStep();
            this.appDataContext.UpdatePlayerButtonVisibilities();

            this.appDataContext.HintSelectedCardNotSimulatedButtonVisible.Value = 
                this.appDataContext.IsSelectionPresentOnCurrentDeck.Value &&
                !this.appDataContext.CanSimulateCardsInSelection();
        }

        
        private void SimulateGameButtonClick(object sender, RoutedEventArgs e)
        {
            this.appDataContext.SimulateGameButtonClick();
        }

        private void Star1Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleDeckRating(1);
        }

        private void Star2Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleDeckRating(2);
        }

        private void Star3Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleDeckRating(3);
        }

        private void Star4Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleDeckRating(4);
        }

        private void Star5Button_Click(object sender, RoutedEventArgs e)
        {
            ToggleDeckRating(5);
        }

        private void ToggleDeckRating(int value)
        {
            if (this.appDataContext.DeckRating.Value == value)
                this.appDataContext.DeckRating.Value = 0;
            else
                this.appDataContext.DeckRating.Value = value;
        }        
    }
}
