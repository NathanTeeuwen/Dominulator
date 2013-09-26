using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public struct CardCountPair
    {
        private Card card;
        private int count;

        public CardCountPair(Card card, int count)
        {
            this.card = card;
            this.count = count;
        }

        public Card Card
        {
            get
            {
                return this.card;
            }
        }

        public int Count
        {
            get
            {
                return this.count;
            }
        }
    }
}
