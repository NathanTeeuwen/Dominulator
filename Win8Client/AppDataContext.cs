using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using System.ComponentModel;

namespace Win8Client
{
    public class AppDataContext
        : DependencyObject
    {
        System.Collections.ObjectModel.ObservableCollection<DominionCard> availableCards;
        public System.Collections.ObjectModel.ObservableCollection<DominionCard> selectedCards;
        private SortableCardList allCards;
        private SortableCardList shelterCards;
        private SortableCardList colonyPlatinumCards;
        private SortableCardList currentDeck;
        private SortableCardList baneCard;
        private SortableCardList eventCards;
        private SortableCardList commonCards;
        private System.Collections.ObjectModel.ObservableCollection<Expansion> expansions;
        
        public DependencyObjectDeclWithSettings<bool, DefaultTrue> Use3OrMoreFromExpansions { get; private set; }
        public DependencyObjectDeclWithSettings<bool, DefaultFalse> RequireTrashing { get; private set; }
        public DependencyObjectDeclWithSettings<bool, DefaultFalse> RequirePlusCards { get; private set; }
        public DependencyObjectDeclWithSettings<bool, DefaultFalse> RequirePlusBuy { get; private set; }
        public DependencyObjectDeclWithSettings<bool, DefaultFalse> RequirePlus2Actions { get; private set; }
        public DependencyObjectDeclWithSettings<bool, DefaultFalse> RequireAttack { get; private set; }
        public DependencyObjectDeclWithSettings<bool, DefaultTrue> AllowAttack { get; private set; }

        public DependencyObjectDecl<bool, DefaultTrue> IsPlayer1StrategyChecked { get; private set; }
        public DependencyObjectDecl<bool, DefaultFalse> IsPlayer2StrategyChecked { get; private set; }

        public DependencyObjectDecl<StrategyDescription, DefaultEmptyStrategyDescription> currentStrategy { get; private set; }
        public StrategyDescription player1Strategy { get; private set; }
        public StrategyDescription player2Strategy { get; private set; }
        private bool isStrategy1Selected;

        public DependencyObjectDecl<PageConfig, DefaultCurrent> CurrentPageConfig{ get; private set; }
        public DependencyObjectDecl<SettingsButtonVisibility, DefaultSettingsButton> SettingsButtonVisibility { get; private set; }
        public DependencyObjectDecl<SimulationStep, DefaultSimulationStep> NextSimulationStep { get; private set; }        
        public DependencyObjectDeclWithSettings<bool, DefaultFalse> UseSideBySideStrategy { get; private set; }
        public DependencyObjectDecl<bool, DefaultFalse> SideBySideVisibility { get; private set; }
        public DependencyObjectDecl<bool, DefaultFalse> IsSelectionPresentOnCurrentDeck { get; private set; }
        public DependencyObjectDecl<bool, DefaultFalse> IsAddToPlayer1ButtonVisible { get; private set; }
        public DependencyObjectDecl<bool, DefaultFalse> IsAddToPlayer2ButtonVisible { get; private set; }
        public DependencyObjectDecl<bool, DefaultFalse> HintSelectedCardNotSimulatedButtonVisible { get; private set; }

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
            this.selectedCards = new System.Collections.ObjectModel.ObservableCollection<DominionCard>();
            this.expansions = new System.Collections.ObjectModel.ObservableCollection<Expansion>();


            this.Use3OrMoreFromExpansions = new DependencyObjectDeclWithSettings<bool, DefaultTrue>(this, "Use3OrMoreFromExpansions");
            this.RequireTrashing = new DependencyObjectDeclWithSettings<bool, DefaultFalse>(this, "Require Trashing");
            this.RequirePlusCards = new DependencyObjectDeclWithSettings<bool, DefaultFalse>(this, "Require Plus Card");
            this.RequirePlusBuy = new DependencyObjectDeclWithSettings<bool, DefaultFalse>(this, "Require Plus Buy");
            this.RequirePlus2Actions = new DependencyObjectDeclWithSettings<bool, DefaultFalse>(this, "Require Plus 2 Actions");
            this.RequireAttack = new DependencyObjectDeclWithSettings<bool, DefaultFalse>(this, "Require Attack");
            this.AllowAttack = new DependencyObjectDeclWithSettings<bool, DefaultTrue>(this, "Allow Attack");

            this.player1Strategy = new StrategyDescription();
            this.player2Strategy = new StrategyDescription();
            this.currentStrategy = new DependencyObjectDecl<StrategyDescription, DefaultEmptyStrategyDescription>(this);
            this.currentStrategy.Value = this.player1Strategy;
            this.IsPlayer1StrategyChecked = new DependencyObjectDecl<bool, DefaultTrue>(this);
            this.IsPlayer2StrategyChecked = new DependencyObjectDecl<bool, DefaultFalse>(this);

            this.CurrentPageConfig = new DependencyObjectDecl<PageConfig, DefaultCurrent>(this);
            this.SettingsButtonVisibility = new DependencyObjectDecl<SettingsButtonVisibility, DefaultSettingsButton>(this);            
            this.UseSideBySideStrategy = new DependencyObjectDeclWithSettings<bool, DefaultFalse>(this, "View Strategy Side By Side");
            this.SideBySideVisibility = new DependencyObjectDecl<bool, DefaultFalse>(this);
            this.IsSelectionPresentOnCurrentDeck = new DependencyObjectDecl<bool, DefaultFalse>(this);
            this.NextSimulationStep = new DependencyObjectDecl<SimulationStep, DefaultSimulationStep>(this);
            this.IsAddToPlayer1ButtonVisible = new DependencyObjectDecl<bool, DefaultFalse>(this);
            this.IsAddToPlayer2ButtonVisible = new DependencyObjectDecl<bool, DefaultFalse>(this);
            this.HintSelectedCardNotSimulatedButtonVisible = new DependencyObjectDecl<bool, DefaultFalse>(this);

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

            this.CurrentPageConfig.PropertyChanged += CalculateSideBySideViewVisibility;
            this.UseSideBySideStrategy.PropertyChanged += CalculateSideBySideViewVisibility;
            this.IsSelectionPresentOnCurrentDeck.PropertyChanged += UpdateSimulationStepEvent;
            this.NextSimulationStep.PropertyChanged += UpdatePlayerButtonVisibilitiesEvent;            
            this.NextSimulationStep.PropertyChanged += CalculateSideBySideViewVisibility;            

            this.CalculateSideBySideViewVisibility(this, null);

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

        public void UpdatePlayerButtonVisibilities()
        {
            if (!this.IsSelectionPresentOnCurrentDeck.Value)
            {
                this.IsAddToPlayer1ButtonVisible.Value = false;
                this.IsAddToPlayer2ButtonVisible.Value = false;
                return;
            }

            if (this.NextSimulationStep.Value == SimulationStep.AddToPlayer1 ||
                this.NextSimulationStep.Value == SimulationStep.ReviewAndSimulate && CanSimulateCardsInSelection())
            {
                this.IsAddToPlayer1ButtonVisible.Value = true;
            }
            else
                this.IsAddToPlayer1ButtonVisible.Value = false;

            if (this.NextSimulationStep.Value == SimulationStep.AddToPlayer2 ||
                this.NextSimulationStep.Value == SimulationStep.ReviewAndSimulate && CanSimulateCardsInSelection())
            {
                this.IsAddToPlayer2ButtonVisible.Value = true;
            }
            else
                this.IsAddToPlayer2ButtonVisible.Value = false;
        }

        void UpdatePlayerButtonVisibilitiesEvent(object sender, PropertyChangedEventArgs e)
        {
            UpdatePlayerButtonVisibilities();
        }

        void CalculateSideBySideViewVisibility(object sender, PropertyChangedEventArgs e)
        {
            if (this.CurrentPageConfig.Value == Win8Client.PageConfig.AllCards || this.CurrentPageConfig.Value == Win8Client.PageConfig.CurrentDeck)
            {
                if (this.UseSideBySideStrategy.Value)
                    this.SideBySideVisibility.Value = true;
                else if (this.player1Strategy.PurchaseOrderDescriptions.Any() || this.player2Strategy.PurchaseOrderDescriptions.Any())
                    this.SideBySideVisibility.Value = true;
                else
                    this.SideBySideVisibility.Value = false;
            }
            else
            {
                this.SideBySideVisibility.Value = false;
            }                
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

            this.CurrentPageConfig.Value = Win8Client.PageConfig.Report;
        }

        private void GetStrategyNames(Dominion.Strategy.Description.StrategyDescription player1Descr, Dominion.Strategy.Description.StrategyDescription player2Descr, out string player1Name, out string player2Name)
        {
            var player1Uniques = new HashSet<Dominion.Card>();
            var player2Uniques = new HashSet<Dominion.Card>();

            foreach (Dominion.Card card in player1Descr.purchaseOrderDescription.descriptions.Select(descr => descr.card))
            {
                player1Uniques.Add(card);
            }

            foreach (Dominion.Card card in player2Descr.purchaseOrderDescription.descriptions.Select(descr => descr.card))
            {
                player2Uniques.Add(card);
                player1Uniques.Remove(card);
            }

            foreach (Dominion.Card card in player1Descr.purchaseOrderDescription.descriptions.Select(descr => descr.card))
            {
                player2Uniques.Remove(card);
            }


            player1Name = string.Join(", ", player1Uniques.Select(c => c.name));
            player2Name = string.Join(", ", player2Uniques.Select(c => c.name));

            return;
        }    

        private bool CanSimulateStrategies()
        {
            return this.player1Strategy.CanSimulate() && this.player2Strategy.CanSimulate();
        }

        public void SimulateGameButtonClick()
        {
            if (!CanSimulateStrategies())
                return;

            this.StrategyResultsAvailable.Value = false;

            var uiScheduler = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();

            Dominion.Strategy.Description.StrategyDescription player1Descr = this.player1Strategy.ConvertToDominionStrategy();
            Dominion.Strategy.Description.StrategyDescription player2Descr = this.player2Strategy.ConvertToDominionStrategy();

            Dominion.GameConfig gameConfig = this.GetGameConfig();

            System.Diagnostics.Debug.WriteLine("Player 1: ");
            System.Diagnostics.Debug.WriteLine(player1Descr.ToString());
            System.Diagnostics.Debug.WriteLine("Player 2: ");
            System.Diagnostics.Debug.WriteLine(player2Descr.ToString());

            System.Threading.Tasks.Task<StrategyUIResults>.Factory.StartNew(() =>
            {

                string player1nameAppend, player2nameAppend;
                GetStrategyNames(player1Descr, player2Descr, out player1nameAppend, out player2nameAppend);

                string player1Name = "Player 1 " + (!string.IsNullOrEmpty(player1nameAppend) ? "(" + player1nameAppend + ")" : "");
                string player2Name = "Player 2 " + (!string.IsNullOrEmpty(player2nameAppend) ? "(" + player2nameAppend + ")" : "");

                var playerActions = new Dominion.Strategy.PlayerAction[] 
                {
                    player1Descr.ToPlayerAction(player1Name),
                    player2Descr.ToPlayerAction(player2Name)
                };

                bool rotateWhoStartsFirst = true;
                int numberOfGames = 1000;

                var strategyComparison = new Dominion.Data.StrategyComparison(playerActions, gameConfig, rotateWhoStartsFirst, numberOfGames);

                Dominion.Data.StrategyComparisonResults strategyComparisonResults = strategyComparison.ComparePlayers(randomSeed: MainPage.random.Next());
                return new StrategyUIResults()
                {
                    strategyComparisonResults = strategyComparisonResults,
                    Player1Name = player1Name,
                    Player2Name = player2Name,
                    Player1WinPercent = strategyComparisonResults.PlayerWinPercent(0),
                    Player2WinPercent = strategyComparisonResults.PlayerWinPercent(1),
                    TiePercent = strategyComparisonResults.TiePercent,
                };
            }).ContinueWith(async (continuation) =>
            {
                var results = (StrategyUIResults)continuation.Result;

                this.strategyReportDirty = true;
                this.strategyComparisonResults = results.strategyComparisonResults;
                this.Player1Name.Value = results.Player1Name;
                this.Player2Name.Value = results.Player2Name;
                this.Player1WinPercent.Value = results.Player1WinPercent;
                this.Player2WinPercent.Value = results.Player2WinPercent;
                this.TiePercent.Value = results.TiePercent;
                this.StrategyResultsAvailable.Value = true;
            }, uiScheduler);
        }

        private class StrategyUIResults
        {
            public Dominion.Data.StrategyComparisonResults strategyComparisonResults;
            public string Player1Name;
            public string Player2Name;
            public double Player1WinPercent;
            public double Player2WinPercent;
            public double TiePercent;
        }

        public bool CanSimulateCardsInSelection()
        {
            foreach(var card in this.selectedCards)
            {
                if (!card.CanSimulate)
                    return false;
            }
            return true;
        }

        public void UpdateSimulationStep()
        {
            this.NextSimulationStep.Value = EvaluateNextSimulationStep();
        }

        void UpdateSimulationStepEvent(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateSimulationStep();
        }

        public SimulationStep EvaluateNextSimulationStep()
        {
            bool hasStrategy1 = this.player1Strategy.PurchaseOrderDescriptions.Any();
            bool hasStrategy2 = this.player2Strategy.PurchaseOrderDescriptions.Any();
            bool hasSelection = this.IsSelectionPresentOnCurrentDeck.Value;

            if (!hasStrategy1 && !hasStrategy2 && !hasSelection)
                return SimulationStep.MakeSelection;

            if (!hasStrategy1)
            {
                if (!hasSelection)
                    return SimulationStep.MakeSelectionForPlayer1;

                if (!CanSimulateCardsInSelection())
                    return SimulationStep.SelectedCardCanNotBeSimulated;

                return SimulationStep.AddToPlayer1;
            }
                
            if (!this.player1Strategy.CanSimulate())
                return SimulationStep.Player1CanNotBeSimulated;

            if (!hasStrategy2)
            {
                if (!hasSelection)
                    return SimulationStep.MakeSelectionForPlayer2;

                if (!CanSimulateCardsInSelection())
                    return SimulationStep.SelectedCardCanNotBeSimulated;

                return SimulationStep.AddToPlayer2;
            }

            if (!this.player2Strategy.CanSimulate())
                return SimulationStep.Player2CanNotBeSimulated;

            return SimulationStep.ReviewAndSimulate;
        }
    }

    public enum SimulationStep
    {        
        MakeSelection,
        MakeSelectionForPlayer1,
        AddToPlayer1,
        Player1CanNotBeSimulated,
        SelectedCardCanNotBeSimulated,
        MakeSelectionForPlayer2,
        AddToPlayer2,
        Player2CanNotBeSimulated,
        ReviewAndSimulate
    }

    public enum PageConfig
    {
        CurrentDeck,
        AllCards,
        Strategy,
        Settings,
        Report
    }

    public enum SettingsButtonVisibility
    {
        Settings,
        Back        
    }        
}
