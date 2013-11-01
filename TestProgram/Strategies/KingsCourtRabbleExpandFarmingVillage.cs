using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Strategies
{    
    public class KingsCourtRabbleExpandFarmingVillage
        : Strategy
    {            
        public static PlayerAction Player()
        {
            return new MyPlayerAction();
        }

        class MyPlayerAction
            : PlayerAction
        {
            public MyPlayerAction()
                : base("KingsCourtRabbleExpandFarmingVillage",
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder:TrashOrder(),
                        treasurePlayOrder: TreasurePlayOrder())
            {
            }                
        }

        public static CardPickByPriority TreasurePlayOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.Contraband, gameState => gameState.Self.ExpectedCoinValueAtEndOfTurn < Cards.Colony.CurrentCoinCost(gameState.Self)),
                // base set first
                CardAcceptance.For(Cards.Platinum),
                CardAcceptance.For(Cards.Gold),
                CardAcceptance.For(Cards.Silver),
                CardAcceptance.For(Cards.Copper));
        }

        private static ICardPicker PurchaseOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Colony),
                        CardAcceptance.For(Cards.Province, gameState => CardBeingPlayedIs(Cards.Expand, gameState)),
                        CardAcceptance.For(Cards.Platinum, 1),                           
                        CardAcceptance.For(Cards.Expand, 1),
                        CardAcceptance.For(Cards.KingsCourt, 1),
                        CardAcceptance.For(Cards.Expand, gameState => CountAllOwned(Cards.Expand, gameState) < CountAllOwned(Cards.KingsCourt, gameState) - 3),
                        CardAcceptance.For(Cards.KingsCourt),
                        CardAcceptance.For(Cards.Contraband, 1),
                        CardAcceptance.For(Cards.FarmingVillage, gameState => gameState.Self.AllOwnedCards.CountWhere(c => c.isAction && c.plusAction == 0) >= gameState.Self.AllOwnedCards.CountWhere(c => c.isAction && c.plusAction != 0)),
                        CardAcceptance.For(Cards.Rabble),
                        CardAcceptance.For(Cards.Silver, 1));
        }

        private static ICardPicker ActionOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.KingsCourt, gameState => !CardBeingPlayedIs(Cards.KingsCourt, gameState)),                           
                        CardAcceptance.For(Cards.FarmingVillage, gameState => gameState.Self.AvailableActions == 0),                           
                        CardAcceptance.For(Cards.Rabble, gameState => gameState.Self.AvailableActions >= 1 && gameState.Self.CardsInDeckAndDiscard.Count() > 0),
                        CardAcceptance.For(Cards.Expand),
                        CardAcceptance.For(Cards.KingsCourt),
                        CardAcceptance.For(Cards.FarmingVillage),
                        CardAcceptance.For(Cards.Rabble, gameState => gameState.Self.CardsInDeckAndDiscard.Count() > 0));
        }

        private static ICardPicker TrashOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Curse),                           
                        CardAcceptance.For(Cards.Estate),
                        CardAcceptance.For(Cards.Copper),
                        CardAcceptance.For(Cards.Province),
                        CardAcceptance.For(Cards.Rabble),
                        CardAcceptance.For(Cards.KingsCourt),                           
                        CardAcceptance.For(Cards.Silver),                                                    
                        CardAcceptance.For(Cards.FarmingVillage),
                        CardAcceptance.For(Cards.Expand),
                        CardAcceptance.For(Cards.Colony));
        }
    }    
}