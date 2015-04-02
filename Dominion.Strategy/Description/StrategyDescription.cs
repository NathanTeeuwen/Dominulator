using Dominion.Strategy;

namespace Dominion.Strategy.Description
{
    public class StrategyDescription
    {
        public readonly PickByPriorityDescription purchaseOrderDescription;

        public StrategyDescription()
        {
            this.purchaseOrderDescription = new PickByPriorityDescription(new CardAcceptanceDescription[0]);
        }

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

        public StrategyDescription AddCardToPurchaseOrder(Card card)
        {
            return new StrategyDescription(this.purchaseOrderDescription.AddCardInBestLocation(card));
        }

        public static StrategyDescription GetDefaultStrategyDescription(Card card)
        {
            StrategyDescription result = new StrategyDescription();
            result = result.AddCardToPurchaseOrder(Cards.Province);
            result = result.AddCardToPurchaseOrder(Cards.Gold);
            result = result.AddCardToPurchaseOrder(Cards.Duchy);
            result = result.AddCardToPurchaseOrder(Cards.Silver);
            result = result.AddCardToPurchaseOrder(Cards.Estate);
            result = result.AddCardToPurchaseOrder(card);
            return result;
        }
    }
}