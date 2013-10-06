using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    class MapPileOfCardsToProperty<T>
    {
        PileOfCards[] supplyPiles;
        T[] array;

        public MapPileOfCardsToProperty(PileOfCards[] supplyPiles)
        {
            this.supplyPiles = supplyPiles;
            this.array = new T[this.supplyPiles.Length];
        }

        public T this[Card card]
        {
            get
            {
                return this.array[this.GetIndexForPile(GetPile(card))];
            }

            set
            {
                this.array[this.GetIndexForPile(GetPile(card))] = value;
            }
        }       

        public T this[PileOfCards pileOfCards]
        {
            get
            {
                return this.array[this.GetIndexForPile(pileOfCards)];
            }

            set
            {
                this.array[this.GetIndexForPile(pileOfCards)] = value;
            }
        }      

        private PileOfCards GetPile(Card cardType)
        {
            for (int i = 0; i < this.supplyPiles.Length; ++i)
            {
                PileOfCards cardPile = this.supplyPiles[i];
                if (cardPile.IsType(cardType))
                {
                    return cardPile;
                }
            }

            return null;
        }

        private int GetIndexForPile(PileOfCards pile)
        {
            for (int index = 0; index < this.supplyPiles.Length; ++index)
            {
                if (object.ReferenceEquals(pile, this.supplyPiles[index]))
                {
                    return index;
                }
            }

            throw new Exception("Pile not a part of supply");
        }
    }
}
