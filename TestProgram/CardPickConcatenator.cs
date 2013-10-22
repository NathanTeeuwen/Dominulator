using Dominion;
using System;
using System.Collections.Generic;

namespace Program
{
    public class CardPickConcatenator
        : ICardPicker
    {
        private readonly ICardPicker[] matchers;

        public CardPickConcatenator(params ICardPicker[] matchers)
        {
            this.matchers = matchers;
        }

        public Card GetPreferredCard(GameState gameState, CardPredicate cardPredicate)
        {
            foreach (ICardPicker matcher in this.matchers)
            {
                Card result = matcher.GetPreferredCard(gameState, cardPredicate);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public Card GetPreferredCard(GameState gameState, CardPredicate cardPredicate, CardPredicate defaultPredicate)
        {
            foreach (ICardPicker matcher in this.matchers)
            {
                Card result = matcher.GetPreferredCard(gameState, cardPredicate, defaultPredicate);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public Card GetPreferredCardReverse(GameState gameState, CardPredicate cardPredicate)
        {
            for (int i = this.matchers.Length-1; i > 0; --i)            
            {
                ICardPicker matcher = this.matchers[i];
                Card result = matcher.GetPreferredCard(gameState, cardPredicate);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }        

        public IEnumerable<Card> GetNeededCards()
        {
            foreach (ICardPicker matcher in this.matchers)
            {
                foreach (Card card in matcher.GetNeededCards())
                {
                    yield return card;
                }
            }
        }

        public int AmountWillingtoOverPayFor(Card card, GameState gameState)
        {
            int result = 0;
            foreach (ICardPicker picker in this.matchers)
            {
                result = Math.Max(result, picker.AmountWillingtoOverPayFor(card, gameState));                
            }

            return result;
        }
    }
}
