using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    public class AmbassadorCaravanApprenticeMerchantGuild
        : Strategy    
    {

        public static PlayerAction Player()            
        {
            return PlayerCustom("AmbassadorCaravanApprenticeMerchantGuild");
        }

        public static PlayerAction PlayerCustom(string playerName, bool shouldApprentice = false)
        {
            return new PlayerAction(
                        playerName,
                        purchaseOrder: PurchaseOrder(shouldApprentice),
                        actionOrder: ActionOrder());
        }

        private static ICardPicker PurchaseOrder(bool shouldApprentice)
        {
            return new CardPickByPriority(
                          CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) >= 2),
                          CardAcceptance.For(Cards.Duchy, gameState => CountAllOwned(Cards.Province, gameState) > 1 && CountOfPile(Cards.Province, gameState) <= 4),
                          CardAcceptance.For(Cards.Estate, gameState => CountAllOwned(Cards.Province, gameState) > 1 && CountOfPile(Cards.Province, gameState) <= 2),
                          CardAcceptance.For(Cards.Apprentice, gameState => shouldApprentice && CountAllOwned(Cards.Apprentice, gameState) < 1 && CountAllOwned(Cards.Copper, gameState) + CountAllOwned(Cards.Estate, gameState) > 6),          
                          CardAcceptance.For(Cards.Gold),
                          CardAcceptance.For(Cards.MerchantGuild, 1),
                          CardAcceptance.For(Cards.Caravan, 5),
                          CardAcceptance.For(Cards.Ambassador, 1),
                          CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 3),                          
                          CardAcceptance.For(Cards.Silver));
        }

        private static ICardPicker ActionOrder()
        {
            return new CardPickByPriority(
                 CardAcceptance.For(Cards.Caravan),
                 CardAcceptance.For(Cards.Apprentice),
                 CardAcceptance.For(Cards.MerchantGuild),
                 CardAcceptance.For(Cards.Ambassador));
        }
    }
}
