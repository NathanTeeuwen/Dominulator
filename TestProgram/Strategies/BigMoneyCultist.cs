using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

//Author: SheCantSayNo

namespace Strategies
{    
    public class BigMoneyCultist
        : Strategy 
    {

        public static PlayerAction Player()
        {
            return CustomPlayer();
        }

        public static PlayerAction CustomPlayer(int secondCultist = 15)
        {
            return new PlayerAction(
                        "BigMoneyCultist",                            
                        purchaseOrder: PurchaseOrder(secondCultist));
        }

        private static CardPickByPriority PurchaseOrder(int secondCultist)
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 1),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 5),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 3),
                        CardAcceptance.For(Cards.Cultist, gameState => CountAllOwned(Cards.Cultist, gameState) < 3),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Cultist),
                        CardAcceptance.For(Cards.Silver));
        }            
    }     
}