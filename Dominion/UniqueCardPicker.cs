using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class UniqueCardPicker
    {
        private readonly Random random;
        private readonly Dictionary<Dominion.Card, bool> excludes;
        private readonly Dominion.Card[] allCards;
        private readonly int[] remainingCards;
        int maxIndex;

        public UniqueCardPicker(IEnumerable<Dominion.Card> allCards, Random random)
        {
            this.random = random;
            this.allCards = allCards.ToArray();
            this.remainingCards = new int[this.allCards.Length];
            for (int i = 0; i < remainingCards.Length; ++i)
                remainingCards[i] = i;

            this.maxIndex = remainingCards.Length - 1;

            this.excludes = new Dictionary<Dominion.Card, bool>();
        }

        public void ExcludeCards(IEnumerable<Dominion.Card> excludes)
        {
            foreach (var card in excludes)
                this.excludes[card] = true;
        }

        public bool IsExcluded(Dominion.Card card)
        {
            bool unused;
            if (this.excludes.TryGetValue(card, out unused))
                return true;

            return false;
        }

        public Dominion.Card GetCard(Func<Dominion.Card, bool> meetConstraint)
        {
            int curIndex = this.maxIndex;
            while (curIndex >= 0)
            {
                int resultCardIndex = NumberBetweenInclusive(this.random, 0, maxIndex);
                Dominion.Card currentCard = this.allCards[remainingCards[resultCardIndex]];

                if (!IsExcluded(currentCard) && meetConstraint(currentCard))
                {
                    remainingCards[resultCardIndex] = remainingCards[maxIndex];
                    --maxIndex;
                    return currentCard;
                }
                --curIndex;
            }

            return null;
        }

        private static int NumberBetweenInclusive(Random random, int lowerBoundInclusive, int upperBoundInclusive)
        {
            int count = upperBoundInclusive - lowerBoundInclusive + 1;
            return random.Next(count) + lowerBoundInclusive;
        }
    }    
}
