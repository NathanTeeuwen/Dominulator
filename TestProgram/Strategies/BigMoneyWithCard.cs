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
                           CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                           CardAcceptance.For(card, gameState => CountAllOwned(card, gameState) < cardCount && CountAllOwned(Cards.Gold, gameState) >= afterGoldCount),
                           CardAcceptance.For(Cards.Gold),
                           CardAcceptance.For(card, gameState => CountAllOwned(card, gameState) < cardCount && CountAllOwned(Cards.Silver, gameState) >= afterSilverCount),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                           CardAcceptance.For(Cards.Silver, CardAcceptance.AlwaysMatch, CardAcceptance.OverPayZero));
            }           
        }
        
        /*
        public static class BigMoneyTorturer
        {
            
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard<CardTypes.Torturer>.Player(playerNumber, "BigMoneyTorturer");
            }
        }*/

        public static class BigMoneyWharf
        {
            
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard.Player(Cards.Wharf, playerNumber, "BigMoneyWharf");
            }
        }

        public static class BigMoneyBridge
        {
            
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard.Player(Cards.Bridge, playerNumber, "BigMoneyBridge");
            }
        }        

        public static class BigMoneySingleSmithy
        {
            
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard.Player(Cards.Smithy, playerNumber, "BigMoneySingleSmithy");
            }
        }

        public static class BigMoneySingleWitch
        {
            
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard.Player(Cards.Witch, playerNumber, "BigMoneySingleWitch");
            }
        }

        public static class BigMoneyDoubleWitch
        {
            
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard.Player(Cards.Witch, playerNumber, "BigMoneyDoubleWitch", cardCount: 2, afterGoldCount: 0);
            }
        }

        public static class BigMoneyMoneylender
        {
            
            public static PlayerAction Player(int playerNumber)
            {
                return BigMoneyWithCard.Player(Cards.Moneylender, playerNumber, cardCount: 2);
            }
        } 

        public static class BigMoneyWithThief            
        {
            
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
                           CardAcceptance.For(Cards.Province),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) < 4),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                           CardAcceptance.For(Cards.Gold),
                           CardAcceptance.For(Cards.Thief, gameState => CountAllOwned(Cards.Thief, gameState) < cardCount && CountAllOwned(Cards.Silver, gameState) >= 2),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                           CardAcceptance.For(Cards.Silver));

            }

            private static ICardPicker TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Gold),
                           CardAcceptance.For(Cards.Silver),
                           CardAcceptance.For(Cards.Copper));
            }
        }


        public static class BigMoneySingleCardCartographer
        {
            
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
                           CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) < 4),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                           CardAcceptance.For(card, gameState => CountAllOwned(card, gameState) < cardCount),
                           CardAcceptance.For(Cards.Gold),
                           CardAcceptance.For(Cards.Cartographer),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                           CardAcceptance.For(Cards.Silver));

            }

            private static ICardPicker ActionOrder(Card card)
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Cartographer),
                           CardAcceptance.For(card));
            }
        }
    }
}
