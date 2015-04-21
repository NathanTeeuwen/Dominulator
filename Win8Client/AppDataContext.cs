using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using System.ComponentModel;

namespace Win8Client
{
    class AppDataContext
        : DependencyObject
    {
        System.Collections.ObjectModel.ObservableCollection<DominionCard> availableCards;
        private SortableCardList allCards;
        private SortableCardList shelterCards;
        private SortableCardList colonyPlatinumCards;
        private SortableCardList currentDeck;
        private SortableCardList baneCard;
        private SortableCardList eventCards;
        private SortableCardList commonCards;
        private System.Collections.ObjectModel.ObservableCollection<Expansion> expansions;

        /*
        public DependencyObjectDecl<bool, DefaultTrue> Use3OrMoreFromExpansions { get; private set; }
        public DependencyObjectDecl<bool, DefaultTrue> RequireTrashing { get; private set; }
        public DependencyObjectDecl<bool, DefaultTrue> RequirePlusCards { get; private set; }
        public DependencyObjectDecl<bool, DefaultTrue> RequirePlusBuy { get; private set; }
        public DependencyObjectDecl<bool, DefaultTrue> RequirePlus2Actions { get; private set; }
        public DependencyObjectDecl<bool, DefaultTrue> RequireAttack { get; private set; }
        public DependencyObjectDecl<bool, DefaultTrue> AllowAttack { get; private set; }*/

        public DependencyObjectDecl<bool, DefaultTrue> IsPlayer1StrategyChecked { get; private set; }
        public DependencyObjectDecl<bool, DefaultFalse> IsPlayer2StrategyChecked { get; private set; }

        public DependencyObjectDecl<StrategyDescription, DefaultEmptyStrategyDescription> currentStrategy { get; private set; }
        public StrategyDescription player1Strategy { get; private set; }
        public StrategyDescription player2Strategy { get; private set; }
        private bool isStrategy1Selected;

        public DependencyObjectDecl<CardVisibility, DefaultCurrent> CardVisibility{ get; private set; }
        public DependencyObjectDecl<SettingsButtonVisibility, DefaultSettingsButton> SettingsButtonVisibility { get; private set; }
        public DependencyObjectDecl<PageConfig, DefaultPageConfig> PageConfig { get; private set; }

        public DependencyObjectDecl<bool, DefaultFalse> StrategyResultsAvailable { get; private set; }
        public bool strategyReportDirty = false;
        public Dominion.Data.StrategyComparisonResults strategyComparisonResults;
        public DependencyObjectDecl<string, DefaultEmptyString> StrategyReport { get; private set; }
        public DependencyObjectDecl<string, DefaultEmptyString> Player1Name { get; private set; }
        public DependencyObjectDecl<string, DefaultEmptyString> Player2Name { get; private set; }        
        public DependencyObjectDecl<double, DefaultDoubleZero> Player1WinPercent { get; private set; }
        public DependencyObjectDecl<double, DefaultDoubleZero> Player2WinPercent { get; private set; }
        public DependencyObjectDecl<double, DefaultDoubleZero> TiePercent { get; private set; }

        public DependencyObjectDecl<bool, DefaultFalse> UseShelters { get; private set; }
        public DependencyObjectDecl<bool, DefaultFalse> UseColonyPlatinum { get; private set; }

        public DependencyObjectDecl<bool, DefaultFalse> IsBaneCardVisible { get; private set; }
        public DependencyObjectDecl<bool, DefaultFalse> AreEventCardsVisible { get; private set; }

        internal bool isCurrentDeckIgnoringAllDeckSelectionUpdates = false;

        private MainPage mainPage;

        public AppDataContext(MainPage mainPage)
        {
            this.mainPage = mainPage;

            this.allCards = new SortableCardList();
            this.colonyPlatinumCards = new SortableCardList();
            this.shelterCards = new SortableCardList();
            this.currentDeck = new SortableCardList();
            this.commonCards = new SortableCardList();
            this.baneCard = new SortableCardList();
            this.eventCards = new SortableCardList();
            this.availableCards = new System.Collections.ObjectModel.ObservableCollection<DominionCard>();
            this.expansions = new System.Collections.ObjectModel.ObservableCollection<Expansion>();
            /*
            this.Use3OrMoreFromExpansions = new DependencyObjectDecl<bool, DefaultTrue>(this);
            this.RequireTrashing = new DependencyObjectDecl<bool, DefaultTrue>(this);
            this.RequirePlusCards = new DependencyObjectDecl<bool, DefaultTrue>(this);
            this.RequirePlusBuy = new DependencyObjectDecl<bool, DefaultTrue>(this);
            this.RequirePlus2Actions = new DependencyObjectDecl<bool, DefaultTrue>(this);
            this.RequireAttack = new DependencyObjectDecl<bool, DefaultTrue>(this);
            this.AllowAttack = new DependencyObjectDecl<bool, DefaultTrue>(this);*/

            this.player1Strategy = new StrategyDescription();
            this.player2Strategy = new StrategyDescription();
            this.currentStrategy = new DependencyObjectDecl<StrategyDescription, DefaultEmptyStrategyDescription>(this);
            this.currentStrategy.Value = this.player1Strategy;
            this.IsPlayer1StrategyChecked = new DependencyObjectDecl<bool, DefaultTrue>(this);
            this.IsPlayer2StrategyChecked = new DependencyObjectDecl<bool, DefaultFalse>(this);

            this.CardVisibility = new DependencyObjectDecl<CardVisibility, DefaultCurrent>(this);
            this.SettingsButtonVisibility = new DependencyObjectDecl<SettingsButtonVisibility, DefaultSettingsButton>(this);
            this.PageConfig = new DependencyObjectDecl<PageConfig, DefaultPageConfig>(this);   
            this.IsBaneCardVisible = new DependencyObjectDecl<bool, DefaultFalse>(this);
            this.AreEventCardsVisible = new DependencyObjectDecl<bool, DefaultFalse>(this);

            this.StrategyResultsAvailable = new DependencyObjectDecl<bool, DefaultFalse>(this);
            this.StrategyReport = new DependencyObjectDecl<string, DefaultEmptyString>(this);
            this.Player1Name = new DependencyObjectDecl<string, DefaultEmptyString>(this);
            this.Player2Name = new DependencyObjectDecl<string, DefaultEmptyString>(this);            
            this.Player1WinPercent = new DependencyObjectDecl<double, DefaultDoubleZero>(this);
            this.Player2WinPercent = new DependencyObjectDecl<double, DefaultDoubleZero>(this);
            this.TiePercent = new DependencyObjectDecl<double, DefaultDoubleZero>(this);

            this.UseShelters = new DependencyObjectDecl<bool, DefaultFalse>(this);
            this.UseColonyPlatinum = new DependencyObjectDecl<bool, DefaultFalse>(this);

            this.expansions.Add(new Expansion("Adventures", ExpansionIndex.Adventures));
            this.expansions.Add(new Expansion("Alchemy", ExpansionIndex.Alchemy));
            this.expansions.Add(new Expansion("Base", ExpansionIndex.Base));
            this.expansions.Add(new Expansion("Cornucopia", ExpansionIndex.Cornucopia));
            this.expansions.Add(new Expansion("Dark Ages", ExpansionIndex.DarkAges));
            this.expansions.Add(new Expansion("Guilds", ExpansionIndex.Guilds));
            this.expansions.Add(new Expansion("Hinterlands", ExpansionIndex.Hinterlands));
            this.expansions.Add(new Expansion("Intrigue", ExpansionIndex.Intrigue));
            this.expansions.Add(new Expansion("Promo", ExpansionIndex.Promo));
            this.expansions.Add(new Expansion("Prosperity", ExpansionIndex.Prosperity));
            this.expansions.Add(new Expansion("Seaside", ExpansionIndex.Seaside));            

            foreach (var expansion in expansions)
            {
                expansion.IsEnabled.PropertyChanged += ExpansionEnabledChangedEventHandler;
            }

            this.commonCards.PropertyChanged += AvailableCards_PropetyChanged;
            this.currentDeck.PropertyChanged += AvailableCards_PropetyChanged;
            this.currentDeck.PropertyChanged += UpdateBaneCard_PropetyChanged;
            this.eventCards.PropertyChanged += UpdateEventCard_PropertyChanged;

            this.allCards.ApplyFilter(card => card.Expansion != ExpansionIndex._Unknown && this.expansions[(int)card.Expansion].IsEnabled.Value);
            this.currentDeck.ApplyFilter(card => card.Expansion != ExpansionIndex._Unknown && this.expansions[(int)card.Expansion].IsEnabled.Value);
            this.eventCards.ApplyFilter(card => card.Expansion != ExpansionIndex._Unknown && this.expansions[(int)card.Expansion].IsEnabled.Value);
        }              

        void AvailableCards_PropetyChanged(object sender, PropertyChangedEventArgs e)
        {
            var setCards = new HashSet<Dominion.Card>();
            foreach (var card in this.currentDeck.Cards)
            {                
                setCards.Add(card.dominionCard);
            }

            foreach (var card in this.commonCards.Cards)
            {
                setCards.Add(card.dominionCard);
            }

            this.player1Strategy.ConvertToDominionStrategy().GetAllCardsInStrategy(setCards);
            this.player2Strategy.ConvertToDominionStrategy().GetAllCardsInStrategy(setCards);
            
            var extraCards = new List<DominionCard>();

            foreach (var card in this.availableCards)
            {
                if (!setCards.Contains(card.dominionCard))
                    extraCards.Add(card);
            }

            foreach (var card in setCards)
            {
                var dominionCard = DominionCard.Create(card);
                if (!this.availableCards.Contains(dominionCard))
                    this.availableCards.Add(dominionCard);
            }
            
            foreach (var card in extraCards)
            {
                this.availableCards.Remove(card);
            }

            // this sort method will order but cause items in strategy to be data bound empty
            //Sort(this.availableCards, c => c.dominionCard.name);
        }

        void UpdateBaneCard_PropetyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateBaneCard();
        }

        void UpdateBaneCard()
        {
            this.IsBaneCardVisible.Value = this.currentDeck.CurrentCards.Where(c => c.dominionCard == Dominion.Cards.YoungWitch).Any();
        }

        void UpdateEventCard_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateEventCard();
        }

        void UpdateEventCard()
        {
            this.AreEventCardsVisible.Value = this.eventCards.CurrentCards.Any();
        }

        public static void Sort<T, T2>(System.Collections.ObjectModel.ObservableCollection<T> collection, System.Func<T, T2> func) 
            where T2 : System.IComparable
        {
            List<T> sorted = collection.OrderBy(x => func(x)).ToList();
            for (int i = 0; i < sorted.Count(); i++)
                collection.Move(collection.IndexOf(sorted[i]), i);
        }


        public SortableCardList AllCards
        {
            get
            {
                return this.allCards;
            }
        }

        public SortableCardList ShelterCards
        {
            get
            {
                return this.shelterCards;
            }
        }

        public SortableCardList ColonyPlatinumCards
        {
            get
            {
                return this.colonyPlatinumCards;
            }
        }

        public SortableCardList CurrentDeck
        {
            get
            {
                return this.currentDeck;
            }
        }

        public SortableCardList BaneCard
        {
            get
            {
                return this.baneCard;
            }
        }

        public SortableCardList EventCards
        {
            get
            {
                return this.eventCards;
            }
        }

        public System.Collections.ObjectModel.ObservableCollection<DominionCard> AvailableCards
        {
            get
            {
                return this.availableCards;
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
            this.isCurrentDeckIgnoringAllDeckSelectionUpdates = true;
            this.allCards.UpdateUIFromUIThread();
            this.currentDeck.UpdateUIFromUIThread();
            this.isCurrentDeckIgnoringAllDeckSelectionUpdates = false;
            this.mainPage.UpdateAllCardsListSelection();
        }

        public Dominion.GameConfig GetGameConfig()
        {

            Dominion.StartingCardSplit player1Split = this.player1Strategy.StartingCardSplit.Value;
            Dominion.StartingCardSplit player2Split = this.player2Strategy.StartingCardSplit.Value;
            Dominion.Card[] kingdomCards = this.currentDeck.Cards.Select(c => c.dominionCard).ToArray();
            DominionCard baneCard = this.BaneCard.CurrentCards.FirstOrDefault();            

            var builder = new Dominion.GameConfigBuilder();            
                
            builder.SetKingdomPiles(kingdomCards);
            if (baneCard != null)
                builder.SetBaneCard(baneCard.dominionCard);

            builder.useColonyAndPlatinum = this.UseColonyPlatinum.Value;
            builder.useShelters = this.UseShelters.Value;
            builder.SetCardSplitPerPlayer(new Dominion.StartingCardSplit[] { player1Split, player2Split });

            return builder.ToGameConfig();
        }

        public void ShowReport()
        {
            if (this.strategyReportDirty)
            {
                this.strategyReportDirty = false;
                var uiScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();
                System.Threading.Tasks.Task<string>.Factory.StartNew(() =>
                {
                    return HtmlRenderer.HtmlReportGenerator.GetHtmlReport(this.strategyComparisonResults);
                }).ContinueWith(async (continuation) =>
                {
                    this.StrategyReport.Value = continuation.Result;
                }, uiScheduler);
            }

            this.PageConfig.Value = Win8Client.PageConfig.StrategyReport;
        }

    }

    public enum CardVisibility
    {
        Current,
        All,
        Settings
    }

    public enum SettingsButtonVisibility
    {
        Settings,
        Back        
    }

    public enum PageConfig
    {
        StrategyReport,
        Design
    }

    class DefaultCurrent
        : DependencyPolicy<CardVisibility>
    {
        public CardVisibility DefaultValue
        {
            get
            {
                return CardVisibility.Current;
            }
        }
    }

    class DefaultSettingsButton
        : DependencyPolicy<SettingsButtonVisibility>
    {
        public SettingsButtonVisibility DefaultValue
        {
            get
            {
                return SettingsButtonVisibility.Settings;
            }
        }
    }

    class DefaultPageConfig
        : DependencyPolicy<PageConfig>
    {
        public PageConfig DefaultValue
        {
            get
            {
                return PageConfig.Design;
            }
        }
    }
}
