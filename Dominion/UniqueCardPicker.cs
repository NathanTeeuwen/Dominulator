using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class UniqueCardPicker<T>
        where T : Dominion.CardShapedObject
    {
        private readonly Random random;
        private readonly Dictionary<T, bool> excludes;
        private readonly T[] allCards;
        private readonly int[] remainingCards;
        int maxIndex;

        public UniqueCardPicker(IEnumerable<T> allCards, Random random)
        {
            this.random = random;
            this.allCards = allCards.ToArray();
            this.remainingCards = new int[this.allCards.Length];
            for (int i = 0; i < remainingCards.Length; ++i)
                remainingCards[i] = i;

            this.maxIndex = remainingCards.Length - 1;

            this.excludes = new Dictionary<T, bool>();
        }

        public void ExcludeCards(IEnumerable<T> excludes)
        {
            foreach (var card in excludes)
                this.excludes[card] = true;
        }

        public bool IsExcluded(T card)
        {
            bool unused;
            if (this.excludes.TryGetValue(card, out unused))
                return true;

            return false;
        }

        public T GetCard(Func<T, bool> meetConstraint)
        {
            int curIndex = this.maxIndex;
            while (curIndex >= 0)
            {
                int resultCardIndex = NumberBetweenInclusive(this.random, 0, maxIndex);
                T currentCard = this.allCards[remainingCards[resultCardIndex]];

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
