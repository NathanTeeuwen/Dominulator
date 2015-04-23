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
            Win8Client.Strategies.PrepareDragAndDrop(e);
            this.CurrentCardsListView.SelectedItems.Clear();
        }     

        public void Randomize10Cards()
        {
            var selectedItems = this.CurrentCardsListView.SelectedItems.Select(item => (DominionCard)item).ToArray<DominionCard>();

            bool useShelter = this.appDataContext.UseShelters.Value;
            bool useColony = this.appDataContext.UseColonyPlatinum.Value;
            DominionCard baneCard = this.appDataContext.BaneCard.CurrentCards.FirstOrDefault();

            bool isCleanRoll = this.appDataContext.CurrentDeck.GenerateRandom(10, ref baneCard, this.appDataContext.AllCards.CurrentCards, itemsToReplace: selectedItems);
            this.appDataContext.BaneCard.PopulateBaneCard(baneCard);            

            if (isCleanRoll)
            {                
                this.appDataContext.UseColonyPlatinum.Value = ShouldIncludeExpansion(Dominion.Expansion.Prosperity);
                this.appDataContext.UseShelters.Value = ShouldIncludeExpansion(Dominion.Expansion.DarkAges);
                ReRollEvents();

                this.appDataContext.player1Strategy.CardAcceptanceDescriptions.Clear();
                this.appDataContext.player2Strategy.CardAcceptanceDescriptions.Clear();
            }
            
            if (this.CurrentCardsChanged != null)
                this.CurrentCardsChanged();            
        }

        private bool ShouldIncludeExpansion(Dominion.Expansion expansion)
        {
            int cExpansion = this.appDataContext.CurrentDeck.CurrentCards.Select(c => c.dominionCard).Where(c => c.expansion == expansion).Count();
            int roll = MainPage.random.Next(1, 10);
            return cExpansion >= roll ? true : false;
        }

        private void ReRollEvents()
        {
            int cEventsToInclude = 0;
            
            int cEventRemaining = 20;
            int totalKingdomCount = Dominion.Cards.AllKingdomCards().Count();
            for (int i = 0; i < 10; ++i)
            {
                int roll = MainPage.random.Next(totalKingdomCount);
                if (roll <= cEventRemaining)
                {
                    cEventsToInclude++;
                    cEventRemaining--;
                    i--;
                    continue;
                }                
                totalKingdomCount--;
            }

            var allEventsCards = Dominion.Cards.AllCards().Where(c => c.isEvent).Select(c => DominionCard.Create(c)).ToArray();
            var selectedItems = this.EventCardsListView.SelectedItems.Select(item => (DominionCard)item).ToArray<DominionCard>();

            var cardPicker = new UniqueCardPicker(allEventsCards.Select(c => c.dominionCard));
            DominionCard baneCard = null;
            this.appDataContext.EventCards.Clear();
            this.appDataContext.EventCards.GenerateRandom(cEventsToInclude, ref baneCard, allEventsCards, itemsToReplace: selectedItems);
        }

        private void RandomizeButtonClick(object sender, RoutedEventArgs e)
        {
            Randomize10Cards();
        }

        private void GoogleButtonClick(object sender, RoutedEventArgs e)
        {
            var uriBuilder = new System.Text.StringBuilder();
            uriBuilder.Append("https://www.google.com/?gws_rd=ssl#safe=off&q=dominion");

            foreach(Dominion.Card card in this.GetSelectedCardsAndClear(fShouldClear:false))
            {
                uriBuilder.Append("+");
                uriBuilder.Append(card.name.Replace(" ", "%20"));
            }
            
            string uriToLaunch = uriBuilder.ToString();
            var uri = new Uri(uriToLaunch);
            Windows.System.Launcher.LaunchUriAsync(uri);
        }

        Dominion.Card[] GetSelectedCardsAndClear(bool fShouldClear = true)
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
            var newDescription = originalDescription.AddCardsToPurchaseOrder(cards);
            strategyDescription.PopulateFrom(newDescription);
        }

        private void AddToStrategy1Click(object sender, RoutedEventArgs e)
        {
            AddSelectedCardsToStrateyDescription(this.appDataContext.player1Strategy);
            this.appDataContext.IsPlayer1StrategyChecked.Value = true;
            this.appDataContext.IsPlayer2StrategyChecked.Value = false;
        }

        private void AddToStrategy2Click(object sender, RoutedEventArgs e)
        {
            AddSelectedCardsToStrateyDescription(this.appDataContext.player2Strategy);
            this.appDataContext.IsPlayer1StrategyChecked.Value = false;
            this.appDataContext.IsPlayer2StrategyChecked.Value = true;
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
    }
}
