using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CardTypes = Dominion.CardTypes;
using Dominion;

namespace Program
{
    class StartingDeckBuilder
    {
        protected static IEnumerable<CardCountPair>[] StartingDecks(params IEnumerable<CardCountPair>[] result)
        {
            return result;
        }

        protected static IEnumerable<CardCountPair> StartingDeck(params CardCountPair[] result)
        {
            return result;
        }

        protected static CardCountPair CardWithCount(Card card, int count)
        {
            return new CardCountPair(card, count);
        }
    }
}
