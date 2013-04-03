using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class BagOfCards
        : CollectionCards
    {
        internal void AddCard(Card card)
        {
            this.cards.Add(card);
        }

        public bool HasCard(Type t)
        {
            return this.FindCardIndexOfType(t) != -1;
        }

        public bool HasCard<T>()
        {
            return this.HasCard(typeof(T));
        }

        internal Card RemoveCard()
        {
            return this.RemoveFromEnd();
        }

        internal Card RemoveCard(CardPredicate acceptableCard)
        {
            int cardIndex = this.FindCardIndexThatMatchesPredicate(acceptableCard);
            if (cardIndex == -1)
            {
                return null;
            }

            this.MoveCardToEnd(cardIndex);
            return this.RemoveFromEnd();
        }

        internal Card RemoveCard(Card card)
        {
            return RemoveCard(card.GetType());
        }

        internal Card RemoveCard(Type cardType)
        {
            int cardIndex = this.FindCardIndexOfType(cardType);
            if (cardIndex == -1)
            {
                return null;
            }

            this.MoveCardToEnd(cardIndex);
            return this.RemoveFromEnd();
        }

        public Card FindCard(Type cardType)
        {
            int cardIndex = this.FindCardIndexOfType(cardType);
            if (cardIndex == -1)
            {
                return null;
            }

            return this.cards[cardIndex];
        }

        private int FindCardIndexThatMatchesPredicate(CardPredicate predicate)
        {
            for (int i = 0; i < this.cards.Count; ++i)
            {
                Card card = cards[i];
                if (predicate(card))
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindCardIndexOfType(Type cardEquivalent)
        {
            for (int i = 0; i < this.cards.Count; ++i)
            {
                Card card = cards[i];
                if (card.GetType().Equals(cardEquivalent))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
