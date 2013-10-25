using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class MapOfCards<T>
    {        
        List<T> mapCardIndexToResult;

        public MapOfCards()
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

}
