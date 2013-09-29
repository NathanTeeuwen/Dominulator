using Dominion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Program
{
    public class CardPickByBuildOrder
        : ICardPicker
    {
        private readonly Card[] buildOrder;

        public CardPickByBuildOrder(params Card[] buildOrer)
        {
            this.buildOrder = buildOrer.Where(card => card != null).ToArray();
        }

        public int AmountWillingtoOverPayFor(Card card, GameState gameState)
        {
            return int.MaxValue;
        }

        public Type GetPreferredCard(GameState gameState, CardPredicate cardPredicate)
        {        
            var existingCards = new BagOfCards();

            foreach (Card card in gameState.players.CurrentPlayer.AllOwnedCards)
            {
                existingCards.AddCard(card);
            }

            int numberOfTries = 2;

            for (int index = 0; index < this.buildOrder.Length; ++index)
            {
                Card currentCard = this.buildOrder[index];

                if (currentCard == null)
                    continue;

                if (existingCards.HasCard(currentCard))
                {
                    existingCards.RemoveCard(currentCard);
                    continue;
                }
                numberOfTries--;

                if (cardPredicate(currentCard))
                {
                    return currentCard.GetType();
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
            return this.buildOrder;
        }
    }
}
