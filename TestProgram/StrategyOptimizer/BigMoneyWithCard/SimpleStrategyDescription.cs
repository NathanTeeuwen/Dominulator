using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program.GeneticAlgorithm;

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
            var purchaseAcceptances = new List<CardAcceptance>();

            // do the end game
            purchaseAcceptances.Add(CardAcceptance.For(Cards.Province, gameState => Strategy.CountAllOwned(Cards.Gold, gameState) >= builder.Next(defaultValue:2, defaultLowerBound:0)));
            purchaseAcceptances.Add(CardAcceptance.For(Cards.Duchy, gameState => Strategy.CountOfPile(Cards.Province, gameState) <= builder.Next(defaultValue:3, defaultLowerBound:0)));
            purchaseAcceptances.Add(CardAcceptance.For(Cards.Estate, gameState => Strategy.CountOfPile(Cards.Province, gameState) <= builder.Next(defaultValue: 1, defaultLowerBound:0)));
            foreach (Card card in cards.Where(c => c.DefaultCoinCost >= Cards.Gold.DefaultCoinCost).OrderBy(c => c.DefaultCoinCost))
            {
                purchaseAcceptances.Add(CardAcceptance.For(card, builder.Next(defaultValue: 0)));
            }
            purchaseAcceptances.Add(CardAcceptance.For(Cards.Gold));
            foreach (Card card in cards.Where(c => c.DefaultCoinCost >= Cards.Potion.DefaultCoinCost && c.DefaultCoinCost < Cards.Gold.DefaultCoinCost).OrderBy(c => c.DefaultCoinCost))
            {
                purchaseAcceptances.Add(CardAcceptance.For(card, builder.Next(defaultValue: 0)));
            }
            purchaseAcceptances.Add(CardAcceptance.For(Cards.Estate, gameState => Strategy.CountOfPile(Cards.Province, gameState) <= builder.Next(defaultValue:2, defaultLowerBound:0)));
            if (cards.Where(card => card.potionCost != 0).Any())
            {
                purchaseAcceptances.Add(CardAcceptance.For(Cards.Potion, builder.Next()));
            }
            foreach (Card card in cards.Where(c => c.DefaultCoinCost >= Cards.Silver.DefaultCoinCost && c.DefaultCoinCost < Cards.Potion.DefaultCoinCost).OrderBy(c => c.DefaultCoinCost))
            {
                purchaseAcceptances.Add(CardAcceptance.For(card, builder.Next(defaultValue: 0)));
            }
            purchaseAcceptances.Add(CardAcceptance.For(Cards.Silver));
            foreach (Card card in cards.Where(c => c.DefaultCoinCost < Cards.Silver.DefaultCoinCost).OrderBy(c => c.DefaultCoinCost))
            {
                purchaseAcceptances.Add(CardAcceptance.For(card, builder.Next(defaultValue: 0)));
            }
            builder.purchaseOrder = new CardPickByPriority(purchaseAcceptances.ToArray());            
        }

        public PlayerAction ToPlayerAction()
        {
            var builder = new ParameterBuilderAndRetreiver(this.parameters);
            BuildPurchaseOrder(builder, cards);
            return new PlayerAction(
                this.Name,
                builder.purchaseOrder);
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
        public ICardPicker purchaseOrder;

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
