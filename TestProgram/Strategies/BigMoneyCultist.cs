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
                           CardAcceptance.For<CardTypes.Cultist>(gameState => CountOfPile<CardTypes.Ruins>(gameState) > 2),
                           CardAcceptance.For<CardTypes.Province>(gameState => CountAllOwned<CardTypes.Gold>(gameState) > 1),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 5),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 3),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Cultist>(),
                           CardAcceptance.For<CardTypes.Silver>());
            }            
        }
    }
}