using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Strategies
{
    public class BigMoneyTrader
        : Strategy
    {
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "BigMoneyTrader",
                         purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder(),
                        chooseDefaultActionOnNone: false,
                        enablePenultimateProvinceRule:true);
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 5),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 3),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.Trader, 1),
                        CardAcceptance.For(Cards.Silver));
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Trader, ShouldPlayTrader));
        }

        private static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Estate, gameState => !ShouldGainEstate(gameState)),
                        CardAcceptance.For(Cards.Gold, gameState => gameState.CurrentContext.Reason != CardContextReason.CardReacting),
                        CardAcceptance.For(Cards.Silver, gameState => gameState.CurrentContext.Reason != CardContextReason.CardReacting),
                        CardAcceptance.For(Cards.Copper));
        }

        private static bool ShouldGainEstate(GameState gameState)
        {
            return gameState.Self.Actions.ShouldGainCard(gameState, Cards.Estate);
        }

        private static bool ShouldPlayTrader(GameState gameState)
        {
            
            if (gameState.Self.Hand.HasCard(Cards.Estate))
            {
                return !ShouldGainEstate(gameState);            
            }

            if (gameState.Self.ExpectedCoinValueAtEndOfTurn >= 8)
                return false;
            /*
            if (gameState.Self.Actions.ShouldGainCard(gameState, Cards.Duchy) && gameState.Self.ExpectedCoinValueAtEndOfTurn >= 5)
                return false;
            */
            if (gameState.Self.ExpectedCoinValueAtEndOfTurn <= 4)
                return true;
            
            /*
            if (gameState.Self.Actions.ShouldGainCard(gameState, Cards.Province))
                return false;*/

            if (gameState.Self.ExpectedCoinValueAtEndOfTurn <= 3 && !gameState.Self.Hand.HasCard(Cards.Silver))
                return false;

            return true;
        }
    }
}