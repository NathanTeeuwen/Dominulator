using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public class CollectionCards
        : IEnumerable<Card>
    {
        protected readonly CardGameSubset gameSubset;
        protected int[] mapGameCardIndexToCount;
        protected readonly BagOfCards parent;
        private int count;

        public CollectionCards(CardGameSubset gameSubset, BagOfCards parent)
        {
            this.parent = parent;
            this.gameSubset = gameSubset;
            this.mapGameCardIndexToCount = new int[this.gameSubset.CountOfCardTypesInGame];
        }

        public int Count
        {
            get
            {
                return this.count;
            }
        }

        public bool Any
        {
            get
            {
                return this.Count > 0;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return this.count == 0;
            }
        }

        protected void Add(Card card)
        {
            if (card == null)
                return;

            int indexForCard = this.gameSubset.GetIndexFor(card);
            if (indexForCard == -1)
                throw new Exception("Tried to add card that wasn't in the game");

            this.mapGameCardIndexToCount[indexForCard] += 1;
            this.count++;

            if (this.parent != null)
            {
                this.parent.mapGameCardIndexToCount[indexForCard] += 1;
                this.parent.count++;
            }                
        }

        protected void MoveAllCardsFrom(CollectionCards other)
        {
            for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
            {
                this.mapGameCardIndexToCount[index] += other.mapGameCardIndexToCount[index];
            }

            this.count += other.count;

            other.Clear();
        }

        protected Card Remove(Card card)        
        {
            if (card == null)
                return null;

            int indexForCard = this.gameSubset.GetIndexFor(card);

            if (indexForCard == -1)
                return null;

            if (this.mapGameCardIndexToCount[indexForCard]  == 0)
            {
                return null;                
            }

            this.mapGameCardIndexToCount[indexForCard] -= 1;
            this.count--;

            if (this.parent != null)
            {
                this.parent.mapGameCardIndexToCount[indexForCard] -= 1;
                this.parent.count--;
            }

            return card;
        }

        protected Card RemoveCard(CardPredicate acceptableCard)
        {
            Card result = this.FindCard(acceptableCard);
            if (result != null)
            {
                this.Remove(result);
            }
            return result;
        }

        protected void CopyFrom(CollectionCards other)
        {
            Array.Copy(other.mapGameCardIndexToCount, this.mapGameCardIndexToCount, other.mapGameCardIndexToCount.Length);
            this.count = other.count;
        }

        protected Card RemoveSomeCard()
        {
            for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
            {
                if (this.mapGameCardIndexToCount[index] == 0)
                    continue;

                Card result = this.gameSubset.GetCardForIndex(index);
                this.Remove(result);                
                return result;
            }

            return null;
        }

        virtual public void Clear()
        {
            if (this.parent != null)
            {
                for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
                {
                    parent.mapGameCardIndexToCount[index] -= this.mapGameCardIndexToCount[index];
                }
                parent.count -= this.count;
            }

            this.count = 0;
            Array.Clear(this.mapGameCardIndexToCount, 0, this.mapGameCardIndexToCount.Length);
        }

        public int CountOfCard<T>()
            where T : Card, new()
        {
            return CountOfCard(Card.Type<T>());
        }

        public int CountOfCard(Card card)
        {
            int index = this.gameSubset.GetIndexFor(card);
            if (index == -1)
                return 0;
            return this.mapGameCardIndexToCount[index];
        }

        public int CountCards(CardPredicate predicate)
        {
            int result = 0;

            for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)            
            {
                if (this.mapGameCardIndexToCount[index] == 0)
                    continue;

                Card card = this.gameSubset.GetCardForIndex(index);

                if (predicate(card))
                {
                    result += this.mapGameCardIndexToCount[index];
                }
            }

            return result;
        }

        public bool HasCard(Card card)
        {
            int subsetIndex = this.gameSubset.GetIndexFor(card);
            if (subsetIndex == -1)
                return false;
            return this.mapGameCardIndexToCount[subsetIndex] > 0;            
        }

        public bool HasCard<T>()
            where T : Card, new()
        {
            return this.HasCard(Card.Type<T>());
        }

        public Card FindCard(CardPredicate predicate)
        {
            for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
            {
                if (this.mapGameCardIndexToCount[index] == 0)
                    continue;

                Card card = this.gameSubset.GetCardForIndex(index);

                if (predicate(card))
                {
                    return card;
                }
            }

            return null;
        }

        public bool HasCard(CardPredicate predicate)
        {
            return FindCard(predicate) != null;
        }

        public virtual IEnumerator<Card> GetEnumerator()
        {
            for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
            {
                Card card = this.gameSubset.GetCardForIndex(index);

                for (int i = 0; i < this.mapGameCardIndexToCount[index]; ++i)
                    yield return card;
            }
        }

        IEnumerator<Card> IEnumerable<Card>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
