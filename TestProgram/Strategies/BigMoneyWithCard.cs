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
        public static class BigMoneyWithCard<T>
            where T : Card, new()
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber, string playerName = null, int cardCount = 1, int afterSilverCount = 0)
            {
                return new PlayerAction(
                            playerName == null ? "BigMoneyWithCard<" + typeof(T).Name + ">" : playerName,
                            playerNumber,
                            purchaseOrder: PurchaseOrder(cardCount, afterSilverCount),                            
                            actionOrder: ActionOrder());
            }

            public static ICardPicker PurchaseOrder(int cardCount, int afterSilverCount)
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 2),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 2),                           
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<T>(gameState => CountAllOwned<T>(gameState) < cardCount && CountAllOwned<CardTypes.Silver>(gameState) >= afterSilverCount),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 4),
                           CardAcceptance.For<CardTypes.Silver>(CardAcceptance.AlwaysMatch, CardAcceptance.OverPayZero));

            }

            private static ICardPicker ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<T>());
            }
        }
        
        /*
        public static class BigMoneyTorturer
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard<CardTypes.Torturer>.Player(playerNumber, "BigMoneyTorturer");
            }
        }*/

        public static class BigMoneyWharf
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard<CardTypes.Wharf>.Player(playerNumber, "BigMoneyWharf");
            }
        }

        public static class BigMoneyBridge
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard<CardTypes.Bridge>.Player(playerNumber, "BigMoneyBridge");
            }
        }        

        public static class BigMoneySingleSmithy
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard<CardTypes.Smithy>.Player(playerNumber, "BigMoneySingleSmithy");
            }
        }

        public static class BigMoneySingleWitch
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard<CardTypes.Witch>.Player(playerNumber, "BigMoneySingleWitch");
            }
        }

        public static class BigMoneyDoubleWitch
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard<CardTypes.Witch>.Player(playerNumber, "BigMoneyDoubleWitch", cardCount:2);
            }
        }

        public static class BigMoneyMoneylender
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard<CardTypes.Moneylender>.Player(playerNumber, cardCount: 2);
            }
        } 

        public static class BigMoneyWithThief            
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return CustomPlayer(playerNumber);                
            }

            public static PlayerAction CustomPlayer(int playerNumber, int cardCount = 1)
            {
                return new MyPlayerAction(playerNumber, cardCount);                            
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber, int cardCount)
                    : base("Thief",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(cardCount),                            
                            trashOrder: TrashOrder())
                {
                }                
            }

            public static ICardPicker PurchaseOrder(int cardCount)
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) < 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Thief>(gameState => CountAllOwned<CardTypes.Thief>(gameState) < cardCount && CountAllOwned<CardTypes.Silver>(gameState) >= 2),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 4),
                           CardAcceptance.For<CardTypes.Silver>());

            }

            private static ICardPicker TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Silver>(),
                           CardAcceptance.For<CardTypes.Copper>());
            }
        }


        public static class BigMoneySingleCardCartographer<T>
            where T : Card, new()
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber, int cardCount = 1)
            {
                return new PlayerAction(
                            "BigMoneySingleCardCartographer",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(cardCount),
                            treasurePlayOrder: Default.TreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: Default.EmptyPickOrder(),
                            discardOrder: Default.EmptyPickOrder());
            }

            private static ICardPicker PurchaseOrder(int cardCount)
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 2),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) < 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 2),
                           CardAcceptance.For<T>(gameState => CountAllOwned<T>(gameState) < cardCount),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Cartographer>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 4),
                           CardAcceptance.For<CardTypes.Silver>());

            }

            private static ICardPicker ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Cartographer>(),
                           CardAcceptance.For<T>());
            }
        }
    }
}
