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
        public static Random random = new System.Random();

        AppDataContext appDataContext;

        public MainPage()
        {
            this.appDataContext = new AppDataContext(this);  

            this.InitializeComponent();
                  
            this.DataContext = this.appDataContext;            

            this.CurrentCards.CurrentCardsChanged += this.AllCards.UpdateAllCardsListSelection;
            this.appDataContext.CurrentDeck.PropertyChanged += UpdateCommonCardsFromKingdom;
            this.appDataContext.StrategyReport.PropertyChanged += StrategyReport_PropertyChanged;
            this.appDataContext.PageConfig.PropertyChanged += PageConfig_PropertyChanged;
            this.appDataContext.UseShelters.PropertyChanged += UpdateCommonCardsFromKingdom;
            this.appDataContext.UseColonyPlatinum.PropertyChanged += UpdateCommonCardsFromKingdom;

            this.appDataContext.CurrentDeck.SortByCost();
            this.appDataContext.AllCards.SortByName();

            this.Loaded += MainPage_Loaded;
        }

        void PageConfig_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.appDataContext.PageConfig.Value == PageConfig.StrategyReport)
            {
                this.appDataContext.SettingsButtonVisibility.Value = SettingsButtonVisibility.Back;
            }
        }
        
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var uiScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();

            appDataContext.ShelterCards.PopulateShelters();
            appDataContext.ColonyPlatinumCards.PopulateColonyPlatinum();

            appDataContext.AllCards.Populate().ContinueWith(
                delegate(System.Threading.Tasks.Task task)
                {
                    this.CurrentCards.Randomize10Cards();
                }, uiScheduler);
        }

        public void UpdateAllCardsListSelection()
        {
            this.AllCards.UpdateAllCardsListSelection();
        }
               
        internal void UpdateCommonCardsFromKingdom(object sender, PropertyChangedEventArgs e)
        {
            Dominion.GameConfig gameConfig = this.appDataContext.GetGameConfig();
            this.appDataContext.CommonCards.PopulateCommon(gameConfig);
        }              
      
        internal static void Generate10Random(
            ref bool useShelter,
            ref bool useColony,
            ref DominionCard baneCard,
            IList<DominionCard> resultList, 
            IList<DominionCard> sourceList, 
            IList<DominionCard> allCards, 
            IList<DominionCard> itemsToReplace)
        {
            bool isReplacingItems = itemsToReplace != null && itemsToReplace.Count > 0 && sourceList.Count <= 10;
            bool isReducingItems = itemsToReplace != null && itemsToReplace.Count > 0 && sourceList.Count > 10;
            var cardPicker = new UniqueCardPicker(allCards);

            bool isCleanRoll = false;

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
                isCleanRoll = true;
            }
            
            while (resultList.Count < 10)
            {
                DominionCard currentCard = cardPicker.GetCard(c => true);
                if (currentCard == null)
                    break;
                resultList.Add(currentCard);
            }

            if (isCleanRoll)
            {
                // reroll shelter
                {
                    int cProsperity = resultList.Select(c => c.dominionCard).Where(c => c.expansion == Dominion.Expansion.Prosperity).Count();
                    int roll = MainPage.random.Next(1, 10);
                    useColony = cProsperity >= roll ? true : false;
                }

                // reroll shelter
                {
                    int cDarkAges = resultList.Select(c => c.dominionCard).Where(c => c.expansion == Dominion.Expansion.DarkAges).Count();
                    int roll = MainPage.random.Next(1, 10);
                    useShelter = cDarkAges >= roll ? true : false;
                }
            }

            baneCard = cardPicker.GetCard(c => c.dominionCard.DefaultCoinCost == 2 || c.dominionCard.DefaultCoinCost == 3);            
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            this.appDataContext.CardVisibility.Value = CardVisibility.Settings;
            this.appDataContext.SettingsButtonVisibility.Value = SettingsButtonVisibility.Back;
        }

        private void SettingsBackButton_Click(object sender, RoutedEventArgs e)
        {
            this.appDataContext.CardVisibility.Value = CardVisibility.Current;
            this.appDataContext.SettingsButtonVisibility.Value = SettingsButtonVisibility.Settings;
            this.appDataContext.PageConfig.Value = PageConfig.Design;
        }

        private void AllCardsButton_Click(object sender, RoutedEventArgs e)
        {            
            this.appDataContext.CardVisibility.Value =
                this.appDataContext.CardVisibility.Value == CardVisibility.All ? CardVisibility.Current: CardVisibility.All;
        }        

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.appDataContext.strategyReportDirty)
            {
                this.appDataContext.strategyReportDirty = false;
                var uiScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();
                System.Threading.Tasks.Task<string>.Factory.StartNew(() =>
                {
                    return HtmlRenderer.HtmlReportGenerator.GetHtmlReport(this.appDataContext.strategyComparisonResults);
                }).ContinueWith(async (continuation) =>
                {
                    this.appDataContext.StrategyReport.Value = continuation.Result;
                }, uiScheduler);
            }

            this.appDataContext.PageConfig.Value = PageConfig.StrategyReport;            
        }

        void StrategyReport_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.ResultsWebView.NavigateToString(this.appDataContext.StrategyReport.Value);            
        }

    }

    class SortableCardList
        : DependencyObject
    {        
        System.Collections.Generic.List<DominionCard> originalCards;
        private System.Collections.ObjectModel.ObservableCollection<DominionCard> cards;
        private Func<DominionCard, bool> filter;
        private Func<DominionCard, bool> filter2;
        private Func<DominionCard, object> keySelector;
        
        public DependencyObjectDecl<string, DefaultEmptyString> CurrentSort { get; private set;}

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
        }

        public System.Threading.Tasks.Task Populate()
        {
            return PopulateFromResources().ContinueWith(async (continuation) =>
            {
                await this.UpdateUI();
            });
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
            this.originalCards.Add(card);
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

        public void Generate10Random(ref bool useShelter, ref bool useColony, ref DominionCard baneCard, IList<DominionCard> allCards, IList<DominionCard> itemsToReplace)
        {            
            MainPage.Generate10Random(
                ref useShelter, 
                ref useColony, 
                ref baneCard,
                this.originalCards, this.Cards, allCards, itemsToReplace);            

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
   
    enum ExpansionIndex
    {
        Adventures = 0,
        Alchemy = 1,
        Base = 2,
        Cornucopia = 3,
        DarkAges = 4,
        Guilds = 5,
        Hinterlands = 6,
        Intrigue = 7,
        Promo = 8,
        Prosperity = 9,
        Seaside = 10,        
        _Unknown = 11,
        _Count = 12
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

    public class CurrentCardVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (CardVisibility)value;
            return cardVisibility == CardVisibility.Current ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AllCardVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (CardVisibility)value;
            return cardVisibility == CardVisibility.All ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SettingsVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (CardVisibility)value;
            return cardVisibility == CardVisibility.Settings ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SettingsButtonVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (SettingsButtonVisibility)value;
            return cardVisibility == SettingsButtonVisibility.Settings ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SettingsBackButtonVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardVisibility = (SettingsButtonVisibility)value;
            return cardVisibility == SettingsButtonVisibility.Back ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PageConfigDesignVisibilityConverter
      : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var localValue = (PageConfig)value;
            return localValue == PageConfig.Design? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PageConfigStrategyReportVisibilityConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var localValue = (PageConfig)value;
            return localValue == PageConfig.StrategyReport ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((Dominion.StartingCardSplit)value).HasFlag((Dominion.StartingCardSplit)int.Parse((string)parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}                    
                     
                     
                     
                     
                     
                     
                     