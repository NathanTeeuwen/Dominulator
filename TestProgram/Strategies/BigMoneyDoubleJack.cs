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
        public static class BigMoneyDoubleJack
        {
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "BigMoneyDoubleJack",
                            playerNumber,
                            purchaseOrder: PurchaseOrder());
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                           CardAcceptance.For(Cards.Gold),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                           CardAcceptance.For(Cards.JackOfAllTrades, gameState => CountAllOwned(Cards.JackOfAllTrades, gameState) < 1),
                           CardAcceptance.For(Cards.JackOfAllTrades, gameState => CountAllOwned(Cards.JackOfAllTrades, gameState) < 2 && gameState.Self.AllOwnedCards.Count > 15),                           
                           CardAcceptance.For(Cards.Silver));
            }            
        }

        public static class BigMoneyDoubleJackSlog
        {
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "BigMoneyDoubleJackSlog",
                            playerNumber,
                            purchaseOrder: PurchaseOrder());
            }

            private static CardPickByPriority PurchaseOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2),
                           CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                           CardAcceptance.For(Cards.Gold),
                           CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                           CardAcceptance.For(Cards.JackOfAllTrades, gameState => CountAllOwned(Cards.JackOfAllTrades, gameState) < 3),                           
                           CardAcceptance.For(Cards.Silver));
            }
        }
    }
}
