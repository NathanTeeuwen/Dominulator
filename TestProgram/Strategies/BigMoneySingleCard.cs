using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public static partial class Strategies
    {
        public static class BigMoneySingleCard<T>
            where T : Card, new()
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber, int cardCount = 1)
            {
                return new PlayerAction(playerNumber,
                            purchaseOrder: PurchaseOrder(cardCount),
                            treasurePlayOrder: Default.TreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: Default.EmptyPickOrder(),
                            discardOrder: Default.EmptyPickOrder());
            }

            private static IGetMatchingCard PurchaseOrder(int cardCount)
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 2),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 2),
                           CardAcceptance.For<T>(gameState => CountAllOwned<T>(gameState) < cardCount),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 4),
                           CardAcceptance.For<CardTypes.Silver>());

            }

            private static IGetMatchingCard ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<T>());
            }
        }

        public static class BigMoneySingleCardCartographer<T>
            where T : Card, new()
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber, int cardCount = 1)
            {
                return new PlayerAction(playerNumber,
                            purchaseOrder: PurchaseOrder(cardCount),
                            treasurePlayOrder: Default.TreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: Default.EmptyPickOrder(),
                            discardOrder: Default.EmptyPickOrder());
            }

            private static IGetMatchingCard PurchaseOrder(int cardCount)
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 2),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 2),
                           CardAcceptance.For<T>(gameState => CountAllOwned<T>(gameState) < cardCount),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Cartographer>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => gameState.GetPile<CardTypes.Province>().Count() < 4),
                           CardAcceptance.For<CardTypes.Silver>());

            }

            private static IGetMatchingCard ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Cartographer>(),
                           CardAcceptance.For<T>());
            }
        }
    }
}
