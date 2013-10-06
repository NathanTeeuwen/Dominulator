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
        public void Add(Card card)
        {
            if (this.HasCard(card))
                return;
            this.cards.Add(card);
        }

        public void Remove(Card card)
        {
            this.cards.Remove(card);
        }
    }
}
