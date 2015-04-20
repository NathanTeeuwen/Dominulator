using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    // for the set
    // cultist, monument, venture, oasis, tunnel, forager, mine, rogue, peddler
    public class OasisTunnelForagerMineVenture
        : Strategy    
    {

        public static PlayerAction Player()      
        {
            return PlayerCustom("OasisTunnelForagerMineVenture");
        }

        public static PlayerAction PlayerCustom(string playerName)
        {
            return new PlayerAction(
                        playerName,
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(),
                        discardOrder: DiscardOrder(),
                        trashOrder: TrashOrder(),
                        enablePenultimateProvinceRule:true);
        }

        private static ICardPicker PurchaseOrder()
        {
            return new CardPickByPriority(
                          CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 1),
                          CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                          CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                          //CardAcceptance.For(Cards.Mine, 1),
                          CardAcceptance.For(Cards.Venture),
                          //CardAcceptance.For(Cards.Silver, 1),
                          CardAcceptance.For(Cards.Forager, 2, ShouldGainForager),          
                          CardAcceptance.For(Cards.Oasis, 1),
                          CardAcceptance.For(Cards.Tunnel, 1),
                          //CardAcceptance.For(Cards.Forager, 3, ShouldGainForager),
                          CardAcceptance.For(Cards.Oasis, 2),
                          CardAcceptance.For(Cards.Tunnel, 2),
                          CardAcceptance.For(Cards.Silver));
        }

        private static ICardPicker DiscardOrder()
        {
            return new CardPickByPriority(
                 CardAcceptance.For(Cards.Tunnel));
        }

        private static ICardPicker TrashOrder()
        {
            return new CardPickByPriority(
                 CardAcceptance.For(Cards.Curse),                
                 CardAcceptance.For(Cards.RuinedVillage),
                 CardAcceptance.For(Cards.RuinedMarket),
                 CardAcceptance.For(Cards.RuinedLibrary),
                 CardAcceptance.For(Cards.Survivors),
                 CardAcceptance.For(Cards.AbandonedMine),
                 CardAcceptance.For(Cards.Estate),
                 CardAcceptance.For(Cards.Copper),
                 CardAcceptance.For(Cards.Forager, ShouldTrashForager));
        }

        private static bool ShouldTrashForager(GameState gameState)
        {
            if (gameState.Self.ExpectedCoinValueAtEndOfTurn == 7)
                return true;

            return Strategy.CountAllOwned(Cards.Forager, gameState)+1 >=
                    Strategy.CountAllOwned(Cards.Curse, gameState) +
                    Strategy.CountAllOwned(Cards.Estate, gameState) +
                    Strategy.CountAllOwned(Cards.Copper, gameState) + 
                    Strategy.CountAllOwned(Cards.AbandonedMine, gameState) + 
                    Strategy.CountAllOwned(Cards.RuinedLibrary, gameState) + 
                    Strategy.CountAllOwned(Cards.RuinedMarket, gameState) + 
                    Strategy.CountAllOwned(Cards.RuinedVillage, gameState) + 
                    Strategy.CountAllOwned(Cards.Survivors, gameState);
        }

        private static bool ShouldGainForager(GameState gameState)
        {            
            return Strategy.CountAllOwned(Cards.Forager, gameState)+1 <
                    Strategy.CountAllOwned(Cards.Curse, gameState) +
                    Strategy.CountAllOwned(Cards.Estate, gameState) +
                    Strategy.CountAllOwned(Cards.Copper, gameState) +
                    Strategy.CountAllOwned(Cards.AbandonedMine, gameState) +
                    Strategy.CountAllOwned(Cards.RuinedLibrary, gameState) +
                    Strategy.CountAllOwned(Cards.RuinedMarket, gameState) +
                    Strategy.CountAllOwned(Cards.RuinedVillage, gameState) +
                    Strategy.CountAllOwned(Cards.Survivors, gameState);
        }

        private static ICardPicker ActionOrder()
        {
            return new CardPickByPriority(
                 CardAcceptance.For(Cards.Oasis),
                 CardAcceptance.For(Cards.Forager),
                 CardAcceptance.For(Cards.AbandonedMine),
                 CardAcceptance.For(Cards.Survivors),
                 CardAcceptance.For(Cards.RuinedLibrary),
                 CardAcceptance.For(Cards.Mine));
        }
    }
}
