using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;

namespace Strategies
{
    /*
     * 
     * http://dominionstrategy.com/2012/07/30/building-the-first-game-engine/
     */
    public class FirstGame
        : Strategy
    {
            
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "FirstGame",                            
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder(),                            
                        chooseDefaultActionOnNone:false);
        }

        private static ICardPicker PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => gameState.Self.AvailableBuys > 1 && gameState.Self.AvailableCoins >=16),
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Province, gameState) > 0),
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Smithy, gameState) >= 5),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 3),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        //CardAcceptance.For(Cards.Smithy, gameState => gameState.Self.AvailableBuys > 1 && gameState.Self.AvailableCoins > 6),
                        CardAcceptance.For(Cards.Market, 1),
                        CardAcceptance.For(Cards.Gold, 2),
                        CardAcceptance.For(Cards.Market, 5),
                        CardAcceptance.For(Cards.Remodel, 1),
                        CardAcceptance.For(Cards.Village, gameState => CountAllOwned(Cards.Village, gameState) < 
                                                                       CountAllOwned(Cards.Remodel, gameState) +
                                                                       CountAllOwned(Cards.Militia, gameState) +
                                                                       CountAllOwned(Cards.Smithy, gameState)),
                        CardAcceptance.For(Cards.Militia, 1),
                        CardAcceptance.For(Cards.Smithy),
                        CardAcceptance.For(Cards.Village),                        
                        CardAcceptance.For(Cards.Cellar, 2));
        }

        private static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Market),
                        CardAcceptance.For(Cards.Village),
                        CardAcceptance.For(Cards.Cellar, gameState => gameState.Self.AvailableActions == 0),
                        CardAcceptance.For(Cards.Smithy, gameState => gameState.Self.AvailableActions > 0),
                        CardAcceptance.For(Cards.Militia),
                        CardAcceptance.For(Cards.Remodel),                        
                        CardAcceptance.For(Cards.Smithy),
                        CardAcceptance.For(Cards.Cellar));
        }

        private static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Curse),
                        CardAcceptance.For(Cards.Gold, gameState => CountOfPile(Cards.Province, gameState) <= 3),
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.Copper));
        }
    }
}
