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
using Generic = System.Collections.Generic;

namespace Win8Client
{
    public sealed partial class MainPage : Page
    {
        AppDataContext appDatacontext = new AppDataContext();

        public MainPage()
        {
            this.InitializeComponent();
                       
            this.DataContext = this.appDatacontext;
            var initTask = appDatacontext.AllCards.Populate();

            initTask.ContinueWith((continuation) => appDatacontext.CurrentDeck.Generate10Random(appDatacontext.AllCards.Cards, null));
        }

        private void Randomize(object sender, RoutedEventArgs e)
        {
            var selectedItems = this.CurrentCardsListView.SelectedItems.Select(item => (DominionCard)item).ToArray<DominionCard>();
            this.appDatacontext.CurrentDeck.Generate10Random(this.appDatacontext.AllCards.Cards, itemsToReplace:selectedItems);
        }

        private void SortAllByName(object sender, RoutedEventArgs e)
        {
            this.appDatacontext.AllCards.SortByName();
        }

        private void SortAllByCost(object sender, RoutedEventArgs e)
        {
            this.appDatacontext.AllCards.SortByCost();
        }

        private void SortAllByExpansion(object sender, RoutedEventArgs e)
        {
            this.appDatacontext.AllCards.SortByExpansion();
        }

        private void SortCurrentByName(object sender, RoutedEventArgs e)
        {
            this.appDatacontext.CurrentDeck.SortByName();
        }

        private void SortCurrentByCost(object sender, RoutedEventArgs e)
        {
            this.appDatacontext.CurrentDeck.SortByCost();
        }

        private void SortCurrentByExpansion(object sender, RoutedEventArgs e)
        {
            this.appDatacontext.CurrentDeck.SortByExpansion();
        }

        private void CurrentDeckSelectionChanged(object sender, SelectionChangedEventArgs e)
        {                               
        }
        
    }

    class SortableCardList
        : DependencyObject
    {
        System.Collections.Generic.List<DominionCard> originalCards;
        private System.Collections.ObjectModel.ObservableCollection<DominionCard> cards;
        private Func<DominionCard, bool> filter;
        private Func<DominionCard, object> keySelector;

        public SortableCardList()
        {
            this.originalCards = new List<DominionCard>();
            this.cards = new System.Collections.ObjectModel.ObservableCollection<DominionCard>();
            this.filter = delegate(DominionCard card)
            {
                return true;
            };
            ClearSort();
        }

        private void ClearSort()
        {
            this.keySelector = delegate(DominionCard card)
            {
                return 0;
            };
        }

        public System.Collections.ObjectModel.ObservableCollection<DominionCard> Cards
        {
            get
            {
                return this.cards;
            }
        }

        public void SortByName()
        {
            SortCards(card => card.Name);
        }

        public void SortByCost()
        {
            SortCards(card => card.Coin);
        }

        public void SortByExpansion()
        {
            SortCards(card => card.Expansion);
        }

        public void ApplyFilter(Func<DominionCard, bool> filter)
        {
            this.filter = filter;
        }

        public async System.Threading.Tasks.Task UpdateUI()
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                this.cards.Clear();
                foreach (var item in this.originalCards.Where(this.filter).OrderBy<DominionCard, object>(this.keySelector))
                {
                    this.cards.Add(item);
                }
            });
        }

        private void SortCards(Func<DominionCard, object> keySelector)
        {
            this.keySelector = keySelector;
            this.UpdateUI();
        }

        public async System.Threading.Tasks.Task Populate()
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

                await this.UpdateUI();
            }
            );
        }

        public void Generate10Random(IList<DominionCard> allCards, IList<DominionCard> itemsToReplace)
        {
            bool isReplacingItems = itemsToReplace != null && itemsToReplace.Count > 0;
            var cardPicker = new UniqueCardPicker(allCards);

            if (isReplacingItems)
            {
                cardPicker.ExcludeCards(itemsToReplace);

                if (itemsToReplace != null)
                {
                    foreach (DominionCard cardToReplace in itemsToReplace)
                    {
                        for (int i = 0; i < this.Cards.Count; ++i)
                        {
                            if (this.Cards[i] == cardToReplace)
                            {
                                var nextCard = cardPicker.GetCard();
                                if (nextCard == null)
                                {
                                    this.Cards.Remove(cardToReplace);
                                }
                                else
                                {
                                    this.Cards[i] = nextCard;
                                }
                            }
                        }
                    }
                }
            }

            this.originalCards.Clear();

            if (this.cards.Count < 10 || isReplacingItems)
            {
                this.originalCards.Clear();
                cardPicker.ExcludeCards(this.cards);
                foreach (DominionCard card in this.cards)
                {
                    if (itemsToReplace == null || !itemsToReplace.Contains(card))
                        this.originalCards.Add(card);
                }
                this.ClearSort();
            }

            while (this.originalCards.Count < 10)
            {
                DominionCard currentCard = cardPicker.GetCard();
                if (currentCard == null)
                    break;
                this.originalCards.Add(currentCard);
            }

            this.UpdateUI();
        }

    }

    class AppDataContext
        : DependencyObject
    {        
        private SortableCardList allCards;
        private SortableCardList currentDeck;
        private System.Collections.ObjectModel.ObservableCollection<Expansion> expansions;
        private DependencyObjectDecl<bool, DefaultTrue> use3OrMoreFromExpansions;

        public DependencyObjectDecl<bool, DefaultTrue> Use3OrMoreFromExpansions
        {
            get
            {
                return use3OrMoreFromExpansions;
            }
        }

        public AppDataContext()
        {
            this.allCards = new SortableCardList();
            this.currentDeck = new SortableCardList();
            this.expansions = new System.Collections.ObjectModel.ObservableCollection<Expansion>();
            this.use3OrMoreFromExpansions = new DependencyObjectDecl<bool, DefaultTrue>(this);
                        
            this.expansions.Add(new Expansion("Alchemy", ExpansionIndex.Base));
            this.expansions.Add(new Expansion("Base", ExpansionIndex.Alchemy));
            this.expansions.Add(new Expansion("Cornucopia", ExpansionIndex.Cornucopia));
            this.expansions.Add(new Expansion("Dark Ages", ExpansionIndex.DarkAges));
            this.expansions.Add(new Expansion("Guilds", ExpansionIndex.Guilds));
            this.expansions.Add(new Expansion("Hinterlands", ExpansionIndex.Hinterlands));
            this.expansions.Add(new Expansion("Intrigue", ExpansionIndex.Intrigue));
            this.expansions.Add(new Expansion("Promo", ExpansionIndex.Promo));
            this.expansions.Add(new Expansion("Prosperity", ExpansionIndex.Prosperity));
            this.expansions.Add(new Expansion("Seaside", ExpansionIndex.Seaside));

            foreach(var expansion in expansions)
            {
                expansion.IsEnabled.Changed += ExpansionEnabledChangedEventHandler;
            }

            this.use3OrMoreFromExpansions.Changed += Enable3orMoreFromExpansionsChangedEventHandler;

            this.allCards.ApplyFilter(card => card.Expansion != ExpansionIndex._Unknown && this.expansions[(int)card.Expansion].IsEnabled.Value);
            this.currentDeck.ApplyFilter(card => card.Expansion != ExpansionIndex._Unknown && this.expansions[(int)card.Expansion].IsEnabled.Value);

        }

        public SortableCardList AllCards
        {
            get
            {
                return this.allCards;
            }
        }

        public SortableCardList CurrentDeck
        {
            get
            {
                return this.currentDeck;
            }
        }

        public System.Collections.ObjectModel.ObservableCollection<Expansion> Expansions
        {
            get
            {
                return this.expansions;
            }
        }
        
        public void ExpansionEnabledChangedEventHandler(object owner)
        {
            this.allCards.UpdateUI();
            this.currentDeck.UpdateUI();
        }

        public void Enable3orMoreFromExpansionsChangedEventHandler(object owner)
        {
            this.currentDeck.UpdateUI();
        }
    }

    

    enum ExpansionIndex
    {
        Alchemy = 0,
        Base = 1,
        Cornucopia = 2,
        DarkAges = 3,
        Guilds = 4,
        Hinterlands = 5,
        Intrigue = 6,
        Promo = 7,
        Prosperity = 8,
        Seaside = 9,
        _Unknown = 10,
        _Count = 11
    }

    class Expansion         
    {
        public string Name { get; private set; }
        public ExpansionIndex Index { get; private set; }
        private DependencyObjectDecl<bool, DefaultTrue> isEnabled;        

        public Expansion(string name, ExpansionIndex index)
        {
            this.Name = name;
            this.Index = index;
            this.isEnabled = new DependencyObjectDecl<bool, DefaultTrue>(this);
        }        

        public DependencyObjectDecl<bool, DefaultTrue> IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
        }
    }

    class DominionCard
    {

        public DominionCard(Windows.Data.Json.JsonValue jsonDescription)
        {
            var dictionary = jsonDescription.GetObject();
            this.Id = dictionary.GetNamedString("id");
            this.Name = dictionary.GetNamedString("name");
            this.Coin = (int)dictionary.GetNamedNumber("coin");
            this.Potion = (int)dictionary.GetNamedNumber("potion");
            this.Expansion = GetExpansionIndex(dictionary.GetNamedString("expansion"));
            this.IsAction = dictionary.GetNamedBoolean("isAction");
            this.IsAttack = dictionary.GetNamedBoolean("isAttack");
            this.IsReaction = dictionary.GetNamedBoolean("isReaction");
            this.IsDuration = dictionary.GetNamedBoolean("isDuration");
        }

        public string Name { get; private set; }
        public string Id { get; private set; }
        public int Coin { get; private set; }
        public int Potion { get; private set; }
        public ExpansionIndex Expansion { get; private set; }
        public bool IsAction { get; private set; }
        public bool IsAttack { get; private set; }
        public bool IsReaction { get; private set; }
        public bool IsDuration { get; private set; }

        public string ImageUrl
        {
            get
            {
                return "http://localhost:8081/dominion" + "/resources/cards/" + this.Id + ".jpg";
            }
        }

        private static ExpansionIndex GetExpansionIndex(string value)
        {
            switch (value)
            {
                case "Alchemy": return ExpansionIndex.Alchemy;
                case "Base": return ExpansionIndex.Base;
                case "Cornucopia": return ExpansionIndex.Cornucopia;
                case "DarkAges": return ExpansionIndex.DarkAges;
                case "Guilds": return ExpansionIndex.Guilds;
                case "Hinterlands": return ExpansionIndex.Hinterlands;
                case "Intrigue": return ExpansionIndex.Intrigue;
                case "Promo": return ExpansionIndex.Promo;
                case "Prosperity": return ExpansionIndex.Prosperity;
                case "Seaside": return ExpansionIndex.Seaside;                
            }

            return ExpansionIndex._Unknown;
        }            
    }                
}                    
                     
                     
                     
                     
                     
                     
                     