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

            public static PlayerAction Player(int playerNumber, bool shouldBuySpiceMerchant = false, bool shouldBuyPlaza = true)
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
                     CardAcceptance.For<CardTypes.Province>(CardAcceptance.AlwaysMatch, CardAcceptance.OverPayMaxAmount));

                var buildOrder = new CardPickByBuildOrder(
                    shouldBuySpiceMerchant == true ? (Card)new CardTypes.SpiceMerchant() : (Card)new CardTypes.Silver(),
                    new CardTypes.Silver(),
                    new CardTypes.Embassy(),
                    new CardTypes.Tunnel(),
                    new CardTypes.Embassy(),
                    shouldBuyPlaza ? new CardTypes.Plaza() : null);

                var lowPriority = new CardPickByPriority(                    
                           CardAcceptance.For<CardTypes.Duchy>(gameState => gameState.GetPile<CardTypes.Province>().Count() <= 4),                           
                           CardAcceptance.For<CardTypes.Tunnel>(gameState => gameState.players.CurrentPlayer.AllOwnedCards.Count() > 13),                          
                           CardAcceptance.For<CardTypes.Silver>());

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Plaza>(),
                           CardAcceptance.For<CardTypes.SpiceMerchant>(),
                           CardAcceptance.For<CardTypes.Embassy>());
            }

            private static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(                    
                    CardAcceptance.For<CardTypes.Tunnel>(),
                    CardAcceptance.For<CardTypes.Curse>(),
                    CardAcceptance.For<CardTypes.Province>(),
                    CardAcceptance.For<CardTypes.Duchy>(),
                    CardAcceptance.For<CardTypes.Estate>(),
                    CardAcceptance.For<CardTypes.Copper>(),
                    CardAcceptance.For<CardTypes.Silver>(),                    
                    CardAcceptance.For<CardTypes.Gold>());
            }
        }
    }
}
