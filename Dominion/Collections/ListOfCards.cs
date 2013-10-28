using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class ListOfCards
        : CollectionCards
    {
        private List<Card> cards = new List<Card>(20);
        private int countKnownCard = 0;

        public ListOfCards(CardGameSubset gameSubset)
            : base(gameSubset, null)
        {
        }

        public ListOfCards(CardGameSubset gameSubset, BagOfCards parent)
            : base(gameSubset, parent)
        {
        }

        private static int NumberBetweenInclusive(Random random, int lowerBoundInclusive, int upperBoundInclusive)
        {            
            int count = upperBoundInclusive - lowerBoundInclusive + 1;
            return random.Next(count) + lowerBoundInclusive;            
        }

        public void Shuffle(Random random)
        {
            int lastIndex = this.cards.Count - 1;
            for (int currentIndex = 0; currentIndex < lastIndex; ++currentIndex)
            {
                int swapIndex = NumberBetweenInclusive(random, currentIndex, lastIndex);
                Swap(currentIndex, swapIndex);
            }

            this.countKnownCard = 0;
        }

        public override void Clear()
        {
            base.Clear();
            this.cards.Clear();
        }

        public Card DrawCardFromTop()
        {
            if (this.countKnownCard > 0)
                this.countKnownCard--;

            Card result = this.RemoveFromEnd();
            base.Remove(result);

            return result;
        }

        public Card FindAndRemoveCardOrderDestroyed(Card card)
        {
            if (this.countKnownCard > 0)
                throw new System.Exception("not sure what to do with known cards");

            int cardIndex = FindCardIndex(card);
            if (cardIndex == -1)
                throw new System.Exception("couldnt find card");

            this.MoveCardToEnd(cardIndex);

            return this.DrawCardFromTop();
        } 

        public Card TopCard()
        {
            if (this.cards.Count == 0)
            {
                return null;
            }
            return this.cards[this.cards.Count - 1];
        }

        public Card BottomCard()
        {
            if (this.cards.Count == 0)
            {
                return null;
            }
            return this.cards[0];
        }

        internal void MoveBottomCardToTop()
        {
            Card bottomCard = this.BottomCard();
            if (bottomCard != null)
            {
                this.cards.RemoveAt(0);
                this.AddCardToTop(bottomCard);
            }
        }

        public void AddCardToTop(Card card)
        {
            this.countKnownCard++;
            this.cards.Add(card);
            base.Add(card);
        }

        public void AddNCardsToTop(Card card, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                this.AddCardToTop(card);
            }
        }

        public void RemoveNCardsFromTop(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                this.DrawCardFromTop();
            }
        }        

        public IEnumerable<Card> KnownCards
        {
            get
            {
                for (int index = 0; index < this.countKnownCard; ++index)
                {
                    yield return this.cards[this.cards.Count - 1 - index];
                }
            }
        }

        internal void EraseKnownCountKnowledge()
        {
            this.countKnownCard = 0;
        }

        protected void Swap(int indexfirst, int indexSecond)
        {
            Card temp = this.cards[indexfirst];
            this.cards[indexfirst] = this.cards[indexSecond];
            this.cards[indexSecond] = temp;
        }

        protected int FindCardIndex(Card card)
        {
            for (int i = 0; i < this.cards.Count; ++i)
            {
                if (this.cards[i] == card)
                    return i;
            }

            return -1;
        }

        protected void MoveCardToEnd(int cardIndex)
        {
            if (this.cards.Count > 1)
            {
                this.Swap(cardIndex, this.cards.Count - 1);
            }
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

        public override IEnumerator<Card> GetEnumerator()
        {
            return this.cards.GetEnumerator();
        }        
    }
}
