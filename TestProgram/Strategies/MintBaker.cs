using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    class MintBaker
        : Strategy    
    {

        public static PlayerAction Player()                        
        {
            return PlayerCustom("MintBaker");
        }

        /*
        public static PlayerAction Player(string playerName)
        {
            return PlayerCustom(playerName);
        }*/

        public static PlayerAction PlayerCustom(string playerName)
        {
            return new PlayerAction(
                        playerName,
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder());
        }

        private static ICardPicker PurchaseOrder()
        {
            return new CardPickByPriority(
                          CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) >= 2),
                          CardAcceptance.For(Cards.Duchy, gameState => CountAllOwned(Cards.Province, gameState) > 1 && CountOfPile(Cards.Province, gameState) <= 4),
                          CardAcceptance.For(Cards.Estate, gameState => CountAllOwned(Cards.Province, gameState) > 1 && CountOfPile(Cards.Province, gameState) <= 2),                                                    
                          CardAcceptance.For(Cards.Mint, 1),
                          CardAcceptance.For(Cards.Gold),
                          CardAcceptance.For(Cards.Baker, 0),                          
                          CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 3),                          
                          CardAcceptance.For(Cards.Silver));
        }

        private static ICardPicker ActionOrder()
        {
            return new CardPickByPriority(
                 CardAcceptance.For(Cards.Baker),
                 CardAcceptance.For(Cards.Mint));
        }
    }
}
