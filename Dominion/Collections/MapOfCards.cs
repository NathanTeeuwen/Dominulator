using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class MapOfCards<T>
    {        
        private readonly List<T> mapCardIndexToResult;

        public MapOfCards()
        {
            this.mapCardIndexToResult = new List<T>(capacity: Game.ApproxNumberOfDifferentCards);
        }

        private void GrowToSize(int maxIndex)
        {
            while (maxIndex >= this.mapCardIndexToResult.Count)
            {
                this.mapCardIndexToResult.Add(default(T));
            }
        }

        public T this[Card card]
        {
            set
            {
                GrowToSize(card.Index);                
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

        public void CopyNonSentinelValues(MapOfCards<T> other)
        {
            GrowToSize(other.mapCardIndexToResult.Count+1);
            for (int index = 0; index < other.mapCardIndexToResult.Count; ++index)
            {
                if (!other.mapCardIndexToResult[index].Equals(default(T)))
                {
                    this.mapCardIndexToResult[index] = other.mapCardIndexToResult[index];
                }
            }
        }
    }

}
