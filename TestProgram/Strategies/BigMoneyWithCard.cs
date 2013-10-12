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
        public static class BigMoneyWithCard
        {
            // big money smithy player
            public static PlayerAction Player(Card card, int playerNumber, string playerName = null, int cardCount = 1, int afterSilverCount = 0, int afterGoldCount = int.MaxValue)
            {
                return new PlayerAction(
                            playerName == null ? "BigMoneyWithCard<" + card.GetType().Name + ">" : playerName,
                            playerNumber,
                            purchaseOrder: PurchaseOrder(card, cardCount, afterSilverCount, afterGoldCount));
            }

            public static ICardPicker PurchaseOrder(Card card, int cardCount, int afterSilverCount, int afterGoldCount)
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Province.card, gameState => CountAllOwned(CardTypes.Gold.card, gameState) > 2),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 2),
                           CardAcceptance.For(card, gameState => CountAllOwned(card, gameState) < cardCount && CountAllOwned(CardTypes.Gold.card, gameState) >= afterGoldCount),
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(card, gameState => CountAllOwned(card, gameState) < cardCount && CountAllOwned(CardTypes.Silver.card, gameState) >= afterSilverCount),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 4),
                           CardAcceptance.For(CardTypes.Silver.card, CardAcceptance.AlwaysMatch, CardAcceptance.OverPayZero));
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
                return BigMoneyWithCard.Player(CardTypes.Wharf.card, playerNumber, "BigMoneyWharf");
            }
        }

        public static class BigMoneyBridge
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard.Player(CardTypes.Bridge.card, playerNumber, "BigMoneyBridge");
            }
        }        

        public static class BigMoneySingleSmithy
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard.Player(CardTypes.Smithy.card, playerNumber, "BigMoneySingleSmithy");
            }
        }

        public static class BigMoneySingleWitch
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard.Player(CardTypes.Witch.card, playerNumber, "BigMoneySingleWitch");
            }
        }

        public static class BigMoneyDoubleWitch
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard.Player(CardTypes.Witch.card, playerNumber, "BigMoneyDoubleWitch", cardCount: 2, afterGoldCount: 0);
            }
        }

        public static class BigMoneyMoneylender
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard.Player(CardTypes.Moneylender.card, playerNumber, cardCount: 2);
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
                           CardAcceptance.For(CardTypes.Province.card),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 4),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 2),
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.Thief.card, gameState => CountAllOwned(CardTypes.Thief.card, gameState) < cardCount && CountAllOwned(CardTypes.Silver.card, gameState) >= 2),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 4),
                           CardAcceptance.For(CardTypes.Silver.card));

            }

            private static ICardPicker TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.Silver.card),
                           CardAcceptance.For(CardTypes.Copper.card));
            }
        }


        public static class BigMoneySingleCardCartographer
        {
            // big money smithy player
            public static PlayerAction Player(Card card, int playerNumber, int cardCount = 1)
            {
                return new PlayerAction(
                            "BigMoneySingleCardCartographer",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(card, cardCount),                            
                            actionOrder: ActionOrder(card));
            }

            private static ICardPicker PurchaseOrder(Card card, int cardCount)
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Province.card, gameState => CountAllOwned(CardTypes.Gold.card, gameState) > 2),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 4),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 2),
                           CardAcceptance.For(card, gameState => CountAllOwned(T.card, gameState) < cardCount),
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.Cartographer.card),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 4),
                           CardAcceptance.For(CardTypes.Silver.card));

            }

            private static ICardPicker ActionOrder(Card card)
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Cartographer.card),
                           CardAcceptance.For(card));
            }
        }
    }
}
