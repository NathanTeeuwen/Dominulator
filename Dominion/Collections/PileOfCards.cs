using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class PileOfCards
        : ListOfCards
    {
        private Card protoType;

        internal int embargoTokenCount;
        internal bool tradeRouteTokenCount;
        internal readonly bool isInSupply;

        public PileOfCards(Card protoType, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                this.AddCardToTop(protoType);
            }

            this.protoType = protoType;
        }

        public bool IsType(Type card)
        {
            return this.protoType.Is(card);
        }

        public bool IsType<T>()
        {
            return IsType(typeof(T));
        }
    }
}
