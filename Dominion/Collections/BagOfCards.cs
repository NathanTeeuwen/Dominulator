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

        public BagOfCards(CardGameSubset gameSubset)
            : base(gameSubset)
        {            
        }

        public void CopyFrom(BagOfCards other)
        {
            this.Clear();
            foreach (Card card in other)
            {
                this.AddCard(card);
            }
        }

        public void AddCard(Card card)
        {
            this.cards.Add(card);
        }             

        public bool HasCard<T>()
            where T : Card, new()
        {
            return this.HasCard(Card.Type<T>());
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

        public Card RemoveCard(Card cardType)
        {            
            int cardIndex = this.FindCardIndexOfType(cardType);
            if (cardIndex == -1)
            {
                return null;
            }

            this.MoveCardToEnd(cardIndex);
            return this.RemoveFromEnd();
        }

        public Card FindCard(Card cardType)
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

        private int FindCardIndexOfType(Card cardEquivalent)
        {
            for (int i = 0; i < this.cards.Count; ++i)
            {
                Card card = cards[i];
                if (card.Equals(cardEquivalent))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
