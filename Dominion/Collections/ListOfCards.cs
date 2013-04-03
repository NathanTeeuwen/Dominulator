using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class ListOfCards
        : CollectionCards
    {
        static Random random = new Random();

        public void Shuffle()
        {
            for (int i = 0; i < this.cards.Count; ++i)
            {
                int other = ListOfCards.random.Next(this.cards.Count);
                Swap(i, other);
            }
        }

        public Card DrawCardFromTop()
        {
            return this.RemoveFromEnd();
        }

        public Card TopCard()
        {
            if (this.cards.Count == 0)
            {
                return null;
            }
            return this.cards[this.cards.Count - 1];
        }

        public void AddCardToTop(Card card)
        {
            this.cards.Add(card);
        }

        public void RemoveNCardsFromTop(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                this.DrawCardFromTop();
            }
        }
    }
}
