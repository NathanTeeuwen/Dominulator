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
}
