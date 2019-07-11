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

        public int CountTypes
        {
            get
            {
                int result = 0;
                for (int i = 0; i < this.mapGameCardIndexToCount.Length; ++i)
                {
                    if (this.mapGameCardIndexToCount[i] > 0)
                    {
                        result++;
                    }
                }

                return result;
            }
        }

        public IEnumerable<Card> AllTypes
        {
            get
            {                
                for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
                {
                    if (this.mapGameCardIndexToCount[index] > 0)
                    {
                        yield return (Card)this.gameSubset.GetCardForIndex(index);
                    }
                }                
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

            var parent = this.parent;
            while (parent != null)
            {
                parent.mapGameCardIndexToCount[indexForCard] += 1;
                parent.count++;
                parent = parent.parent;
            }                
        }

        protected void AddAllCardsFrom(CollectionCards other)
        {
            for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
            {
                int count = other.mapGameCardIndexToCount[index];
                if (count == 0)
                    continue;

                this.mapGameCardIndexToCount[index] += count;
                this.count+=count;

                var parent = this.parent;
                while (parent != null)
                {
                    parent.mapGameCardIndexToCount[index] += count;
                    parent.count+=count;
                    parent = parent.parent;
                }
            }
        }

        protected void MoveAllCardsFrom(CollectionCards other)
        {
            for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
            {
                this.mapGameCardIndexToCount[index] += other.mapGameCardIndexToCount[index];

                var parent = this.parent;
                while (parent != null)
                {
                    parent.mapGameCardIndexToCount[index] += other.mapGameCardIndexToCount[index];
                    parent = parent.parent;
                }
            }

            this.count += other.count;
            {
                var parent = this.parent;
                while (parent != null)
                {
                    parent.count += other.count;
                    parent = parent.parent;
                }
            }

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


            var parent = this.parent;
            while (parent != null)
            {
                parent.mapGameCardIndexToCount[indexForCard] -= 1;
                parent.count--;
                parent = parent.parent;
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

                var result = (Card)this.gameSubset.GetCardForIndex(index);
                this.Remove(result);                
                return result;
            }

            return null;
        }

        virtual public void Clear()
        {
            if (this.count == 0)
                return;

            var parent = this.parent;
            while (parent != null)
            {
                for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
                {
                    parent.mapGameCardIndexToCount[index] -= this.mapGameCardIndexToCount[index];
                }
                parent.count -= this.count;
                parent = parent.parent;
            }

            this.count = 0;
            Array.Clear(this.mapGameCardIndexToCount, 0, this.mapGameCardIndexToCount.Length);
        }

        public int CountOf(Card card)
        {
            int index = this.gameSubset.GetIndexFor(card);
            if (index == -1)
                return 0;
            return this.mapGameCardIndexToCount[index];
        }

        public bool AnyOf(Card card)
        {
            return this.CountOf(card) > 0;
        }

        public bool Contains(Card card)
        {
            return this.AnyOf(card);
        }

        public bool AnyWhere(CardPredicate predicate)
        {
            for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
            {
                if (this.mapGameCardIndexToCount[index] == 0)
                    continue;

                var card = (Card)this.gameSubset.GetCardForIndex(index);

                if (predicate(card))
                {
                    return true;
                }
            }

            return false;
        }

        public int CountWhere(CardPredicate predicate)
        {
            int result = 0;

            for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)            
            {
                if (this.mapGameCardIndexToCount[index] == 0)
                    continue;

                var card = (Card)this.gameSubset.GetCardForIndex(index);

                if (predicate(card))
                {
                    result += this.mapGameCardIndexToCount[index];
                }
            }

            return result;
        }

        public bool HasDuplicates()
        {
            for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
            {
                if (this.mapGameCardIndexToCount[index] > 1)
                    return true;
            }

            return false;
        }

        public bool HasDuplicatesExceptMenagerie()
        {
            for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
            {
                if (this.gameSubset.GetCardForIndex(index) == Cards.Menagerie)
                    continue;
                if (this.mapGameCardIndexToCount[index] > 1)
                    return true;
            }

            return false;
        }

        public bool HasCard(Card card)
        {
            return CountOf(card) > 0;
        }

        public Card FindCard(CardPredicate predicate)
        {
            for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
            {
                if (this.mapGameCardIndexToCount[index] == 0)
                    continue;

                var card = (Card)this.gameSubset.GetCardForIndex(index);

                if (predicate(card))
                {
                    return card;
                }
            }

            return null;
        }

        public Card SomeCard()
        {
            for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
            {
                if (this.mapGameCardIndexToCount[index] == 0)
                    continue;

                var card = (Card)this.gameSubset.GetCardForIndex(index);

                return card;
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
                var card = (Card)this.gameSubset.GetCardForIndex(index);

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

        public bool IsSubsetOf(CollectionCards other)
        {
            for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
            {
                if (this.mapGameCardIndexToCount[index] > other.mapGameCardIndexToCount[index])
                    return false;
            }

            return true;
        }

        public Card MostCommonCardWhere(CardPredicate predicate)
        {
            Card result = null;
            int resultCount = -1;

            for (int index = 0; index < this.mapGameCardIndexToCount.Length; ++index)
            {
                var card = (Card)this.gameSubset.GetCardForIndex(index);
                if (!predicate(card))
                    continue;

                if (this.mapGameCardIndexToCount[index] > resultCount)
                {
                    resultCount = this.mapGameCardIndexToCount[index];
                    result = card;
                }
            }

            return result;
        }        
    }
}
