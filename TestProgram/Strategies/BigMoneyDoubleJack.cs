using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

namespace Strategies
{    
    public class BigMoneyDoubleJack
        : Strategy 
    {
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "BigMoneyDoubleJack",                            
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

    public class BigMoneyDoubleJackSlog
        : Strategy
    {
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "BigMoneyDoubleJackSlog",                            
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
