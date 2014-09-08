using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program.GeneticAlgorithm;
using Dominion.Strategy.Description;

namespace Program
{
    class SimpleStrategyDescription
        : Parameters
    {
        Card[] cards;

        public SimpleStrategyDescription(Card[] cards)
            : base(GetParametersForCards(cards))
        {
            this.cards = cards;
        }

        private static Parameter[] GetParametersForCards(Card[] cards)
        {
            var builder = new ParameterBuilderAndRetreiver();
            BuildPurchaseOrder(builder, cards);
            return builder.Result; 
        }
        
        private static void BuildPurchaseOrder(ParameterBuilderAndRetreiver builder, Card[] cards)
        {
            var purchaseAcceptances = new List<CardAcceptanceDescription>();
            
            // do the end game
            purchaseAcceptances.Add(new CardAcceptanceDescription(Cards.Province, CountSource.CountAllOwned, Cards.Gold, Comparison.GreaterThan, builder.Next(defaultValue:2, defaultLowerBound:0)));
            purchaseAcceptances.Add(new CardAcceptanceDescription(Cards.Duchy, CountSource.CountOfPile, Cards.Province, Comparison.LessThanEqual, builder.Next(defaultValue: 3, defaultLowerBound: 0)));
            purchaseAcceptances.Add(new CardAcceptanceDescription(Cards.Estate, CountSource.CountOfPile, Cards.Province, Comparison.LessThanEqual, builder.Next(defaultValue: 1, defaultLowerBound: 0)));
            
            // add cards costing more than gold
            foreach (Card card in cards.Where(c => c.DefaultCoinCost >= Cards.Gold.DefaultCoinCost).OrderBy(c => c.DefaultCoinCost))
            {
                purchaseAcceptances.Add(new CardAcceptanceDescription(card, CountSource.CountAllOwned, card, Comparison.LessThanEqual, builder.Next(defaultValue: 0, defaultLowerBound: 0)));
            }
            
            purchaseAcceptances.Add(new CardAcceptanceDescription(Cards.Gold));                
            
            // add cards costing more than potion and less than gold
            foreach (Card card in cards.Where(c => c.DefaultCoinCost >= Cards.Potion.DefaultCoinCost && c.DefaultCoinCost < Cards.Gold.DefaultCoinCost).OrderBy(c => c.DefaultCoinCost))
            {
                purchaseAcceptances.Add(new CardAcceptanceDescription(card, CountSource.CountAllOwned, card, Comparison.LessThanEqual, builder.Next(defaultValue: 0, defaultLowerBound: 0)));                
            }
            
            if (cards.Where(card => card.potionCost != 0).Any())
            {
                purchaseAcceptances.Add(new CardAcceptanceDescription(Cards.Potion, CountSource.CountAllOwned, Cards.Potion, Comparison.LessThanEqual, builder.Next(defaultValue: 1, defaultLowerBound: 0)));                                
            }

            // add cards costing mor than silver
            foreach (Card card in cards.Where(c => c.DefaultCoinCost >= Cards.Silver.DefaultCoinCost && c.DefaultCoinCost < Cards.Potion.DefaultCoinCost).OrderBy(c => c.DefaultCoinCost))
            {
                purchaseAcceptances.Add(new CardAcceptanceDescription(card, CountSource.CountAllOwned, card, Comparison.LessThanEqual, builder.Next(defaultValue: 0, defaultLowerBound: 0)));                
            }

            purchaseAcceptances.Add(new CardAcceptanceDescription(Cards.Silver));                
            
            // cards costing less than silver
            foreach (Card card in cards.Where(c => c.DefaultCoinCost < Cards.Silver.DefaultCoinCost).OrderBy(c => c.DefaultCoinCost))
            {
                purchaseAcceptances.Add(new CardAcceptanceDescription(card, CountSource.CountAllOwned, card, Comparison.LessThanEqual, builder.Next(defaultValue: 0, defaultLowerBound: 0)));                
            }

            builder.description = new PickByPriorityDescription(purchaseAcceptances.ToArray());
        }

        public PlayerAction ToPlayerAction()
        {
            var builder = new ParameterBuilderAndRetreiver(this.parameters);
            BuildPurchaseOrder(builder, cards);
            return new PlayerAction(
                this.Name,
                builder.description.ToCardPicker());
        }

        public string Name
        {
            get
            {
                return string.Concat(this.cards.OrderBy(card => card.DefaultCoinCost).Select(card => card.name));
            }
        }
    }

    class ParameterBuilderAndRetreiver
    {
        Parameter[] parameters;
        List<Parameter> result;
        int parameterCountNeeded;
        public PickByPriorityDescription description;

        public ParameterBuilderAndRetreiver()
        {
            this.result = new List<Parameter>();
            this.parameterCountNeeded = 0;
            this.parameters = null;
        }

        public ParameterBuilderAndRetreiver(Parameter[] parameters)
        {
            this.result = null;
            this.parameterCountNeeded = 0;
            this.parameters = parameters;
        }

        public int Next(int defaultValue = 1, int defaultLowerBound = 1, int defaultUpperBound = int.MaxValue)
        {
            if (this.result != null)
            {
                this.result.Add(new Parameter(defaultValue, defaultLowerBound, defaultUpperBound));
                return this.parameterCountNeeded++;
            }
            else
            {
                return this.parameters[this.parameterCountNeeded++].Value;
            }
        }

        public Parameter[] Result
        {
            get
            {
                return this.result.ToArray();
            }
        }
    }
}
