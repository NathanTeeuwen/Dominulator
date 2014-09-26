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
        static private readonly int countSupportedCards = 250;
        static private readonly int sentinelIndex = -1;
        private int nextIndex = 0;
        private int[] mapCardIndexToSubsetIndex = new int[countSupportedCards];
        private Card[] mapSubsetIndexToCard = new Card[countSupportedCards];

        internal bool isInitializing = true;

        public CardGameSubset()
        {
            for (int i = 0; i < countSupportedCards; ++i)
            {
                this.mapCardIndexToSubsetIndex[i] = sentinelIndex;
            }
        }

        internal int CountOfCardTypesInGame
        {
            get
            {
                return this.nextIndex;
            }
        }

        internal void AddCard(Card card)
        {
            if (this.mapCardIndexToSubsetIndex[card.Index] == sentinelIndex)
            {
                int subsetIndex = nextIndex++;
                this.mapCardIndexToSubsetIndex[card.Index] = subsetIndex;
                this.mapSubsetIndexToCard[subsetIndex] = card;
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
            return this.mapCardIndexToSubsetIndex[card.Index];
        }      

        public IEnumerator<Card> GetEnumerator()
        {
            for (int i = 0; i < this.mapSubsetIndexToCard.Length; ++i )
            {
                if (this.mapSubsetIndexToCard[i] == null)
                    yield break;
                yield return this.mapSubsetIndexToCard[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
