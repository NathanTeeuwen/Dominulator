
namespace Win8Client
{
    class StrategyDescription
    {
        public System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription> CardAcceptanceDescriptions { get; private set; }

        public StrategyDescription()
        {
            this.CardAcceptanceDescriptions = new System.Collections.ObjectModel.ObservableCollection<CardAcceptanceDescription>();            
        }
    }
}