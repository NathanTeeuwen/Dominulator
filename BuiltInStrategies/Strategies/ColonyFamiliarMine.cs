using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    namespace Strategies
    {
        public class ColonyFamiliarMine
            : Strategy
        {

            public static PlayerAction Player()
            {
                return new PlayerAction(
                            "ColonyFamiliarMine",
                            purchaseOrder: PurchaseOrder());
            }

            public static ICardPicker PurchaseOrder()
            {
                return new CardPickByPriority(
                            CardAcceptance.For(Cards.Colony),
                            CardAcceptance.For(Cards.Platinum),
                            CardAcceptance.For(Cards.Gold),
                            CardAcceptance.For(Cards.Familiar, 3),
                            CardAcceptance.For(Cards.Potion, 1),
                            CardAcceptance.For(Cards.Mine, 2),
                            CardAcceptance.For(Cards.Silver));
            }
        }
    }

}
