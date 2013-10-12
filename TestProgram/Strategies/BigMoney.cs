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
        public static class BigMoney
        {
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "BigMoney",
                            playerNumber,
                            purchaseOrder: PurchaseOrder());
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 2),                           
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 4),                           
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 4),
                           CardAcceptance.For<CardTypes.Silver>());
            }
        }

        public static class BigMoneyColony
        {
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "BigMoneyColony",
                            playerNumber,
                            purchaseOrder: PurchaseOrder());
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Colony>(gameState => CountAllOwned<CardTypes.Platinum>(gameState) > 2),                           
                           CardAcceptance.For<CardTypes.Duchy>(gameState => GainsUntilEndGame(gameState) <= 2),
                           CardAcceptance.For<CardTypes.Estate>(gameState => GainsUntilEndGame(gameState) <= 2),
                           CardAcceptance.For<CardTypes.Platinum>(),
                           CardAcceptance.For<CardTypes.Province>(gameState => GainsUntilEndGame(gameState) <= 2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => GainsUntilEndGame(gameState) < 4),
                           CardAcceptance.For<CardTypes.Silver>());
            }

            private static int GainsUntilEndGame(GameState gameState)
            {
                int result = Math.Min(CountOfPile<CardTypes.Colony>(gameState), CountOfPile<CardTypes.Province>(gameState));
                return result;
            }
        }           
    }

    public static partial class Strategies
    {
        public static class BigMoneySmithyEarlyProvince
        {
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "BigMoneySmithyEarlyProvince",
                            playerNumber,
                            purchaseOrder: PurchaseOrder());
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 0),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 5),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Smithy>(gameState => CountAllOwned<CardTypes.Smithy>(gameState) < 1),
                           CardAcceptance.For<CardTypes.Silver>());
            }
        }
    }

    public static partial class Strategies
    {
        public static class BigMoneyCouncilRoomEarlyProvince
        {
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "BigMoneyCouncilRoomEarlyProvince",
                            playerNumber,
                            purchaseOrder: PurchaseOrder());
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 0),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 5),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 2),
                           CardAcceptance.For<CardTypes.Gold>(),                           
                           CardAcceptance.For<CardTypes.CouncilRoom>(gameState => CountAllOwned<CardTypes.CouncilRoom>(gameState) < 2),
                           CardAcceptance.For<CardTypes.Silver>());
            }
        }      
    }
}
