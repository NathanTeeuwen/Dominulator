using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class MapOfCardsForGameSubset<T>
    {
        private readonly CardGameSubset gameSubset;
        T[] mapCardIndexToResult;

        public MapOfCardsForGameSubset(CardGameSubset gameSubset)
        {
            this.gameSubset = gameSubset;
            this.mapCardIndexToResult = new T[gameSubset.CountOfCardTypesInGame];
        }

        public CardGameSubset GameSubset
        {
            get
            {
                return this.gameSubset;
            }
        }

        public T this[Card card]
        {
            set
            {
                int index = this.gameSubset.GetIndexFor(card);
                this.mapCardIndexToResult[index] = value;
            }

            get
            {
                int index = this.gameSubset.GetIndexFor(card);
                return this.mapCardIndexToResult[index];
            }
        }
    }
}
