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
using System.ComponentModel;

namespace Win8Client
{
    public sealed partial class MainPage : Page
    {
        AppDataContext appDatacontext;

        public MainPage()
        {
            this.appDatacontext = new AppDataContext(this);
  
            this.InitializeComponent();

      
            this.DataContext = this.appDatacontext;
            var uiScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();           

            System.Threading.Tasks.Task.WhenAll(
                appDatacontext.AllCards.Populate(),
                appDatacontext.CommonCards.PopulateCommon()
                ).ContinueWith(delegate(System.Threading.Tasks.Task task)
                    {
                        appDatacontext.PopulateAllCardsMap();
                        Randomize10Cards();                 
                    }, uiScheduler);

            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateAllCardsListSelection();
        }

        private void RandomizeButtonClick(object sender, RoutedEventArgs e)
        {            
            Randomize10Cards();
        }

        private void Randomize10Cards()
        {
            var selectedItems = this.CurrentCardsListView.SelectedItems.Select(item => (DominionCard)item).ToArray<DominionCard>();
            this.appDatacontext.CurrentDeck.Generate10Random(this.appDatacontext.AllCards.Cards, itemsToReplace: selectedItems);
            UpdateAllCardsListSelection();
        }

        internal bool isCurrentDeckIgnoringAllDeckSelectionUpdates = false;
        internal void UpdateAllCardsListSelection()
        {
            this.isCurrentDeckIgnoringAllDeckSelectionUpdates = true;
            this.AllCardsListView.SelectedItems.Clear();
            foreach (DominionCard card in this.appDatacontext.CurrentDeck.CurrentCards)
            {
                this.AllCardsListView.SelectedItems.Add(card);
            }
            this.isCurrentDeckIgnoringAllDeckSelectionUpdates = false;
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

        private async void CurrentCardsListView_Drop(object sender, DragEventArgs e)
        {
            string cardName = (string)await e.Data.GetView().GetTextAsync("data");

            DominionCard card = this.appDatacontext.mapNameToCard[cardName];

            if (this.appDatacontext.currentStrategy.Value.CardAcceptanceDescriptions.Count == 0)
            {
                var defaultDescr = Dominion.Strategy.Description.StrategyDescription.GetDefaultStrategyDescription(card.dominionCard);
                this.appDatacontext.currentStrategy.Value.PopulateFrom(defaultDescr);
            }
            else
            {
                this.appDatacontext.currentStrategy.Value.CardAcceptanceDescriptions.Add(
                    new CardAcceptanceDescription(card));
            }
        }             

        private void CurrentCardsListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            try
            {
                e.Data.SetData("data", (e.Items[0] as DominionCard).Name);
            }
            catch
            { }
        }

        private void Player1RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            appDatacontext.CurrentStrategy.Value = appDatacontext.player1Strategy;
        }

        private void Player2RadioButtonChecked(object sender, RoutedEventArgs e)
        {
            appDatacontext.CurrentStrategy.Value = appDatacontext.player2Strategy;
        }

        private void ClearStrategyButtonClick(object sender, RoutedEventArgs e)
        {
            this.appDatacontext.currentStrategy.Value.CardAcceptanceDescriptions.Clear();
        }

        private void SimulateGameButtonClick(object sender, RoutedEventArgs e)
        {
            var uiScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext(); 
          
            Dominion.Strategy.Description.StrategyDescription player1Descr = this.appDatacontext.player1Strategy.ConvertToDominionStrategy();
            Dominion.Strategy.Description.StrategyDescription player2Descr = this.appDatacontext.player2Strategy.ConvertToDominionStrategy();

            System.Threading.Tasks.Task<string>.Factory.StartNew(() =>
            {

                var playerActions = new Dominion.Strategy.PlayerAction[] 
                {
                    player1Descr.ToPlayerAction("Player 1"),
                    player2Descr.ToPlayerAction("Player 2")
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

        internal static void Generate10Random(IList<DominionCard> resultList, IList<DominionCard> sourceList, IList<DominionCard> allCards, IList<DominionCard> itemsToReplace)
        {
            bool isReplacingItems = itemsToReplace != null && itemsToReplace.Count > 0 && sourceList.Count <= 10;
            bool isReducingItems = itemsToReplace != null && itemsToReplace.Count > 0 && sourceList.Count > 10;
            var cardPicker = new UniqueCardPicker(allCards);

            if (isReplacingItems)
            {
                cardPicker.ExcludeCards(itemsToReplace);

                if (itemsToReplace != null)
                {
                    foreach (DominionCard cardToReplace in itemsToReplace)
                    {
                        for (int i = 0; i < resultList.Count; ++i)
                        {
                            if (resultList[i] == cardToReplace)
                            {
                                var nextCard = cardPicker.GetCard(c => true);
                                if (nextCard == null)
                                {
                                    resultList.Remove(cardToReplace);
                                    i--;  // do this index again
                                }
                                else
                                {
                                    resultList[i] = nextCard;
                                }
                            }
                        }
                    }
                }
            }
            else if (sourceList.Count < 10)
            {
                var listRemoved = new List<DominionCard>();
                foreach(var card in resultList)
                {
                    if (!sourceList.Contains(card))
                        listRemoved.Add(card);
                }

                foreach(var card in listRemoved)
                {
                    resultList.Remove(card);
                }                       
            }
            else if (isReducingItems)
            {
                foreach (var card in itemsToReplace)
                {
                    resultList.Remove(card);
                }
            }
            else
            {
                resultList.Clear();
            }

            
            while (resultList.Count < 10)
            {
                DominionCard currentCard = cardPicker.GetCard(c => true);
                if (currentCard == null)
                    break;
                resultList.Add(currentCard);
            }
        }

        private void AllCardsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.isCurrentDeckIgnoringAllDeckSelectionUpdates)
                return;
            this.appDatacontext.CurrentDeck.UpdateOriginalCards(e.AddedItems.Select(item => (DominionCard)item), e.RemovedItems.Select(item => (DominionCard)item));            
        }
    }

    class SortableCardList
        : DependencyObject
    {        
        System.Collections.Generic.List<DominionCard> originalCards;
        private System.Collections.ObjectModel.ObservableCollection<DominionCard> cards;
        private Func<DominionCard, bool> filter;
        private Func<DominionCard, object> keySelector;
        
        public DependencyObjectDecl<string, DefaultEmptyString> CurrentSort { get; private set;}

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
                return this.originalCards.Where(this.filter).OrderBy<DominionCard, object>(this.keySelector);
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
            this.CurrentSort = new DependencyObjectDecl<string, DefaultEmptyString>(this);
            ClearSort();
        }

        private void ClearSort()
        {
            this.keySelector = delegate(DominionCard card)
            {
                return 0;
            };
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => this.CurrentSort.Value = "Not Sorted");            
        }
     
        public void SortByName()
        {
            SortCards(card => card.Name);
            this.CurrentSort.Value = "By Name";
        }

        public void SortByCost()
        {
            SortCards(card => card.Coin);
            this.CurrentSort.Value = "By Cost";
        }

        public void SortByExpansion()
        {
            SortCards(card => card.Expansion);
            this.CurrentSort.Value = "By Expansion";
        }

        public void ApplyFilter(Func<DominionCard, bool> filter)
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
        }

        private void SortCards(Func<DominionCard, object> keySelector)
        {
            this.keySelector = keySelector;
            this.UpdateUI();
        }

        public System.Threading.Tasks.Task Populate()
        {
            return PopulateFromResources().ContinueWith(async (continuation) =>
            {
                await this.UpdateUI();
            });
        }

        public System.Threading.Tasks.Task PopulateCommon()
        {
            return PopulateCommonFromResources().ContinueWith(async (continuation) =>
            {
                await this.UpdateUI();
            });
        }

        private async System.Threading.Tasks.Task PopulateFromResources()
        {
            foreach (Dominion.Card card in Dominion.Cards.AllKingdomCards())
            {
                this.originalCards.Add(new DominionCard(card));
            }
        }

        private async System.Threading.Tasks.Task PopulateCommonFromResources()
        {
            Dominion.Card[] commonCards = new Dominion.Card[] {
                Dominion.Cards.Province,
                Dominion.Cards.Duchy,
                Dominion.Cards.Estate,
                Dominion.Cards.Gold,
                Dominion.Cards.Silver,
                Dominion.Cards.Copper,
            };
            foreach (Dominion.Card card in commonCards)
            {
                this.originalCards.Add(new DominionCard(card));
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

        public void Generate10Random(IList<DominionCard> allCards, IList<DominionCard> itemsToReplace)
        {
            MainPage.Generate10Random(this.originalCards, this.Cards, allCards, itemsToReplace);
            this.UpdateUIFromUIThread();
        }       

        public void UpdateOriginalCards(IEnumerable<DominionCard> addedCards, IEnumerable<DominionCard> removedCards)
        {
            foreach(var card in addedCards)
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

    class AppDataContext
        : DependencyObject
    {
        public System.Collections.Generic.Dictionary<string, DominionCard> mapNameToCard = new System.Collections.Generic.Dictionary<string, DominionCard>();
        private SortableCardList allCards;
        private SortableCardList currentDeck;
        private SortableCardList commonCards;
        private System.Collections.ObjectModel.ObservableCollection<Expansion> expansions;        

        public DependencyObjectDecl<bool, DefaultTrue> Use3OrMoreFromExpansions { get; private set;}
        public DependencyObjectDecl<bool, DefaultTrue> RequireTrashing { get; private set; }
        public DependencyObjectDecl<bool, DefaultTrue> RequirePlusCards { get; private set; }
        public DependencyObjectDecl<bool, DefaultTrue> RequirePlusBuy { get; private set; }
        public DependencyObjectDecl<bool, DefaultTrue> RequirePlus2Actions { get; private set; }
        public DependencyObjectDecl<bool, DefaultTrue> RequireAttack { get; private set; }
        public DependencyObjectDecl<bool, DefaultTrue> AllowAttack { get; private set; }

        public DependencyObjectDecl<StrategyDescription, DefaultEmptyStrategyDescription> currentStrategy { get; private set; }
        public StrategyDescription player1Strategy { get; private set; }
        public StrategyDescription player2Strategy { get; private set; }

        private MainPage mainPage;

        public AppDataContext(MainPage mainPage)
        {
            this.mainPage = mainPage;

            this.allCards = new SortableCardList();
            this.currentDeck = new SortableCardList();
            this.commonCards = new SortableCardList();
            this.expansions = new System.Collections.ObjectModel.ObservableCollection<Expansion>();
            this.Use3OrMoreFromExpansions = new DependencyObjectDecl<bool, DefaultTrue>(this);
            this.RequireTrashing = new DependencyObjectDecl<bool, DefaultTrue>(this);
            this.RequirePlusCards = new DependencyObjectDecl<bool, DefaultTrue>(this);
            this.RequirePlusBuy = new DependencyObjectDecl<bool, DefaultTrue>(this);
            this.RequirePlus2Actions = new DependencyObjectDecl<bool, DefaultTrue>(this);
            this.RequireAttack = new DependencyObjectDecl<bool, DefaultTrue>(this);
            this.AllowAttack = new DependencyObjectDecl<bool, DefaultTrue>(this);

            this.player1Strategy = new StrategyDescription();
            this.player2Strategy = new StrategyDescription();
            this.currentStrategy = new DependencyObjectDecl<StrategyDescription, DefaultEmptyStrategyDescription>(this);
            this.currentStrategy.Value = this.player1Strategy;
                                    
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
                expansion.IsEnabled.PropertyChanged += ExpansionEnabledChangedEventHandler;
            }

            this.Use3OrMoreFromExpansions.PropertyChanged += Enable3orMoreFromExpansionsChangedEventHandler;

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

        public SortableCardList CommonCards
        {
            get
            {
                return this.commonCards;
            }
        }

        public System.Collections.ObjectModel.ObservableCollection<Expansion> Expansions
        {
            get
            {
                return this.expansions;
            }
        }

        public DependencyObjectDecl<StrategyDescription, DefaultEmptyStrategyDescription> CurrentStrategy
        {
            get
            {
                return this.currentStrategy;
            }
        }
        
        public void ExpansionEnabledChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            this.mainPage.isCurrentDeckIgnoringAllDeckSelectionUpdates = true;
            this.allCards.UpdateUIFromUIThread();
            this.currentDeck.UpdateUIFromUIThread();
            this.mainPage.isCurrentDeckIgnoringAllDeckSelectionUpdates = false;
            this.mainPage.UpdateAllCardsListSelection();
        }

        public void Enable3orMoreFromExpansionsChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            this.currentDeck.UpdateUI();
        }

        public void PopulateAllCardsMap()
        {
            foreach(DominionCard card in this.AllCards.Cards)
            {
                this.mapNameToCard[card.Name] = card;
            }

            foreach (DominionCard card in this.CommonCards.Cards)
            {
                this.mapNameToCard[card.Name] = card;
            }
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

    public class ComparisonToIntegerConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var comparison = (Dominion.Strategy.Description.Comparison)value;
            return (int)comparison;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            int intValue = (int)value;
            return (Dominion.Strategy.Description.Comparison)intValue;
        }
    }

    public class CountSourceToIntegerConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var comparison = (Dominion.Strategy.Description.CountSource)value;
            return (int)comparison;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            int intValue = (int)value;
            return (Dominion.Strategy.Description.CountSource)intValue;
        }
    }

    public class BoolToVisibilityConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var boolValue = (bool)value;
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToInVisibilityConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var boolValue = (bool)value;
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}                    
                     
                     
                     
                     
                     
                     
                     