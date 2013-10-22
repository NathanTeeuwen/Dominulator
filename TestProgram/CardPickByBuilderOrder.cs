using Dominion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Program
{
    public class CardPickByBuildOrder
        : ICardPicker
    {
        private readonly CardAcceptance[] buildOrder;

        public CardPickByBuildOrder(params CardAcceptance[] buildOrer)
        {
            this.buildOrder = buildOrer.Where(acceptance => acceptance != null && acceptance.card != null).ToArray();
        }

        public int AmountWillingtoOverPayFor(Card card, GameState gameState)
        {
            bool wantsToGainCard = this.GetPreferredCard(gameState, c => c == card) != null;
            return wantsToGainCard ? CardAcceptance.DefaultOverpayAmount(gameState) : 0;
        }

        public Card GetPreferredCardReverse(GameState gameState, CardPredicate cardPredicate)
        {
            throw new NotImplementedException();
        }

        public Card GetPreferredCard(GameState gameState, CardPredicate cardPredicate)
        {
            var existingCards = gameState.Self.AllOwnedCards.Clone();

            int numberOfTries = 2;

            for (int index = 0; index < this.buildOrder.Length; ++index)
            {
                CardAcceptance acceptance = this.buildOrder[index];

                if (acceptance == null)
                    continue;

                if (!acceptance.match(gameState))
                    continue;

                Card currentCard = acceptance.card;

                if (existingCards.HasCard(currentCard))
                {
                    existingCards.RemoveCard(currentCard);
                    continue;
                }
                numberOfTries--;

                if (cardPredicate(currentCard))
                {
                    return currentCard;
                }

                if (numberOfTries == 0)
                {
                    break;
                }
            }

            return null;
        }

        public Card GetPreferredCard(GameState gameState, CardPredicate cardPredicate, CardPredicate defaultPredicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Card> GetNeededCards()
        {
            return this.buildOrder.Select(acceptance => acceptance.card);
        }
    }
}
