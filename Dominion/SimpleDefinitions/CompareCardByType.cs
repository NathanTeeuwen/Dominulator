using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    public struct CompareCardByType
            : IEqualityComparer<Card>,
              IComparer<Card>
    {
        public bool Equals(Card x, Card y)
        {
            return x.GetType().Equals(y.GetType());
        }

        public int GetHashCode(Card x)
        {
            return x.GetType().GetHashCode();
        }

        public int Compare(Card x, Card y)
        {
            return x.name.CompareTo(y.name);
        }
    }
}