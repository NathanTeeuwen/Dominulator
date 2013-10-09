using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class BagOfCards
        : CollectionCards
    {

        public BagOfCards(CardGameSubset gameSubset)
            : this(gameSubset, null)
        {
        }

        public BagOfCards(CardGameSubset gameSubset, BagOfCards parent)
            : base(gameSubset, parent)
        {            
        }

        public BagOfCards Clone()
        {
            var result = new BagOfCards(this.gameSubset);
            result.CopyFrom(this);
            return result;
        }

        public void CopyFrom(BagOfCards other)
        {
            base.CopyFrom(other);
        }

        public void AddCard(Card card)
        {
            base.Add(card);
        }        

        internal void MoveAllCardsFrom(CollectionCards other)
        {
            base.MoveAllCardsFrom(other);
        }

        internal new Card RemoveCard(CardPredicate acceptableCard)
        {
            return base.RemoveCard(acceptableCard);
        }

        public Card RemoveCard(Card cardType)
        {
            return base.Remove(cardType);
        }

        internal new Card RemoveSomeCard()
        {
            return base.RemoveSomeCard();
        }  
    }
}
