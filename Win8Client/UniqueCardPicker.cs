using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Generic = System.Collections.Generic;

namespace Win8Client
{
    class UniqueCardPicker
    {
        Random random = new System.Random();
        Generic.Dictionary<DominionCard, bool> excludes;
        IList<DominionCard> allCards;
        int[] remainingCards;
        int maxIndex;

        public UniqueCardPicker(IList<DominionCard> allCards)
        {
            this.allCards = allCards;
            this.remainingCards = new int[allCards.Count];
            for (int i = 0; i < remainingCards.Length; ++i)
                remainingCards[i] = i;

            this.maxIndex = remainingCards.Length - 1;

            this.excludes = new Generic.Dictionary<DominionCard, bool>();
        }

        public void ExcludeCards(Generic.IEnumerable<DominionCard> excludes)
        {
            foreach (var card in excludes)
                this.excludes[card] = true;
        }            

        public bool IsExcluded(DominionCard card)
        {
            bool unused;
            if (this.excludes.TryGetValue(card, out unused))
                return true;

            return false;
        }

        public DominionCard GetCard(Func<DominionCard, bool> meetConstraint)
        {
            while (maxIndex > 0)
            {
                int resultCardIndex = NumberBetweenInclusive(this.random, 0, maxIndex);
                DominionCard currentCard = this.allCards[remainingCards[resultCardIndex]];
                remainingCards[resultCardIndex] = remainingCards[maxIndex];
                --maxIndex;

                if (!IsExcluded(currentCard) && meetConstraint(currentCard))
                    return currentCard;                    
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
