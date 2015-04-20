using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Strategies
{
    public class TreasurySeaHag
        : Strategy
    {
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "TreasurySeaHag",
                        purchaseOrder: PurchaseOrder());
        }        

        private static CardPickByPriority PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => CountAllOwned(Cards.Gold, gameState) > 2),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        //CardAcceptance.For(Cards.Inn, gameState => gameState.Self.Discard.CountOf(Cards.SeaHag) > 0),
                        CardAcceptance.For(Cards.Treasury),                        
                        CardAcceptance.For(Cards.SeaHag, 1),
                        CardAcceptance.For(Cards.Silver));
        }
    }
}