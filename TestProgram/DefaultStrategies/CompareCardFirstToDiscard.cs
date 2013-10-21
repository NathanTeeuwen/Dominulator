using Dominion;
using System.Collections.Generic;

namespace Program.DefaultStrategies
{
    struct CompareCardByFirstToDiscard
           : IComparer<Card>
    {
        public int Compare(Card x, Card y)
        {
            if (x.isCurse ^ y.isCurse)
            {
                return x.isCurse ? -1 : 1;
            }

            if (x.isAction ^ y.isAction)
            {
                return x.isAction ? 1 : -1;
            }

            if (x.isVictory ^ y.isVictory)
            {
                return x.isVictory ? -1 : 1;
            }

            return x.DefaultCoinCost < y.DefaultCoinCost ? -1 :
                   x.DefaultCoinCost > y.DefaultCoinCost ? 1 :
                   0;
        }
    }
}
