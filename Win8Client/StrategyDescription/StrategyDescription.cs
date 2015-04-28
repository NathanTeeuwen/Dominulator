
namespace Win8Client
{
    public class StrategyDescription
    {
        public System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription> PurchaseOrderDescriptions { get; private set; }
        public System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription> TrashOrderDescriptions { get; private set; }   
        public DependencyObjectDecl<Dominion.StartingCardSplit, DefaultSplit4> StartingCardSplit { get; private set; }
        
        public StrategyDescription()
        {
            this.PurchaseOrderDescriptions = new System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription>();
            this.TrashOrderDescriptions = new System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription>();
            this.StartingCardSplit = new DependencyObjectDecl<Dominion.StartingCardSplit, DefaultSplit4>(this);            
        }

        public void PopulateFrom(Dominion.Strategy.Description.StrategyDescription descr)
        {
            this.PurchaseOrderDescriptions.Clear();
            foreach (Dominion.Strategy.Description.CardAcceptanceDescription cardAcceptanceDescription in descr.purchaseOrderDescription.descriptions)
            {
                this.PurchaseOrderDescriptions.Add(CardAcceptanceDescription.PopulateFrom(cardAcceptanceDescription));
            }

            this.TrashOrderDescriptions.Clear();
            foreach (Dominion.Strategy.Description.CardAcceptanceDescription cardAcceptanceDescription in descr.trashOrderDescription.descriptions)
            {
                this.TrashOrderDescriptions.Add(CardAcceptanceDescription.PopulateFrom(cardAcceptanceDescription));
            }
        }        

        public Dominion.Strategy.Description.StrategyDescription ConvertToDominionStrategy()
        {
            return new Dominion.Strategy.Description.StrategyDescription(
                new Dominion.Strategy.Description.PickByPriorityDescription(ConvertPurchaseOrderToDominionStrategy(this.PurchaseOrderDescriptions)),
                new Dominion.Strategy.Description.PickByPriorityDescription(ConvertPurchaseOrderToDominionStrategy(this.TrashOrderDescriptions))
                );
        }        

        private static Dominion.Strategy.Description.CardAcceptanceDescription[] ConvertPurchaseOrderToDominionStrategy(System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription> descrs)
        {
            var result = new Dominion.Strategy.Description.CardAcceptanceDescription[descrs.Count];

            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = descrs[i].ConvertToDominionStrategy();
            }

            return result;
        }

        public bool CanSimulate()
        {                    
            foreach (var descr in this.PurchaseOrderDescriptions)
            {
                if (!descr.CanSimulateCard.Value)
                    return false;
            }
            return true;        
        }
    }
}