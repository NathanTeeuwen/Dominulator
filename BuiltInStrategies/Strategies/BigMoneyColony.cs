using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    public class BigMoneyColony
        : Strategy
    {
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "BigMoneyColony",
                        purchaseOrder: PurchaseOrder());
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Colony, gameState => CountAllOwned(Cards.Platinum, gameState) > 2),
                        CardAcceptance.For(Cards.Duchy, gameState => GainsUntilEndGame(gameState) <= 2),
                        CardAcceptance.For(Cards.Estate, gameState => GainsUntilEndGame(gameState) <= 2),
                        CardAcceptance.For(Cards.Platinum),
                        CardAcceptance.For(Cards.Province, gameState => GainsUntilEndGame(gameState) <= 2),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Estate, gameState => GainsUntilEndGame(gameState) < 4),
                        CardAcceptance.For(Cards.Silver));
        }

        private static int GainsUntilEndGame(GameState gameState)
        {
            int result = Math.Min(CountOfPile(Cards.Colony, gameState), CountOfPile(Cards.Province, gameState));
            return result;
        }
    }
}
