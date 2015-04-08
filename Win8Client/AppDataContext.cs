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
        private SortableCardList currentDeck;
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

        public DependencyObjectDecl<string, DefaultEmptyString> StrategyReport { get; private set; }

        internal bool isCurrentDeckIgnoringAllDeckSelectionUpdates = false;

        private MainPage mainPage;

        public AppDataContext(MainPage mainPage)
        {
            this.mainPage = mainPage;

            this.allCards = new SortableCardList();
            this.currentDeck = new SortableCardList();
            this.commonCards = new SortableCardList();
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
            this.StrategyReport = new DependencyObjectDecl<string, DefaultEmptyString>(this);

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

            foreach (var expansion in expansions)
            {
                expansion.IsEnabled.PropertyChanged += ExpansionEnabledChangedEventHandler;
            }

            this.commonCards.PropertyChanged += AvailableCards_PropetyChanged;
            this.currentDeck.PropertyChanged += AvailableCards_PropetyChanged;
            this.IsPlayer1StrategyChecked.PropertyChanged += Player1RadioButtonChecked;
            this.IsPlayer2StrategyChecked.PropertyChanged += Player2RadioButtonChecked;

            //this.Use3OrMoreFromExpansions.PropertyChanged += Enable3orMoreFromExpansionsChangedEventHandler;

            this.allCards.ApplyFilter(card => card.Expansion != ExpansionIndex._Unknown && this.expansions[(int)card.Expansion].IsEnabled.Value);
            this.currentDeck.ApplyFilter(card => card.Expansion != ExpansionIndex._Unknown && this.expansions[(int)card.Expansion].IsEnabled.Value);
        }

        private void Player1RadioButtonChecked(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsPlayer1StrategyChecked.Value)
            {
                this.CurrentStrategy.Value = this.player1Strategy;
            }
        }

        private void Player2RadioButtonChecked(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsPlayer2StrategyChecked.Value)
            {
                this.CurrentStrategy.Value = this.player2Strategy;
            }
        }

        void AvailableCards_PropetyChanged(object sender, PropertyChangedEventArgs e)
        {
            var listCards = new List<DominionCard>();
            foreach (var card in this.currentDeck.Cards)
            {
                listCards.Add(card);
            }

            foreach (var card in this.commonCards.Cards)
            {
                listCards.Add(card);
            }

            this.availableCards.Clear();
            foreach (var card in listCards.OrderBy(c => c.Name))
            {
                this.availableCards.Add(card);
            }
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

        /*
        public void Enable3orMoreFromExpansionsChangedEventHandler(object sender, PropertyChangedEventArgs e)
        {
            this.currentDeck.UpdateUI();
        }*/
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
