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
        static Random random = new Random();

        private static int NumberBetweenInclusive(int lowerBoundInclusive, int upperBoundInclusive)
        {
            int count = upperBoundInclusive - lowerBoundInclusive + 1;

            return random.Next(count) + lowerBoundInclusive;
        }

        public void Shuffle()
        {
            int lastIndex = this.cards.Count() - 1;
            for (int currentIndex = 0; currentIndex < lastIndex; ++currentIndex)
            {
                int swapIndex = NumberBetweenInclusive(currentIndex, lastIndex);
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

        public void RemoveNCardsFromTop(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                this.DrawCardFromTop();
            }
        }
    }
}
