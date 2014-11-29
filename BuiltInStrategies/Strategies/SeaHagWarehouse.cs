using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{
    public class SeaHagWareHouse
        : Strategy
    {
        public static PlayerAction Player(bool shouldOpenSilver, bool oneSilverBeforeWarehouse)
        {
            return new PlayerAction(
                        (shouldOpenSilver ? "SilverFirst" : "") +
                        (oneSilverBeforeWarehouse ? "WareHousePrefer" : "") +
                        "SeaHagWareHouse",
                        purchaseOrder: PurchaseOrder(shouldOpenSilver, oneSilverBeforeWarehouse),
                        discardOrder: DiscardOrder());
        }

        public static ICardPicker PurchaseOrder(bool shouldOpenSilver, bool oneSilverBeforeWarehouse)
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),                        
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.SeaHag, 1),
                        CardAcceptance.For(Cards.Silver, 1, gameState => shouldOpenSilver),
                        CardAcceptance.For(Cards.Warehouse, 1, gameState => !shouldOpenSilver),
                        CardAcceptance.For(Cards.Silver, 1, gameState => oneSilverBeforeWarehouse),
                        CardAcceptance.For(Cards.Warehouse, 1),                        
                        CardAcceptance.For(Cards.Warehouse, ShouldGainWareHouse),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                        CardAcceptance.For(Cards.Silver));
        }

        public static ICardPicker DiscardOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Warehouse, ShouldDiscardWareHouse));
        }

        public static bool ShouldGainWareHouse(GameState gameState)
        {
            return ((double)CountAllOwned(Cards.Warehouse, gameState)) / gameState.Self.AllOwnedCards.Count < (double)1 / 12;
        }

        public static bool ShouldDiscardWareHouse(GameState gameState)
        {
            return gameState.Self.CardsInPlay.CountOf(Cards.Warehouse) >= 1;
        }
    }
}
