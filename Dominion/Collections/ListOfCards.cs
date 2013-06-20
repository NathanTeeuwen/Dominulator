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
        }

        public Card DrawCardFromTop()
        {
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

        public void AddCardToTop(Card card)
        {
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
    }
}
