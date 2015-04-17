
namespace Win8Client
{
    class StrategyDescription
    {
        public System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription> CardAcceptanceDescriptions { get; private set; }
        public DependencyObjectDecl<bool, DefaultFalse> IsOpenRandomSplitChecked { get; private set; }
        public DependencyObjectDecl<bool, DefaultFalse> IsOpen2SplitChecked { get; private set; }
        public DependencyObjectDecl<bool, DefaultFalse> IsOpen3SplitChecked { get; private set; }
        public DependencyObjectDecl<bool, DefaultTrue> IsOpen4SplitChecked { get; private set; }
        public DependencyObjectDecl<bool, DefaultFalse> IsOpen5SplitChecked { get; private set; }

        public StrategyDescription()
        {
            this.CardAcceptanceDescriptions = new System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription>();
            this.IsOpenRandomSplitChecked = new DependencyObjectDecl<bool, DefaultFalse>(this);
            this.IsOpen2SplitChecked = new DependencyObjectDecl<bool, DefaultFalse>(this);
            this.IsOpen3SplitChecked = new DependencyObjectDecl<bool, DefaultFalse>(this);
            this.IsOpen4SplitChecked = new DependencyObjectDecl<bool, DefaultTrue>(this);
            this.IsOpen5SplitChecked = new DependencyObjectDecl<bool, DefaultFalse>(this);
        }

        public void PopulateFrom(Dominion.Strategy.Description.StrategyDescription descr)
        {
            this.CardAcceptanceDescriptions.Clear();
            foreach (Dominion.Strategy.Description.CardAcceptanceDescription cardAcceptanceDescription in descr.purchaseOrderDescription.descriptions)
            {
                this.CardAcceptanceDescriptions.Add(CardAcceptanceDescription.PopulateFrom(cardAcceptanceDescription));
            }
        }

        public Dominion.StartingCardSplit GetStartingCardSplit()
        {
            if (this.IsOpenRandomSplitChecked.Value)
                return Dominion.StartingCardSplit.Random;
            if (this.IsOpen2SplitChecked.Value)
                return Dominion.StartingCardSplit.Split25;
            if (this.IsOpen3SplitChecked.Value)
                return Dominion.StartingCardSplit.Split34;
            if (this.IsOpen4SplitChecked.Value)
                return Dominion.StartingCardSplit.Split43;
            if (this.IsOpen5SplitChecked.Value)
                return Dominion.StartingCardSplit.Split52;

            throw new System.Exception("One of the openning split boxes should have been checked!");
        }

        public Dominion.Strategy.Description.StrategyDescription ConvertToDominionStrategy()
        {
            return new Dominion.Strategy.Description.StrategyDescription(
                new Dominion.Strategy.Description.PickByPriorityDescription(ConvertPurchaseOrderToDominionStrategy()));
        }        

        private Dominion.Strategy.Description.CardAcceptanceDescription[] ConvertPurchaseOrderToDominionStrategy()
        {
            var result = new Dominion.Strategy.Description.CardAcceptanceDescription[this.CardAcceptanceDescriptions.Count];

            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = CardAcceptanceDescriptions[i].ConvertToDominionStrategy();
            }

            return result;
        }
    }
}