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
        public static class DeathCartDoubleWarehouse
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new PlayerAction(
                            "DeathCartDoubleWarehouse",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder(),
                            discardOrder: DiscardOrder());
            }

            static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Province.card, gameState => gameState.Self.AllOwnedCards.CountOf(CardTypes.Gold.card) > 2),
                           CardAcceptance.For(CardTypes.Duchy.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 4),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 2),
                           CardAcceptance.For(CardTypes.Gold.card),
                           CardAcceptance.For(CardTypes.Estate.card, gameState => CountOfPile(CardTypes.Province.card, gameState) <= 3));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(CardTypes.DeathCart.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Warehouse.card),
                    CardAcceptance.For(CardTypes.Warehouse.card));

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Silver.card));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.Warehouse.card, gameState => !HasNoRuinsInDeckAndDeathCartInHand(gameState)),
                    CardAcceptance.For(CardTypes.DeathCart.card, HasActionInHandOtherThanDeathCart),
                    CardAcceptance.For(CardTypes.DeathCart.card, gameState => gameState.Self.AllOwnedCards.CountOf(CardTypes.Gold.card) > 2),
                    CardAcceptance.For(CardTypes.AbandonedMine.card),
                    CardAcceptance.For(CardTypes.RuinedLibrary.card),
                    CardAcceptance.For(CardTypes.Survivors.card),
                    CardAcceptance.For(CardTypes.RuinedMarket.card),
                    CardAcceptance.For(CardTypes.RuinedVillage.card));
            }

            static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.Province.card),
                    CardAcceptance.For(CardTypes.Duchy.card),
                    CardAcceptance.For(CardTypes.Estate.card),
                    CardAcceptance.For(CardTypes.RuinedVillage.card, ShouldDiscardRuin),
                    CardAcceptance.For(CardTypes.Survivors.card, ShouldDiscardRuin),
                    CardAcceptance.For(CardTypes.RuinedMarket.card, ShouldDiscardRuin),
                    CardAcceptance.For(CardTypes.RuinedLibrary.card, ShouldDiscardRuin),
                    CardAcceptance.For(CardTypes.AbandonedMine.card, ShouldDiscardRuin),
                    CardAcceptance.For(CardTypes.Copper.card),
                    CardAcceptance.For(CardTypes.Silver.card),
                    CardAcceptance.For(CardTypes.Warehouse.card),
                    CardAcceptance.For(CardTypes.DeathCart.card),
                    CardAcceptance.For(CardTypes.Gold.card));
            }

            static bool ShouldDiscardRuin(GameState gameState)
            {
                return !gameState.Self.Hand.HasCard(CardTypes.DeathCart.card) ||
                       gameState.Self.Hand.CountWhere(card => card.isRuins) > 1;
            }

            static bool HasActionInHandOtherThanDeathCart(GameState gameState)
            {
                return gameState.Self.Hand.HasCard(card => card.isAction && !(card == CardTypes.DeathCart.card));
            }

            static bool HasNoRuinsInDeck(GameState gameState)
            {
                return !gameState.Self.AllOwnedCards.Where(card => card.isRuins).Any();
            }

            static bool HasNoRuinsInDeckAndDeathCartInHand(GameState gameState)
            {
                return HasNoRuinsInDeck(gameState) && gameState.Self.Hand.HasCard(CardTypes.DeathCart.card);
            }

            public static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For(CardTypes.RuinedVillage.card),
                    CardAcceptance.For(CardTypes.Survivors.card),
                    CardAcceptance.For(CardTypes.RuinedMarket.card),
                    CardAcceptance.For(CardTypes.RuinedLibrary.card),
                    CardAcceptance.For(CardTypes.AbandonedMine.card),
                    CardAcceptance.For(CardTypes.Warehouse.card, HasNoRuinsInDeck));
            }

        }
    }
}
