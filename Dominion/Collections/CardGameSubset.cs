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
        static private readonly int countSupportedCards = Dominion.Cards.AllCardsList.Length;
        static private readonly int sentinelIndex = -1;
        private int nextIndex = 0;
        private int[] mapCardIndexToSubsetIndex = new int[countSupportedCards];
        private Card[] mapSubsetIndexToCard = new Card[countSupportedCards];

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

        public struct Enumerator
            : IEnumerator<Card>
        {
            int currentIndex;
            private CardGameSubset cardGameSubset;

            public Enumerator(CardGameSubset cardGameSubset)
            {
                this.currentIndex = -1;
                this.cardGameSubset = cardGameSubset;                
            }

            public void Reset()
            {
                this.currentIndex = -1;
            }

            public bool MoveNext()
            {
                this.currentIndex++;
                return this.currentIndex < this.cardGameSubset.nextIndex;
            }

            public Card Current
            {
                get
                {
                    return this.cardGameSubset.GetCardForIndex(this.currentIndex);
                }
            }

            public void Dispose()
            {

            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<Card> IEnumerable<Card>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
