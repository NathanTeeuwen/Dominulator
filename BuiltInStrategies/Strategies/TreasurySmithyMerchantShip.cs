using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

//Author: SheCantSayNo

namespace Strategies
{    
    public class TreasurySmithyMerchantShip
        : Strategy 
    {

        public static PlayerAction Player()
        {
            return CustomPlayer();
        }

        public static PlayerAction CustomPlayer()
        {
            return new PlayerAction(
                        "TreasurySmithyMrchantShip",                            
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(),
                        enablePenultimateProvinceRule:true);
        }

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 1),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 3),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Treasury, 2),                        
                        CardAcceptance.For(Cards.MerchantShip, 1),
                        //CardAcceptance.For(Cards.Silver, 2),
                        CardAcceptance.For(Cards.Smithy, 1),
                        CardAcceptance.For(Cards.Silver));
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Treasury),
                        CardAcceptance.For(Cards.MerchantShip),
                        CardAcceptance.For(Cards.Smithy));
        }            
    }     
}