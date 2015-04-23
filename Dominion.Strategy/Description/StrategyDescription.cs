using Dominion.Strategy;
using System.Collections.Generic;

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

        public StrategyDescription AddCardsToPurchaseOrder(Card[] cards)
        {
            var result = this;

            foreach (var card in cards)
            {
                if (result.purchaseOrderDescription.descriptions.Length == 0)
                {
                    result = Dominion.Strategy.Description.StrategyDescription.GetDefaultPurchaseDescription();
                }

                result = result.AddCardToPurchaseOrder(card);
            }

            return result;
        }

        public static StrategyDescription GetDefaultPurchaseDescription()
        {
            var result = new StrategyDescription(
                CardAcceptanceDescription.For(Cards.Province, CountSource.CountAllOwned, Cards.Gold, Comparison.GreaterThanEqual, 2),
                CardAcceptanceDescription.For(Cards.Duchy, CountSource.CountOfPile, Cards.Province, Comparison.LessThanEqual, 4),
                CardAcceptanceDescription.For(Cards.Estate, CountSource.CountOfPile, Cards.Province, Comparison.LessThanEqual, 2),
                CardAcceptanceDescription.For(Cards.Gold),                
                CardAcceptanceDescription.For(Cards.Silver));
                        
            return result;
        }

        public static PickByPriorityDescription GetDefaultTrashDescription(GameConfig gameConfig)
        {
            var result = new List<CardAcceptanceDescription>();
            result.Add(CardAcceptanceDescription.For(Cards.Curse));
            if (gameConfig.NeedsRuins)
            {
                result.Add(CardAcceptanceDescription.For(Cards.RuinedVillage));
                result.Add(CardAcceptanceDescription.For(Cards.RuinedMarket));
                result.Add(CardAcceptanceDescription.For(Cards.Survivors));
                result.Add(CardAcceptanceDescription.For(Cards.RuinedVillage));
                result.Add(CardAcceptanceDescription.For(Cards.AbandonedMine));
            }

            result.Add(CardAcceptanceDescription.For(Cards.Estate, CountSource.CountOfPile, Cards.Province, Comparison.GreaterThan, 2));

            if (gameConfig.useShelters)
            {
                result.Add(CardAcceptanceDescription.For(Cards.Hovel));
                result.Add(CardAcceptanceDescription.For(Cards.OvergrownEstate));
            }
            
            result.Add(CardAcceptanceDescription.For(Cards.Copper));
            
            return new PickByPriorityDescription(result.ToArray());
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