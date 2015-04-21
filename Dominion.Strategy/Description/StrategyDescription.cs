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

        public StrategyDescription(params CardAcceptanceDescription[] purchaseOrder)
        {
            this.purchaseOrderDescription = new PickByPriorityDescription(purchaseOrder);
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

        public static StrategyDescription GetDefaultStrategyDescription()
        {
            var result = new StrategyDescription(
                CardAcceptanceDescription.For(Cards.Province),
                CardAcceptanceDescription.For(Cards.Duchy, CountSource.CountOfPile, Cards.Province, Comparison.LessThan, 4),
                CardAcceptanceDescription.For(Cards.Estate, CountSource.CountOfPile, Cards.Province, Comparison.LessThan, 2),
                CardAcceptanceDescription.For(Cards.Gold),                
                CardAcceptanceDescription.For(Cards.Silver));
                        
            return result;
        }

        public void GetAllCardsInStrategy(System.Collections.Generic.HashSet<Card> cardSet)
        {
            Dominion.Strategy.PlayerAction playerAction = this.ToPlayerAction("GetAllCardsInStategy");
            PlayerAction.AddAllCards(cardSet, playerAction);
        }

        public override string ToString()
        {
            var writer = new System.IO.StringWriter();
            writer.Write("Purchase Order: ");
            this.purchaseOrderDescription.Write(writer);
            return writer.ToString();
        }
    }
}