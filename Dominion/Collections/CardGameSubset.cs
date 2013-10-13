using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class CardGameSubset
        : IEnumerable<Card>
    {
        static private readonly int sentinelIndex = -1;
        private int nextIndex = 0;
        private List<int> mapCardIndexToSubsetIndex = new List<int>(250);
        private List<Card> mapSubsetIndexToCard = new List<Card>(250);

        internal bool isInitializing = true;

        internal int CountOfCardTypesInGame
        {
            get
            {
                return this.nextIndex;
            }
        }

        internal void AddCard(Card card)
        {
            GrowToHandleCard(card);
            if (this.mapCardIndexToSubsetIndex[card.index] == sentinelIndex)
            {
                int subsetIndex = nextIndex++;
                this.mapCardIndexToSubsetIndex[card.index] = subsetIndex;
                this.mapSubsetIndexToCard.Add(card);
            }
        }

        internal Card GetCardForIndex(int index)
        {
            return this.mapSubsetIndexToCard[index];
        }

        public bool HasCard(Card card)
        {
            return this.GetIndexFor(card) != -1;
        }

        internal int GetIndexFor(Card card)
        {
            if (card.index >= this.mapCardIndexToSubsetIndex.Count)
                return -1;

            return this.mapCardIndexToSubsetIndex[card.index];
        }

        private void GrowToHandleCard(Card card)
        {
            while (this.mapCardIndexToSubsetIndex.Count < card.index + 1)
            {
                if (!this.isInitializing)
                    throw new Exception("Can not use unexpected card after initializing");

                this.mapCardIndexToSubsetIndex.Add(sentinelIndex);                
            }
        }

        public IEnumerator<Card> GetEnumerator()
        {
            return mapSubsetIndexToCard.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
