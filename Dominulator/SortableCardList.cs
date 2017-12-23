using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Generic = System.Collections.Generic;
using System.ComponentModel;

namespace Dominulator
{
    public class SortableCardList
       : DependencyObject
    {
        System.Collections.Generic.List<DominionCard> originalCards;
        private System.Collections.ObjectModel.ObservableCollection<DominionCard> cards;
        private Func<DominionCard, bool> filter;
        private Func<DominionCard, bool> filter2;
        private Func<DominionCard, object> keySelector;

        public DependencyObjectDecl<string, DefaultEmptyString> CurrentSort { get; private set; }
        private SortOrder currentSortOrder;

        public event PropertyChangedEventHandler PropertyChanged;

        public System.Collections.ObjectModel.ObservableCollection<DominionCard> Cards
        {
            get
            {
                return this.cards;
            }
        }

        public Generic.IEnumerable<DominionCard> CurrentCards
        {
            get
            {
                return this.originalCards.Where(this.filter).Where(this.filter2).OrderBy<DominionCard, object>(this.keySelector);
            }
        }

        public SortableCardList()
        {
            this.originalCards = new List<DominionCard>();
            this.cards = new System.Collections.ObjectModel.ObservableCollection<DominionCard>();
            this.filter = delegate(DominionCard card)
            {
                return true;
            };
            this.filter2 = delegate(DominionCard card)
            {
                return true;
            };
            this.CurrentSort = new DependencyObjectDecl<string, DefaultEmptyString>(this);
            ClearSort();
        }

        private void ClearSort()
        {
            DefaultOrderNoSort();
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => this.CurrentSort.Value = "Not Sorted");
        }

        enum SortOrder
        {
            Default,
            ByName,
            ByCost,
            ByExpansion
        }

        public void DefaultOrderNoSort()
        {
            this.keySelector = delegate(DominionCard card)
            {
                return 0;
            };
        }

        public void SortByName()
        {
            SortCards(card => card.Name);
            this.CurrentSort.Value = "By Name";
            this.currentSortOrder = SortOrder.ByName;
        }

        public void SortByCost()
        {
            SortCards(card => card.Coin);
            this.CurrentSort.Value = "By Cost";
            this.currentSortOrder = SortOrder.ByCost;
        }

        public void SortByExpansion()
        {
            SortCards(card => card.Expansion);
            this.CurrentSort.Value = "By Expansion";
            this.currentSortOrder = SortOrder.ByExpansion;
        }

        public void ReapplySortOrder()
        {
            switch (this.currentSortOrder)
            {
                case SortOrder.ByExpansion: SortByExpansion(); break;
                case SortOrder.ByName: SortByName(); break;
                case SortOrder.ByCost: SortByCost(); break;
            }
        }

        public void ApplyFilter(Func<DominionCard, bool> filter)
        {
            this.filter = filter;
        }

        public void ApplyFilter2(Func<DominionCard, bool> filter)
        {
            this.filter = filter;
        }

        public System.Threading.Tasks.Task UpdateUI()
        {
            return this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UpdateUIFromUIThread();
            }).AsTask();
        }

        public void UpdateUIFromUIThread()
        {
            this.cards.Clear();
            foreach (var item in this.CurrentCards)
            {
                this.cards.Add(item);
            }

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, null);
        }

        private void SortCards(Func<DominionCard, object> keySelector)
        {
            this.keySelector = keySelector;
            var newCards = this.originalCards.OrderBy(this.keySelector).ToArray();
            this.originalCards.Clear();
            this.originalCards.AddRange(newCards);
        }

        public System.Threading.Tasks.Task Populate()
        {
            return PopulateFromResources().ContinueWith(async (continuation) =>
            {
                await this.UpdateUI();
            });
        }

        public async void Clear()
        {
            this.originalCards.Clear();
            await this.UpdateUI();
        }

        public System.Threading.Tasks.Task PopulateShelters()
        {
            return PopulateSheltersFromResources().ContinueWith(async (continuation) =>
            {
                await this.UpdateUI();
            });
        }

        public System.Threading.Tasks.Task PopulateColonyPlatinum()
        {
            return PopulatColonyPlatinumFromResources().ContinueWith(async (continuation) =>
            {
                await this.UpdateUI();
            });
        }

        public System.Threading.Tasks.Task PopulateCommon(Dominion.GameConfig gameConfig)
        {
            return PopulateCommonFromResources(gameConfig).ContinueWith(async (continuation) =>
            {
                await this.UpdateUI();
            });
        }

        private async System.Threading.Tasks.Task PopulateSheltersFromResources()
        {
            foreach (Dominion.Card card in Dominion.Cards.Shelters)
            {
                this.originalCards.Add(DominionCard.Create(card));
            }
        }

        private async System.Threading.Tasks.Task PopulatColonyPlatinumFromResources()
        {
            this.originalCards.Add(DominionCard.Create(Dominion.Cards.Platinum));
            this.originalCards.Add(DominionCard.Create(Dominion.Cards.Colony));
        }

        private async System.Threading.Tasks.Task PopulateFromResources()
        {
            foreach (Dominion.Card card in Dominion.Cards.AllKingdomCards())
            {
                this.originalCards.Add(DominionCard.Create(card));
            }
        }

        public void PopulateBaneCard(DominionCard card)
        {
            if (this.originalCards.Contains(card))
                return;
            if (this.originalCards.Any())
                this.originalCards.Clear();
            if (card != null)
                this.originalCards.Add(card);
            this.UpdateUIFromUIThread();
        }

        private async System.Threading.Tasks.Task PopulateCommonFromResources(Dominion.GameConfig gameConfig)
        {
            this.originalCards.Clear();

            Dominion.CardGainAvailablility[] availabilities = gameConfig.GetCardAvailability(1, Dominion.CardAvailabilityType.AdditionalCardsAfterKingdom);

            foreach (var availability in availabilities)
            {
                this.originalCards.Add(DominionCard.Create(availability.card));
            }
        }

        private async System.Threading.Tasks.Task PopulateFromWeb()
        {
            var client = new DominulatorWebClient();
            var allCards = client.GetAllCards();

            await allCards.ContinueWith(async (continuation) =>
            {
                var jsonObject = continuation.Result;
                if (jsonObject == null)
                    return;
                var jsonArray = jsonObject.GetArray();
                if (jsonArray == null)
                    return;


                this.originalCards.Clear();
                foreach (Windows.Data.Json.JsonValue currentItem in jsonArray)
                {
                    this.originalCards.Add(new DominionCard(currentItem));
                }
            }
            );
        }

        public bool GenerateRandom(
            int targetCount,
            ref DominionCard baneCard,
            IEnumerable<DominionCard> allCards,
            IEnumerable<DominionCard> itemsToReplace)
        {
            bool isCleanRoll = MainPage.GenerateRandom(
                targetCount,
                ref baneCard,
                this.originalCards, this.Cards, allCards, itemsToReplace);

            if (!isCleanRoll)
                this.ClearSort();
            else
                ReapplySortOrder();

            this.UpdateUIFromUIThread();

            return isCleanRoll;
        }

        public void PopulateCards(IEnumerable<DominionCard> cards)
        {
            this.originalCards.Clear();
            this.originalCards.AddRange(cards);
            this.UpdateUIFromUIThread();
        }

        public void CopyOrder()
        {
            var currentCards = CurrentCards.ToArray();
            this.originalCards.Clear();
            this.originalCards.AddRange(currentCards);
        }

        public void UpdateOriginalCards(IEnumerable<DominionCard> addedCards, IEnumerable<DominionCard> removedCards)
        {
            foreach (var card in addedCards)
            {
                this.originalCards.Add(card);
            }

            foreach (var card in removedCards)
            {
                this.originalCards.Remove(card);
            }

            this.UpdateUIFromUIThread();
        }
    }    
   
}
