using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class CollectionCards
        : IEnumerable<Card>
    {
        protected List<Card> cards = new List<Card>(20);

        public int Count
        {
            get
            {
                return this.cards.Count;
            }
        }

        public bool Any
        {
            get
            {
                return !this.IsEmpty;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return this.cards.Count == 0;
            }
        }

        internal void Clear()
        {
            this.cards.Clear();
        }

        public int CountCards(CardPredicate predicate)
        {
            int result = 0;

            for (int cardIndex = 0; cardIndex < this.cards.Count; ++cardIndex)
            {
                Card card = this.cards[cardIndex];

                if (predicate(card))
                {
                    ++result;
                }
            }

            return result;
        }

        public bool HasCard(CardPredicate predicate)
        {
            for (int cardIndex = 0; cardIndex < this.cards.Count; ++cardIndex)
            {
                Card card = this.cards[cardIndex];

                if (predicate(card))
                {
                    return true;
                }
            }

            return false;
        }

        protected void Swap(int indexfirst, int indexSecond)
        {
            Card temp = this.cards[indexfirst];
            this.cards[indexfirst] = this.cards[indexSecond];
            this.cards[indexSecond] = temp;
        }

        protected void MoveCardToEnd(int cardIndex)
        {
            if (this.cards.Count > 1)
            {
                this.Swap(cardIndex, this.cards.Count - 1);
            }
        }

        public IEnumerator<Card> GetEnumerator()
        {
            return this.cards.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        protected Card RemoveFromEnd()
        {
            if (this.cards.Count == 0)
            {
                return null;
            }

            Card result = this.cards[this.cards.Count - 1];
            this.cards.RemoveAt(this.cards.Count - 1);
            return result;
        }        
    }
}
