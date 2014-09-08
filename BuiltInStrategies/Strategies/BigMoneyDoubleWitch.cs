using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    public class BigMoneyDoubleWitch
    : Strategy
    {
        public static PlayerAction Player()
        {
            return BigMoneyWithCard.Player(Cards.Witch, "BigMoneyDoubleWitch", cardCount: 2, afterGoldCount: 0);
        }
    }

}
