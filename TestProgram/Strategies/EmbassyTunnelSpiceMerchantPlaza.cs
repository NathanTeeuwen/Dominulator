using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{    
    public class EmbassyTunnelSpiceMerchantPlaza
        : Strategy
    {
        public static PlayerAction Player()
        {
            return CustomPlayer();
        }

        public static PlayerAction CustomPlayer(bool shouldBuySpiceMerchant = false, bool shouldBuyPlaza = true)
        {
            return new MyPlayerAction(shouldBuySpiceMerchant, shouldBuyPlaza);
        }

        class MyPlayerAction
            : PlayerAction
        {
            public MyPlayerAction(bool shouldBuySpiceMerchant, bool shouldBuyPlaza)
                : base("EmbassyTunnelSpiceMerchantPlaza",                            
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
                    CardAcceptance.For(Cards.Province, CardAcceptance.AlwaysMatch, CardAcceptance.OverPayMaxAmount));

            var buildOrder = new CardPickByBuildOrder(
                shouldBuySpiceMerchant == true ? CardAcceptance.For(Cards.SpiceMerchant) : CardAcceptance.For(Cards.Silver),
                CardAcceptance.For(Cards.Silver),
                CardAcceptance.For(Cards.Embassy),
                CardAcceptance.For(Cards.Tunnel),
                CardAcceptance.For(Cards.Embassy),
                shouldBuyPlaza ? CardAcceptance.For(Cards.Plaza) : null);

            var lowPriority = new CardPickByPriority(                    
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Tunnel, gameState => gameState.Self.AllOwnedCards.Count > 13),                          
                        CardAcceptance.For(Cards.Silver));

            return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Plaza),
                        CardAcceptance.For(Cards.SpiceMerchant),
                        CardAcceptance.For(Cards.Embassy));
        }

        private static CardPickByPriority DiscardOrder()
        {
            return new CardPickByPriority(                    
                CardAcceptance.For(Cards.Tunnel),
                CardAcceptance.For(Cards.Curse),
                CardAcceptance.For(Cards.Province),
                CardAcceptance.For(Cards.Duchy),
                CardAcceptance.For(Cards.Estate),
                CardAcceptance.For(Cards.Copper),
                CardAcceptance.For(Cards.Silver),                    
                CardAcceptance.For(Cards.Gold));
        }
    }
}
