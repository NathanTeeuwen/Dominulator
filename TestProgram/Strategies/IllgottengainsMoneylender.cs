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
        public static class IllgottengainsMoneylender
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("IllgottengainsMoneylender",
                            playerNumber,
                            purchaseOrder: PurchaseOrder())
                {
                }              
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Province>(),
                    CardAcceptance.For<CardTypes.IllGottenGains>(Default.ShouldGainIllGottenGains),
                    CardAcceptance.For<CardTypes.Gold>(gameState => CountOfPile<CardTypes.Province>(gameState) >= 6),
                    CardAcceptance.For<CardTypes.Duchy>(),
                    CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 2));                    

                var buildOrder = new CardPickByBuildOrder(
                    new CardTypes.Moneylender(),
                    new CardTypes.Silver());

                var lowPriority = new CardPickByPriority(                           
                           CardAcceptance.For<CardTypes.Silver>(),
                           CardAcceptance.For<CardTypes.Copper>());

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }            
        }

        public static class Illgottengains
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("Illgottengains",
                            playerNumber,
                            purchaseOrder: PurchaseOrder())
                {
                }
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Province>(),
                    CardAcceptance.For<CardTypes.IllGottenGains>(Default.ShouldGainIllGottenGains),
                    CardAcceptance.For<CardTypes.Gold>(gameState => CountOfPile<CardTypes.Province>(gameState) >= 6),
                    CardAcceptance.For<CardTypes.Duchy>(),
                    CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) < 2));

                var buildOrder = new CardPickByBuildOrder(                    
                    new CardTypes.Silver());

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Silver>(),
                           CardAcceptance.For<CardTypes.Copper>());

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }
        }
    }
}
