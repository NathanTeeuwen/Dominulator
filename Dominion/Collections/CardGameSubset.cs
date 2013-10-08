using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class CardGameSubset
    {
        static private readonly int sentinelIndex = -1;
        private int nextIndex = 0;
        private List<int> mapCardIndexToSubsetIndex = new List<int>(250);

        internal bool isInitializing = true;

        internal void AddCard(Card card)
        {
            GrowToHandleCard(card);
            if (mapCardIndexToSubsetIndex[card.index] == sentinelIndex)
            {
                mapCardIndexToSubsetIndex[card.index] = nextIndex++;
            }
        }

        internal int GetIndexFor(Card card)
        {
            return this.mapCardIndexToSubsetIndex[card.index];
        }

        private void GrowToHandleCard(Card card)
        {
            while (mapCardIndexToSubsetIndex.Count < card.index + 1)
            {
                if (!this.isInitializing)
                    throw new Exception("Can not use unexpected card after initializing");

                mapCardIndexToSubsetIndex.Add(sentinelIndex);
            }
        }
    }
}
