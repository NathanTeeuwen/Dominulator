using System.Collections.Generic;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Dominion;

namespace Dominulator
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

        internal void UpdateAllCardsListSelection()
        {
            this.appDataContext.isCurrentDeckIgnoringAllDeckSelectionUpdates = true;
            this.SelectedItems.Clear();
            foreach (DominionCard card in this.appDataContext.CurrentDeck.CurrentCards)
            {
                this.SelectedItems.Add(card);
            }
            this.appDataContext.isCurrentDeckIgnoringAllDeckSelectionUpdates = false;
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
            this.appDataContext.isCurrentDeckIgnoringAllDeckSelectionUpdates = true;
            this.appDataContext.AllCards.SortByName();
            this.appDataContext.AllCards.UpdateUIFromUIThread();
            this.UpdateAllCardsListSelection();
            this.appDataContext.isCurrentDeckIgnoringAllDeckSelectionUpdates = false;
        }

        private void SortAllByCost(object sender, RoutedEventArgs e)
        {
            this.appDataContext.isCurrentDeckIgnoringAllDeckSelectionUpdates = true;
            this.appDataContext.AllCards.SortByCost();
            this.appDataContext.AllCards.UpdateUIFromUIThread();
            this.UpdateAllCardsListSelection();
            this.appDataContext.isCurrentDeckIgnoringAllDeckSelectionUpdates = false;
        }

        private void SortAllByExpansion(object sender, RoutedEventArgs e)
        {
            this.appDataContext.isCurrentDeckIgnoringAllDeckSelectionUpdates = true;
            this.appDataContext.AllCards.SortByExpansion();
            this.appDataContext.AllCards.UpdateUIFromUIThread();
            this.UpdateAllCardsListSelection();
            this.appDataContext.isCurrentDeckIgnoringAllDeckSelectionUpdates = false;
        }

        private void DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            Dominulator.SideBySideStrategyView.PrepareDragAndDrop(e);
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
                this.appDataContext.isCurrentDeckIgnoringAllDeckSelectionUpdates = true;
                this.appDataContext.AllCards.UpdateUIFromUIThread();
                this.UpdateAllCardsListSelection();
                this.appDataContext.isCurrentDeckIgnoringAllDeckSelectionUpdates = false;
            }).AsTask();
        }

        private Func<DominionCard, bool> Search(string searchString)
        {
            return delegate (DominionCard domCard)
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

        string[] GetSearchWordsForCard(Dominion.Card card)
        {
            var result = new List<string>();
            result.Add(card.name);
            result.Add(card.expansion.ExpansionToString());
            result.Add(card.DefaultCoinCost.ToString());

            foreach(Dominion.CardType cardType in card.CardTypes)
            {
                result.Add(cardType.CardTypeToString());
                var pluralName = cardType.CardTypeToStringPlural();
                if (pluralName != null)
                    result.Add(pluralName);
            }
            if (card.isEvent)
                result.Add("event");
            if (card.requiresRuins)
                result.Add("ruins");
            if (card.requiresSpoils)
                result.Add("spoils");
            if (card.potionCost != 0)
                result.Add("potion");

            if (this.appDataContext.CurrentDeck.CurrentCards.Where(c => c.dominionCard == card).Any())
            {
                result.Add("current");
                result.Add("deck");
                result.Add("selected");
            }

            return result.ToArray();
        }
    }
}
