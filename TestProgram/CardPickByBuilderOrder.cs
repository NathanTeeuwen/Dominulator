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
            return int.MaxValue;
        }

        public Card GetPreferredCardReverse(GameState gameState, CardPredicate cardPredicate)
        {
            throw new NotImplementedException();
        }

        public Card GetPreferredCard(GameState gameState, CardPredicate cardPredicate)
        {        
            var existingCards = new BagOfCards(gameState.CardGameSubset);

            foreach (Card card in gameState.players.CurrentPlayer.AllOwnedCards)
            {
                existingCards.AddCard(card);
            }

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

        public IEnumerable<Card> GetNeededCards()
        {
            return this.buildOrder.Select(acceptance => acceptance.card);
        }
    }
}
