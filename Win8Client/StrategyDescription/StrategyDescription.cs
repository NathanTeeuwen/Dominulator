using System;
using System.Linq;

namespace Win8Client
{
    public enum PriorityDescription
    {
        PurchaseOrder,
        TrashOrder
    }

    public class StrategyDescription
    {
        public System.Collections.ObjectModel.ObservableCollection<DominionCard> KingdomCards { get; private set; }
        public System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription> PurchaseOrderDescriptions { get; private set; }
        public System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription> TrashOrderDescriptions { get; private set; }   
        public DependencyObjectDecl<Dominion.StartingCardSplit, DefaultSplit4> StartingCardSplit { get; private set; }
        public DependencyObjectDecl<PriorityDescription, DefaultPriorityDescription> EditingDescription { get; private set; }
        public DependencyObjectDecl<System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription>, DefaultObservableCollection> CurrentDescription { get; private set; }
        
        public StrategyDescription()
        {
            this.KingdomCards = new System.Collections.ObjectModel.ObservableCollection<DominionCard>();
            this.PurchaseOrderDescriptions = new System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription>();
            this.TrashOrderDescriptions = new System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription>();
            this.StartingCardSplit = new DependencyObjectDecl<Dominion.StartingCardSplit, DefaultSplit4>(this);
            this.EditingDescription = new DependencyObjectDecl<PriorityDescription, DefaultPriorityDescription>(this);
            this.CurrentDescription = new DependencyObjectDecl<System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription>, DefaultObservableCollection>(this);

            this.EditingDescription.PropertyChanged += EditingDescription_PropertyChanged;
            this.PurchaseOrderDescriptions.CollectionChanged += PurchaseOrderDescriptions_CollectionChanged;

            SetCurrentDescription();
        }

        void PurchaseOrderDescriptions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.KingdomCards.Clear();
            foreach (var card in this.PurchaseOrderDescriptions.GroupBy(c => c.Card.Value).Select(group => group.Key).Where(c => c.dominionCard.isKingdomCard))
            {
                this.KingdomCards.Add(card);
            }
        }

        void EditingDescription_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SetCurrentDescription();
        }

        private void SetCurrentDescription()
        {
            switch(this.EditingDescription.Value)
            {
                case PriorityDescription.PurchaseOrder: this.CurrentDescription.Value = this.PurchaseOrderDescriptions; break;
                case PriorityDescription.TrashOrder: this.CurrentDescription.Value = this.TrashOrderDescriptions; break;
                default: throw new System.Exception();
            }
        }

        public void Clear()
        {
            this.PurchaseOrderDescriptions.Clear();
            this.TrashOrderDescriptions.Clear();
            this.EditingDescription.Value = PriorityDescription.PurchaseOrder;
        }

        public void PopulateFrom(Dominion.Strategy.Description.StrategyDescription descr)
        {
            this.Clear();
            
            foreach (Dominion.Strategy.Description.CardAcceptanceDescription cardAcceptanceDescription in descr.purchaseOrderDescription.descriptions)
            {
                this.PurchaseOrderDescriptions.Add(CardAcceptanceDescription.PopulateFrom(cardAcceptanceDescription));
            }

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