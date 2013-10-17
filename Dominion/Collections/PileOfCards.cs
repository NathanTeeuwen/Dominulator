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

        public PileOfCards(CardGameSubset gameSubset, Card protoType, int count)
            : base(gameSubset)
        {
            this.AddNCardsToTop(protoType, count);           
            this.protoType = protoType;            
        }

        public PileOfCards(CardGameSubset gameSubset, Card protoType)
            : base(gameSubset)
        {
            this.protoType = protoType;
        }

        public bool IsType(Card card)
        {
            return card.IsType(this.protoType);            
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
