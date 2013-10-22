using Dominion;
using System.Collections.Generic;

namespace Program.DefaultStrategies
{
    struct CompareCardByFirstToGain
             : IComparer<Card>
    {
        public int Compare(Card x, Card y)
        {
            if (x.isCurse ^ y.isCurse)
            {
                return x.isCurse ? 1 : -1;
            }

            if (x.isRuins ^ y.isRuins)
            {
                return x.isRuins ? 1 : -1;
            }

            if (x.isTreasure ^ y.isTreasure)
            {
                return x.isTreasure ? -1 : 1;
            }

            return x.DefaultCoinCost > y.DefaultCoinCost ? -1 :
                   x.DefaultCoinCost < y.DefaultCoinCost ? 1 :
                   0;
        }
    }
}
