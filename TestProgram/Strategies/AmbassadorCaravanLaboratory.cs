using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    class AmbassadorCaravanLaboratory
        : Strategy    
    {

        public static PlayerAction Player()            
        {
            return PlayerCustom("AmbassadorCaravanLaboratory");
        }

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
                          CardAcceptance.For(Cards.Gold),
                          CardAcceptance.For(Cards.Laboratory),
                          CardAcceptance.For(Cards.Caravan, 5),
                          CardAcceptance.For(Cards.Ambassador, 1),
                          CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 3),                          
                          CardAcceptance.For(Cards.Silver));
        }

        private static ICardPicker ActionOrder()
        {
            return new CardPickByPriority(
                 CardAcceptance.For(Cards.Caravan),
                 CardAcceptance.For(Cards.Laboratory),                 
                 CardAcceptance.For(Cards.Ambassador));
        }
    }
}
