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
        
        public PileOfCards(Card protoType, int count)
        {
            this.AddNCardsToTop(protoType, count);           
            this.protoType = protoType;            
        }

        public PileOfCards(Card protoType)
        {
            this.protoType = protoType;
        }

        public bool IsType(Type card)
        {
            if (this.protoType.Is(card))
                return true;

            if (card.BaseType != typeof(Card))
                return IsType(card.BaseType);

            return false;
        }

        public bool IsType<T>()
        {
            return IsType(typeof(T));
        }

        public Card ProtoTypeCard
        {
            get
            {
                return this.protoType;
            }
        }

    }
}
