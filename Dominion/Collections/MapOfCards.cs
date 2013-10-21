using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class MapOfCardsFor<T>
    {        
        List<T> mapCardIndexToResult;

        public MapOfCardsFor()
        {            
            this.mapCardIndexToResult = new List<T>(capacity:250);
        }

        public T this[Card card]
        {
            set
            {
                while (card.Index >= this.mapCardIndexToResult.Count)
                {
                    this.mapCardIndexToResult.Add(default(T));
                }
                this.mapCardIndexToResult[card.Index] = value;
            }

            get
            {
                if (card.Index >= this.mapCardIndexToResult.Count)
                {
                    return default(T);
                }
                return this.mapCardIndexToResult[card.Index];
            }
        }
    }

    public class MapOfCardsForGameSubset<T>
    {
        private readonly CardGameSubset gameSubset;
        T[] mapCardIndexToResult;

        public MapOfCardsForGameSubset(CardGameSubset gameSubset)
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
