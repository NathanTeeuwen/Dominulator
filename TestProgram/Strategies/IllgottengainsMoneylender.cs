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
                    CardAcceptance.For(CardTypes.Province.card),
                    CardAcceptance.For(CardTypes.IllGottenGains.card, Default.ShouldGainIllGottenGains),
                    CardAcceptance.For(CardTypes.Gold.card, gameState => CountOfPile(CardTypes.Province.card, gameState) >= 6),
                    CardAcceptance.For(CardTypes.Duchy.card),
                    CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 2));                    

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(CardTypes.Moneylender.card),
                    CardAcceptance.For(CardTypes.Silver.card));

                var lowPriority = new CardPickByPriority(                           
                           CardAcceptance.For(CardTypes.Silver.card),
                           CardAcceptance.For(CardTypes.Copper.card));

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
                    CardAcceptance.For(CardTypes.Province.card),
                    CardAcceptance.For(CardTypes.IllGottenGains.card, Default.ShouldGainIllGottenGains),
                    CardAcceptance.For(CardTypes.Gold.card, gameState => CountOfPile(CardTypes.Province.card, gameState) >= 6),
                    CardAcceptance.For(CardTypes.Duchy.card),
                    CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) < 2));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(CardTypes.Silver.card));

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Silver.card),
                           CardAcceptance.For(CardTypes.Copper.card));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }
        }
    }
}
