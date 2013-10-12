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
        public static class EmbassyTunnelSpiceMerchantPlaza
        {
            public static PlayerAction Player(int playerNumber)
            {
                return CustomPlayer(playerNumber);
            }

            public static PlayerAction CustomPlayer(int playerNumber, bool shouldBuySpiceMerchant = false, bool shouldBuyPlaza = true)
            {
                return new MyPlayerAction(playerNumber, shouldBuySpiceMerchant, shouldBuyPlaza);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber, bool shouldBuySpiceMerchant, bool shouldBuyPlaza)
                    : base("EmbassyTunnelSpiceMerchantPlaza",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(shouldBuySpiceMerchant, shouldBuyPlaza),                            
                            actionOrder: ActionOrder(),                            
                            discardOrder: DiscardOrder())
                {
                }

                public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    return PlayerActionChoice.PlusCard;
                }
            }                       

            private static ICardPicker PurchaseOrder(bool shouldBuySpiceMerchant, bool shouldBuyPlaza)
            {
                var highPriority = new CardPickByPriority(                     
                     CardAcceptance.For(CardTypes.Province.card, CardAcceptance.AlwaysMatch, CardAcceptance.OverPayMaxAmount));

                var buildOrder = new CardPickByBuildOrder(
                    shouldBuySpiceMerchant == true ? CardAcceptance.For(CardTypes.SpiceMerchant.card) : CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Embassy.card),
                    CardAcceptance.For(CardTypes.Tunnel.card),
                    CardAcceptance.For(CardTypes.Embassy.card),
                    shouldBuyPlaza ? CardAcceptance.For(CardTypes.Plaza.card) : null);

                var lowPriority = new CardPickByPriority(                    
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4),
                           CardAcceptance.For(CardTypes.Tunnel.card, gameState => gameState.Self.AllOwnedCards.Count > 13),                          
                           CardAcceptance.For(CardTypes.Silver.card));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Plaza.card),
                           CardAcceptance.For(CardTypes.SpiceMerchant.card),
                           CardAcceptance.For(CardTypes.Embassy.card));
            }

            private static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(                    
                    CardAcceptance.For(CardTypes.Tunnel.card),
                    CardAcceptance.For(CardTypes.Curse.card),
                    CardAcceptance.For(CardTypes.Province.card),
                    CardAcceptance.For(CardTypes.Duchy.card),
                    CardAcceptance.For(CardTypes.Estate.card),
                    CardAcceptance.For(CardTypes.Copper.card),
                    CardAcceptance.For(CardTypes.Silver.card),                    
                    CardAcceptance.For(CardTypes.Gold.card));
            }
        }
    }
}
