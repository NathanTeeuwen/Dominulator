using Dominion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominion.Strategy
{
    public class CardPickByPriority
        : ICardPicker
    {
        private readonly CardAcceptance[] cardAcceptances;

        public CardPickByPriority(params CardAcceptance[] cardAcceptances)
        {
            this.cardAcceptances = cardAcceptances.Where( acceptance => acceptance.card != null).ToArray();
        }

        public int AmountWillingtoOverPayFor(Card card, GameState gameState)
        {
            int result = 0;
            foreach (CardAcceptance acceptance in this.cardAcceptances)
            {                
                if (acceptance.match(gameState) && acceptance.card.Equals(card))
                {
                    result = Math.Max(result, acceptance.overpayAmount(gameState));                    
                }
            }

            return result;
        }

        public Card GetPreferredCard(GameState gameState, CardPredicate cardPredicate)
        {
            foreach (CardAcceptance acceptance in this.cardAcceptances)
            {
                if (cardPredicate(acceptance.card) &&
                    acceptance.match(gameState))
                {                    
                    return acceptance.card;
                }
            }
            
            return null;
        }

        public Card GetPreferredCard(GameState gameState, CardPredicate cardPredicate, CardPredicate defaultPredicate)
        {            
            foreach (CardAcceptance acceptance in this.cardAcceptances)
            {
                if (cardPredicate(acceptance.card))
                {
                    if (acceptance.match == CardAcceptance.DefaultMatch && defaultPredicate(acceptance.card) ||
                        acceptance.match != CardAcceptance.DefaultMatch && acceptance.match(gameState))
                    {
                        return acceptance.card;
                    }
                }
            }

            return null;
        }

        public Card GetPreferredCardReverse(GameState gameState, CardPredicate cardPredicate)
        {
            for (int i = this.cardAcceptances.Length - 1; i >= 0; i--)
            {
                CardAcceptance acceptance = this.cardAcceptances[i];
                if (cardPredicate(acceptance.card) &&
                    acceptance.match(gameState))
                {
                    return acceptance.card;
                }
            }

            return null;
        }

        public IEnumerable<Card> GetNeededCards()
        {
            return this.cardAcceptances.Select(cardAcceptance => cardAcceptance.card);
        }
    }

}
