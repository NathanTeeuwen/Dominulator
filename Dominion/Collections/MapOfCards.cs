using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    class MapOfCards<T>
    {
        private readonly CardGameSubset gameSubset;
        T[] mapCardIndexToResult;

        public MapOfCards(CardGameSubset gameSubset)
        {
            this.gameSubset = gameSubset;
            this.mapCardIndexToResult = new T[gameSubset.CountOfCardTypesInGame];
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
