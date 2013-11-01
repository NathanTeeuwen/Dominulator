using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program;

namespace Strategies
{    
    public class DeathCartDoubleWarehouse
        : Strategy
    {
            
        public static PlayerAction Player()
        {
            return new PlayerAction(
                        "DeathCartDoubleWarehouse",                            
                        purchaseOrder: PurchaseOrder(),
                        treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                        actionOrder: ActionOrder(),
                        trashOrder: TrashOrder(),
                        discardOrder: DiscardOrder());
        }

        static ICardPicker PurchaseOrder()
        {
            var highPriority = new CardPickByPriority(
                        CardAcceptance.For(Cards.Province, gameState => gameState.Self.AllOwnedCards.CountOf(Cards.Gold) > 2),
                        CardAcceptance.For(Cards.Duchy, gameState => CountOfPile(Cards.Province, gameState) <= 4),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 2),
                        CardAcceptance.For(Cards.Gold),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) <= 3));

            var buildOrder = new CardPickByBuildOrder(
                CardAcceptance.For(Cards.DeathCart),
                CardAcceptance.For(Cards.Silver),
                CardAcceptance.For(Cards.Warehouse),
                CardAcceptance.For(Cards.Warehouse));

            var lowPriority = new CardPickByPriority(
                        CardAcceptance.For(Cards.Silver));

            return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
        }

        static CardPickByPriority ActionOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.Warehouse, gameState => !HasNoRuinsInDeckAndDeathCartInHand(gameState)),
                CardAcceptance.For(Cards.DeathCart, HasActionInHandOtherThanDeathCart),
                CardAcceptance.For(Cards.DeathCart, gameState => gameState.Self.AllOwnedCards.CountOf(Cards.Gold) > 2),
                CardAcceptance.For(Cards.AbandonedMine),
                CardAcceptance.For(Cards.RuinedLibrary),
                CardAcceptance.For(Cards.Survivors),
                CardAcceptance.For(Cards.RuinedMarket),
                CardAcceptance.For(Cards.RuinedVillage));
        }

        static CardPickByPriority DiscardOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.Province),
                CardAcceptance.For(Cards.Duchy),
                CardAcceptance.For(Cards.Estate),
                CardAcceptance.For(Cards.RuinedVillage, ShouldDiscardRuin),
                CardAcceptance.For(Cards.Survivors, ShouldDiscardRuin),
                CardAcceptance.For(Cards.RuinedMarket, ShouldDiscardRuin),
                CardAcceptance.For(Cards.RuinedLibrary, ShouldDiscardRuin),
                CardAcceptance.For(Cards.AbandonedMine, ShouldDiscardRuin),
                CardAcceptance.For(Cards.Copper),
                CardAcceptance.For(Cards.Silver),
                CardAcceptance.For(Cards.Warehouse),
                CardAcceptance.For(Cards.DeathCart),
                CardAcceptance.For(Cards.Gold));
        }

        static bool ShouldDiscardRuin(GameState gameState)
        {
            return !gameState.Self.Hand.HasCard(Cards.DeathCart) ||
                    gameState.Self.Hand.CountWhere(card => card.isRuins) > 1;
        }

        static bool HasActionInHandOtherThanDeathCart(GameState gameState)
        {
            return gameState.Self.Hand.HasCard(card => card.isAction && !(card == Cards.DeathCart));
        }

        static bool HasNoRuinsInDeck(GameState gameState)
        {
            return !gameState.Self.AllOwnedCards.Where(card => card.isRuins).Any();
        }

        static bool HasNoRuinsInDeckAndDeathCartInHand(GameState gameState)
        {
            return HasNoRuinsInDeck(gameState) && gameState.Self.Hand.HasCard(Cards.DeathCart);
        }

        public static CardPickByPriority TrashOrder()
        {
            return new CardPickByPriority(
                CardAcceptance.For(Cards.RuinedVillage),
                CardAcceptance.For(Cards.Survivors),
                CardAcceptance.For(Cards.RuinedMarket),
                CardAcceptance.For(Cards.RuinedLibrary),
                CardAcceptance.For(Cards.AbandonedMine),
                CardAcceptance.For(Cards.Warehouse, HasNoRuinsInDeck));
        }

    }
}
