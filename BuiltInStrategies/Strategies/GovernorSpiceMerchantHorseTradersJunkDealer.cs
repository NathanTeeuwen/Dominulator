using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;

namespace Strategies
{
    /*
     * 
     * 
     */
    public class GovernorSpiceMerchantHorseTraderJunkDealer
        : Strategy
    {
            
        public static PlayerAction Player()
        {
            return PlayerCustom(shouldHorseTradersFirst: true);
        }

        public static PlayerAction PlayerCustom(bool shouldHorseTradersFirst)
        {
            return new PlayerAction(
                        "GovernorSpiceMerchantHorseTraderJunkDealer" + (shouldHorseTradersFirst ? "(ht first)" : "(sm first)"),
                        purchaseOrder: PurchaseOrder(shouldHorseTradersFirst),
                        actionOrder: ActionOrder(),
                        //trashOrder: TrashOrder(),
                        chooseDefaultActionOnNone: false);
        }

        private static ICardPicker PurchaseOrder(bool shouldHorseTradersFirst)
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) >= 2 || CountAllOwned(Cards.Province, gameState) > 0),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.JunkDealer, 1),
                        CardAcceptance.For(Cards.Governor),
                        CardAcceptance.For(Cards.Gold),                        
                        CardAcceptance.For(Cards.HorseTraders, 1, gameState => shouldHorseTradersFirst),
                        CardAcceptance.For(Cards.SpiceMerchant, 1),
                        CardAcceptance.For(Cards.HorseTraders, 1, gameState => !shouldHorseTradersFirst),
                        CardAcceptance.For(Cards.Silver));
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Governor),
                        CardAcceptance.For(Cards.SpiceMerchant),
                        CardAcceptance.For(Cards.JunkDealer),
                        CardAcceptance.For(Cards.HorseTraders));
        }

        private static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Curse),
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.Copper),
                        CardAcceptance.For(Cards.SpiceMerchant, gameState => CountAllOwned(Cards.Copper, gameState) <= 2));
        }
    }
}
