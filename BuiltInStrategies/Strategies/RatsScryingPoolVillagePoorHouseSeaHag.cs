using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Strategies
{
    public class RatsScryingPoolVillagePoorHouseSeahag
            : Strategy
    {
        public static PlayerAction Player()
        {
            return new PlayerAction("RatsScryingPoolVillagePoorHouseSeahag",
                    purchaseOrder: PurchaseOrder(),
                    actionOrder: ActionOrder(),
                    trashOrder: TrashOrder());
        }

        private static ICardPicker PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Potion, 1),
                        CardAcceptance.For(Cards.Rats, 1),
                        CardAcceptance.For(Cards.SeaHag, 1),                        
                        CardAcceptance.For(Cards.ScryingPool, 5),
                        CardAcceptance.For(Cards.PoorHouse, 1),
                        CardAcceptance.For(Cards.Village, 1),
                        CardAcceptance.For(Cards.PoorHouse, 2),
                        CardAcceptance.For(Cards.Village));
        }

        private static ICardPicker ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.ScryingPool),
                        CardAcceptance.For(Cards.Village),
                        CardAcceptance.For(Cards.Rats),
                        CardAcceptance.For(Cards.SeaHag, gameState => CountOfPile(Cards.Curse, gameState) > 0),                        
                        CardAcceptance.For(Cards.PoorHouse));
        }

        private static ICardPicker TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Curse),
                        CardAcceptance.For(Cards.Potion, ShouldTrashPotion),                                               
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.Copper));
        }

        private static bool ShouldTrashPotion(GameState gameState)
        {
            if (!gameState.Self.Actions.ShouldGainCard(gameState, Cards.ScryingPool))
                return true;
            
            return false;
        }    
    }
}