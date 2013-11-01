using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{    

    public class BigMoneyWharf
        : Strategy
    {
        public static PlayerAction Player()
        {
            return BigMoneyWithCard.Player(Cards.Wharf, "BigMoneyWharf");
        }
    }

    public class BigMoneyBridge
        : Strategy
    {

        public static PlayerAction Player()
        {
            return BigMoneyWithCard.Player(Cards.Bridge, "BigMoneyBridge");
        }
    }

    public class BigMoneySingleSmithy
        : Strategy
    {

        public static PlayerAction Player()
        {
            return BigMoneyWithCard.Player(Cards.Smithy, "BigMoneySingleSmithy");
        }
    }

    public class BigMoneySingleWitch
        : Strategy
    {

        public static PlayerAction Player()
        {
            return BigMoneyWithCard.Player(Cards.Witch, "BigMoneySingleWitch", afterGoldCount: 0);
        }
    }

    public class BigMoneyDoubleWitch
        : Strategy
    {

        public static PlayerAction Player()
        {
            return BigMoneyWithCard.Player(Cards.Witch, "BigMoneyDoubleWitch", cardCount: 2, afterGoldCount: 0);
        }
    }

    public class BigMoneyMoneylender
        : Strategy
    {

        public static PlayerAction Player()
        {
            return BigMoneyWithCard.Player(Cards.Moneylender);
        }
    }

    public class BigMoneyThief
        : Strategy
    {
        public static PlayerAction Player()
        {
            return BigMoneyWithCard.Player(Cards.Thief);
        }
    }
}