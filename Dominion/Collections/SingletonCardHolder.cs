using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    class SingletonCardHolder
    {
        private Card card;
        private readonly BagOfCards parent;

        public SingletonCardHolder(BagOfCards parent)
        {
            this.parent = parent;
        }

        public void Set(Card card)
        {
            System.Diagnostics.Debug.Assert(this.card == null);
            this.card = card;
            this.parent.AddCard(card);
        }

        public void Clear()
        {
            System.Diagnostics.Debug.Assert(this.card != null);
            this.parent.RemoveCard(this.card);
            this.card = null;            
        }

        public Card Card
        {
            get
            {
                return this.card;
            }
        }

    }
}
