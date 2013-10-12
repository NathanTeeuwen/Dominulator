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
        public static class BigMoneySingleJack
        {
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "BigMoneySingleJack",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            actionOrder: ActionOrder());
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
                           CardAcceptance.For(Cards.Silver));
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.JackOfAllTrades));
            }
        }
    }
}
