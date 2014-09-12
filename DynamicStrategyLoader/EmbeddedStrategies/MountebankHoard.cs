using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    public class MountebankHoard
        : Strategy
    {
        public static PlayerAction Player()
        {
            return new MyPlayerAction();
        }

        class MyPlayerAction
            : PlayerAction
        {
            public MyPlayerAction()
                : base("MountebankHoard",
                    purchaseOrder: PurchaseOrder(),
                    actionOrder: ActionOrder(),
                    discardOrder: DiscardOrder())
            {
            }
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy, gameState => gameState.Self.CardsInPlay.HasCard(Cards.Hoard)),
                        CardAcceptance.For(Cards.Estate, gameState => gameState.Self.CardsInPlay.HasCard(Cards.Hoard)),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 7 || CountOfPileLessthanEqual(Cards.Vineyard, gameState, 7)),
                        CardAcceptance.For(Cards.Hamlet, gameState => CountOfPile(Cards.Duchy, gameState) == 0 && CountOfPile(Cards.Curse, gameState) == 0),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 3 || CountOfPileLessthanEqual(Cards.Vineyard, gameState, 3) || CountOfPile(Cards.Duchy, gameState) <= 2),
                        CardAcceptance.For(Cards.GrandMarket),
                        CardAcceptance.For(Cards.Mountebank, 2),
                        CardAcceptance.For(Cards.Hoard),
                        CardAcceptance.For(Cards.Monument, 1),
                        CardAcceptance.For(Cards.Silver),
                        CardAcceptance.For(Cards.Hamlet));
        }

        private static bool CountOfPileLessthanEqual(Card cardType, GameState gameState, int count)
        {
            if (gameState.GetSupplyPile(cardType) == null)
                return true;

            return CountOfPile(cardType, gameState) <= count;
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.GrandMarket),
                        CardAcceptance.For(Cards.Hamlet),
                        CardAcceptance.For(Cards.Mountebank),
                        CardAcceptance.For(Cards.Monument));
        }

        private static CardPickByPriority DiscardOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy),
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.Curse));
        }
    }
}