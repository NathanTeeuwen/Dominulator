using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class SetOfCards
        : CollectionCards
    {

        public SetOfCards(CardGameSubset gameSubset)
            : base(gameSubset, null)
        {
        }        

        public new void Add(Card card)
        {
            if (this.HasCard(card))
                return;
            base.Add(card);
        }

        public new void Remove(Card card)
        {            
            base.Remove(card);
        }
    }
}
