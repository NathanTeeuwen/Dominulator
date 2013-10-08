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
                            treasurePlayOrder: Default.TreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder(),
                            discardOrder: DiscardOrder());
            }

            static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                          CardAcceptance.For<CardTypes.Province>(gameState => gameState.players.CurrentPlayer.AllOwnedCards.Where(card => card is CardTypes.Gold).Count() > 2),
                           CardAcceptance.For<CardTypes.Duchy>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 4),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 2),
                           CardAcceptance.For<CardTypes.Gold>(),
                           CardAcceptance.For<CardTypes.Estate>(gameState => CountOfPile<CardTypes.Province>(gameState) <= 3));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For<CardTypes.DeathCart>(),
                    CardAcceptance.For<CardTypes.Silver>(),
                    CardAcceptance.For<CardTypes.Warehouse>(),
                    CardAcceptance.For<CardTypes.Warehouse>());

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Silver>());

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Warehouse>(gameState => !HasNoRuinsInDeckAndDeathCartInHand(gameState)),
                    CardAcceptance.For<CardTypes.DeathCart>(HasActionInHandOtherThanDeathCart),
                    CardAcceptance.For<CardTypes.DeathCart>(gameState => gameState.players.CurrentPlayer.AllOwnedCards.Where(card => card is CardTypes.Gold).Count() > 2),
                    CardAcceptance.For<CardTypes.AbandonedMine>(),
                    CardAcceptance.For<CardTypes.RuinedLibrary>(),
                    CardAcceptance.For<CardTypes.Survivors>(),
                    CardAcceptance.For<CardTypes.RuinedMarket>(),
                    CardAcceptance.For<CardTypes.RuinedVillage>());
            }

            static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.Province>(),
                    CardAcceptance.For<CardTypes.Duchy>(),
                    CardAcceptance.For<CardTypes.Estate>(),
                    CardAcceptance.For<CardTypes.RuinedVillage>(ShouldDiscardRuin),
                    CardAcceptance.For<CardTypes.Survivors>(ShouldDiscardRuin),
                    CardAcceptance.For<CardTypes.RuinedMarket>(ShouldDiscardRuin),
                    CardAcceptance.For<CardTypes.RuinedLibrary>(ShouldDiscardRuin),
                    CardAcceptance.For<CardTypes.AbandonedMine>(ShouldDiscardRuin),
                    CardAcceptance.For<CardTypes.Copper>(),
                    CardAcceptance.For<CardTypes.Silver>(),
                    CardAcceptance.For<CardTypes.Warehouse>(),
                    CardAcceptance.For<CardTypes.DeathCart>(),
                    CardAcceptance.For<CardTypes.Gold>());
            }

            static bool ShouldDiscardRuin(GameState gameState)
            {
                return !gameState.players.CurrentPlayer.Hand.HasCard<CardTypes.DeathCart>() ||
                       gameState.players.CurrentPlayer.Hand.Where(card => card.isRuins).Count() > 1;
            }

            static bool HasActionInHandOtherThanDeathCart(GameState gameState)
            {
                return gameState.players.CurrentPlayer.Hand.Where(card => card.isAction && !(card.Is<CardTypes.DeathCart>())).Any();
            }

            static bool HasNoRuinsInDeck(GameState gameState)
            {
                return !gameState.players.CurrentPlayer.AllOwnedCards.Where(card => card.isRuins).Any();
            }

            static bool HasNoRuinsInDeckAndDeathCartInHand(GameState gameState)
            {
                return HasNoRuinsInDeck(gameState) && gameState.players.CurrentPlayer.Hand.HasCard<CardTypes.DeathCart>();
            }

            public static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                    CardAcceptance.For<CardTypes.RuinedVillage>(),
                    CardAcceptance.For<CardTypes.Survivors>(),
                    CardAcceptance.For<CardTypes.RuinedMarket>(),
                    CardAcceptance.For<CardTypes.RuinedLibrary>(),
                    CardAcceptance.For<CardTypes.AbandonedMine>(),
                    CardAcceptance.For<CardTypes.Warehouse>(HasNoRuinsInDeck));
            }

        }
    }
}
