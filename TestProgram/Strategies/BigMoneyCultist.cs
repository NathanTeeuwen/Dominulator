using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Author: SheCantSayNo

namespace Program
{
    public static partial class Strategies
    {
        public static class BigMoneyCultist
        {

            public static PlayerAction Player(int playerNumber)
            {
                return CustomPlayer(playerNumber);
            }

            public static PlayerAction CustomPlayer(int playerNumber, int secondCultist = 15)
            {
                return new PlayerAction(
                            "BigMoneyCultist",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(secondCultist));
            }

            private static CardPickByPriority PurchaseOrder(int secondCultist)
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Province.card, gameState => CountAllOwned(CardTypes.Gold.card, gameState) > 1),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 5),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 3),
                           CardAcceptance.For(CardTypes.Cultist.card, gameState => CountAllOwned(CardTypes.Cultist.card, gameState) < 3),
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.Cultist.card),
                           CardAcceptance.For(CardTypes.Silver.card));
            }            
        }     
    }
}