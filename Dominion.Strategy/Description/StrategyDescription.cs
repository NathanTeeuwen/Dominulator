using Dominion.Strategy;

namespace Dominion.Strategy.Description
{
    public class StrategyDescription
    {
        public readonly PickByPriorityDescription purchaseOrderDescription;

        public StrategyDescription(PickByPriorityDescription purchaseOrderDescription)
        {
            this.purchaseOrderDescription = purchaseOrderDescription;
        }

        public Dominion.Strategy.PlayerAction ToPlayerAction(string playerName)
        {
            return new Dominion.Strategy.PlayerAction(
                playerName,
                this.purchaseOrderDescription.ToCardPicker());
        }
    }
}