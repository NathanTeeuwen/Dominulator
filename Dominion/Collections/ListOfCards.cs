using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class ListOfCards
        : CollectionCards
    {
        private int countKnownCard = 0;

        private static int NumberBetweenInclusive(Random random, int lowerBoundInclusive, int upperBoundInclusive)
        {
            lock (random)
            {
                int count = upperBoundInclusive - lowerBoundInclusive + 1;

                return random.Next(count) + lowerBoundInclusive;
            }
        }

        public void Shuffle(Random random)
        {
            int lastIndex = this.cards.Count() - 1;
            for (int currentIndex = 0; currentIndex < lastIndex; ++currentIndex)
            {
                int swapIndex = NumberBetweenInclusive(random, currentIndex, lastIndex);
                Swap(currentIndex, swapIndex);
            }

            this.countKnownCard = 0;
        }

        public Card DrawCardFromTop()
        {
            if (this.countKnownCard > 0)
                this.countKnownCard--;

            return this.RemoveFromEnd();            
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
    }
}
