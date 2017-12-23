using Dominion.Strategy;
using System.Collections.Generic;

namespace Dominion.Strategy.Description
{
    public class StrategyDescription
    {
        public readonly PickByPriorityDescription purchaseOrderDescription;
        public readonly PickByPriorityDescription trashOrderDescription;

        public bool IsEmptyPurchaseOrder
        {
            get
            {
                return this.purchaseOrderDescription.descriptions.Length == 0;
            }
            
        }

        public StrategyDescription(
            PickByPriorityDescription purchaseOrderDescription,
            PickByPriorityDescription trashOrderDescription
            )
        {
            this.purchaseOrderDescription = purchaseOrderDescription;
            this.trashOrderDescription = trashOrderDescription;
        }

        public Dominion.Strategy.PlayerAction ToPlayerAction(string playerName)
        {
            return new Dominion.Strategy.PlayerAction(
                playerName,
                this.purchaseOrderDescription.ToCardPicker());
        }

        public StrategyDescription AddCardToPurchaseOrder(Card card, int count)
        {
            return new StrategyDescription(
                this.purchaseOrderDescription.AddCardInBestLocation(card, count),
                this.trashOrderDescription);
        }

        public StrategyDescription AddCardsToPurchaseOrder(Card[] cards)
        {
            var result = this;

            foreach (var card in cards)
            {               
                result = result.AddCardToPurchaseOrder(card, GetDefaultCountForCard(card));
            }

            return result;
        }

        private const int CountAsManyAsPossible = 11;

        private int GetDefaultCountForCard(Card card)
        {
            if (MapCardToDefaultCount.ContainsKey(card))
                return MapCardToDefaultCount[card];

            if (card.isTreasure)
            {
                return card == Cards.Potion ? 1 : CountAsManyAsPossible;
            }

            if (card.isVictory)
            {
                return CountAsManyAsPossible;
            }

            if (card.isAction && card.plusAction == 0)
                return 1;

            if (card.isAction && card.plusAction > 0)
                return CountAsManyAsPossible;

            return 1;
        }

        static private Dictionary<Card, int> MapCardToDefaultCount = new Dictionary<Card, int>()
        {
            { Cards.Loan, 1},
            { Cards.Upgrade, 1},
            { Cards.Warehouse, 1},
            { Cards.Dungeon, 1},
            { Cards.Stables, CountAsManyAsPossible},
            { Cards.Cellar, 1},
            { Cards.Contraband, 1},
            { Cards.CounterFeit, 2},
            { Cards.Forager, 2},
            { Cards.Rats, 1},
            { Cards.Raze, 1},
            { Cards.HornOfPlenty, 1},
            { Cards.Cultist, CountAsManyAsPossible},
            { Cards.Storyteller, 2}
        };

        public static StrategyDescription GetDefaultDescription(GameConfig gameConfig)
        {
            var result = new StrategyDescription(
                GetDefaultPurchaseOrder(gameConfig),
                GetDefaultTrashDescription(gameConfig));
                        
            return result;
        }

        public static PickByPriorityDescription GetDefaultPurchaseOrder(GameConfig gameConfig)
        {
            return new PickByPriorityDescription(
                CardAcceptanceDescription.For(Cards.Province, CountSource.CountAllOwned, Cards.Gold, Comparison.GreaterThanEqual, 2),
                CardAcceptanceDescription.For(Cards.Duchy, CountSource.CountOfPile, Cards.Province, Comparison.LessThanEqual, 4),
                CardAcceptanceDescription.For(Cards.Estate, CountSource.CountOfPile, Cards.Province, Comparison.LessThanEqual, 2),
                CardAcceptanceDescription.For(Cards.Gold),
                CardAcceptanceDescription.For(Cards.Silver));
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