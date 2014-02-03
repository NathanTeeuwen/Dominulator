using Dominion;
using System;
using System.Collections.Generic;

namespace Dominion.Strategy
{
    public class CardPickForCondition
        : ICardPicker
    {
        private readonly ICardPicker picker;
        private readonly GameStatePredicate predicate;

        public CardPickForCondition(GameStatePredicate predicate, ICardPicker picker)
        {
            this.picker = picker;
            this.predicate = predicate;
        }

        public Card GetPreferredCard(GameState gameState, CardPredicate cardPredicate)
        {
            if (predicate(gameState))
                return this.picker.GetPreferredCard(gameState, cardPredicate);

            return null;            
        }

        public Card GetPreferredCard(GameState gameState, CardPredicate cardPredicate, CardPredicate defaultPredicate)
        {
            if (predicate(gameState))
                return this.picker.GetPreferredCard(gameState, cardPredicate, defaultPredicate);

            return null; 
        }

        public Card GetPreferredCardReverse(GameState gameState, CardPredicate cardPredicate)
        {
            if (predicate(gameState))
                return this.picker.GetPreferredCardReverse(gameState, cardPredicate);

            return null; 
        }        

        public IEnumerable<Card> GetNeededCards()
        {
            return this.picker.GetNeededCards();
        }

        public int AmountWillingtoOverPayFor(Card card, GameState gameState)
        {
            if (predicate(gameState))
                return this.picker.AmountWillingtoOverPayFor(card, gameState);

            return 0; 
        }
    }
}
