using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Win8Client
{
    public sealed partial class AllCardsPicker : UserControl
    {
        public AllCardsPicker()
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
                return this.AllCardsListView.SelectedItems;
            }
        }

        private bool ignoreShelterChanges = false;

        private void SheltersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ignoreShelterChanges)
                return;

            ignoreShelterChanges = true;

            if (e.RemovedItems.Any())
            {
                this.SheltersListView.SelectedItems.Clear();
                this.appDataContext.UseShelters.Value = false;
            }
            else if (e.AddedItems.Any())
            {
                this.SheltersListView.SelectAll();
                this.appDataContext.UseShelters.Value = true;
            }

            ignoreShelterChanges = false;
        }

        private bool ignoreColonyPlatinumChanges = false;

        private void ColonyPlatinumListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ignoreColonyPlatinumChanges)
                return;

            ignoreColonyPlatinumChanges = true;

            if (e.RemovedItems.Any())
            {
                this.ColonyPlatinumListView.SelectedItems.Clear();
                this.appDataContext.UseColonyPlatinum.Value = false;
            }
            else if (e.AddedItems.Any())
            {
                this.ColonyPlatinumListView.SelectAll();
                this.appDataContext.UseColonyPlatinum.Value = true;
            }

            ignoreColonyPlatinumChanges = false;
        }

        private void AllCardsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.AppDataContext.isCurrentDeckIgnoringAllDeckSelectionUpdates)
                return;

            this.AppDataContext.CurrentDeck.UpdateOriginalCards(e.AddedItems.Select(item => (DominionCard)item), e.RemovedItems.Select(item => (DominionCard)item));
        }

        private void SortAllByName(object sender, RoutedEventArgs e)
        {            
            this.appDataContext.AllCards.SortByName();         
        }

        private void SortAllByCost(object sender, RoutedEventArgs e)
        {            
            this.appDataContext.AllCards.SortByCost();            
        }

        private void SortAllByExpansion(object sender, RoutedEventArgs e)
        {            
            this.appDataContext.AllCards.SortByExpansion();         
        }

        private void DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            Win8Client.Strategies.PrepareDragAndDrop(e);
            foreach (var item in e.Items)
            {
                if (!this.AllCardsListView.SelectedItems.Contains(item))
                {
                    this.AllCardsListView.SelectedItems.Add(item);
                }
            }            
        }

        int searchIndexCurrent = 0;

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.searchIndexCurrent++;
            SearchTask(this.searchIndexCurrent);
        }

        private async void SearchTask(int searchIndex)
        {            
            await System.Threading.Tasks.Task.Delay(700);
            if (searchIndex != searchIndexCurrent)
                return;

            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                string searchString = this.SearchTextBox.Text;

                this.appDataContext.AllCards.ApplyFilter2(Search(searchString));            
                this.appDataContext.AllCards.UpdateUIFromUIThread();
            }).AsTask();
        }

        private Func<DominionCard, bool> Search(string searchString)
        {
            return delegate(DominionCard domCard)
            {
                string[] syllables = searchString.Split(' ');
                string[] words = GetSearchWordsForCard(domCard.dominionCard);
                foreach (var syllable in syllables)
                {
                    bool syllableFound = false;
                    foreach (var word in words)
                    {
                        if (word.ToLower().Contains(syllable.ToLower()))
                        {
                            syllableFound = true;
                            break;
                        }
                    }
                    if (syllableFound == false)
                        return false;
                }

                return true;
            };
        }

        string ExpansionToString(Dominion.Expansion expansion)
        {
            switch (expansion)
            {
                case Dominion.Expansion.Alchemy: return "alchemy";
                case Dominion.Expansion.Base: return "base";
                case Dominion.Expansion.Cornucopia: return "cornucopia";
                case Dominion.Expansion.DarkAges: return "dark ages";
                case Dominion.Expansion.Guilds: return "guilds";
                case Dominion.Expansion.Hinterlands: return "hinterlands";
                case Dominion.Expansion.Intrigue: return "intrigue";
                case Dominion.Expansion.Promo: return "promo";
                case Dominion.Expansion.Prosperity: return "prosperity";
                case Dominion.Expansion.Seaside: return "seaside";
                case Dominion.Expansion.Adventures: return "adventures";
                default: throw new NotImplementedException();
            }
        }

        string[] GetSearchWordsForCard(Dominion.Card card)
        {
            var result = new List<string>();
            result.Add(card.name);
            result.Add(ExpansionToString(card.expansion));
            result.Add(card.DefaultCoinCost.ToString());
            if (card.isAction)
                result.Add("action");
            if (card.isAttack)
                result.Add("attack");
            if (card.isReaction)
                result.Add("reaction");
            if (card.isVictory)
                result.Add("victory");
            if (card.isTreasure)
                result.Add("treasure");
            if (card.isShelter)
                result.Add("shelter");
            if (card.isReserve)
                result.Add("reserve");
            if (card.isTraveller)
                result.Add("traveller");
            if (card.isEvent)
                result.Add("event");
            if (card.isDuration)
                result.Add("duration");
            if (card.requiresRuins)
                result.Add("ruins");
            if (card.requiresSpoils)
                result.Add("spoils");
            if (card.isPrize)
                result.Add("prize");
            if (card.isRuins)
                result.Add("ruins");
            if (card.isRuins)
                result.Add("ruins");
            if (card.potionCost != 0)
                result.Add("potion");

            return result.ToArray();
        }
    }
}
