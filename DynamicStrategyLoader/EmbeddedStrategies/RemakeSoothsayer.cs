using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{    
    public class RemakeSoothsayer
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
                : base("RemakeSoothsayer",
                    purchaseOrder: PurchaseOrder(),
                    actionOrder: ActionOrder(),
                    trashOrder: TrashOrder())
            {
            }
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Soothsayer, 1),
                        CardAcceptance.For(Cards.Gold, 1),
                        CardAcceptance.For(Cards.Soothsayer, 2),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Duchy, gameState => CardBeingPlayedIs(Cards.Remake, gameState)),
                        CardAcceptance.For(Cards.Remake, 1),
                //CardAcceptance.For(Cards.Remake, gameState => ((double)CountAllOwned(TrashOrderWithoutRemake(), gameState)) / gameState.Self.AllOwnedCards.Count > 0.4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.GreatHall, gameState => CountAllOwned(Cards.Silver, gameState) >= 2),
                        CardAcceptance.For(Cards.Silver)
                        );
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.GreatHall),
                        CardAcceptance.For(Cards.Soothsayer),
                        CardAcceptance.For(Cards.Remake, ShouldPlayRemake));
        }

        private static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                    CardAcceptance.For(Cards.Curse),
                    CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) > 2),
                    CardAcceptance.For(Cards.Remake),
                    CardAcceptance.For(Cards.Copper));
        }

        private static CardPickByPriority TrashOrderWithoutRemake()
        {
            return new CardPickByPriority(
                    CardAcceptance.For(Cards.Curse),
                    CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) > 2),
                    CardAcceptance.For(Cards.Copper));
        }

        private static bool ShouldPlayRemake(GameState gameState)
        {
            return CountInHandFrom(TrashOrderWithoutRemake(), gameState) >= 2;
        }
    }
}
