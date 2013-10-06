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
        public static class FishingVillageLibraryCountPoorHouse
        {
            // big money smithy player
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base( "FishingVillageLibraryCountPoorHouse",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),
                            treasurePlayOrder: Default.TreasurePlayOrder(),
                            actionOrder: ActionOrder(),
                            trashOrder: TrashOrder(),
                            discardOrder: DiscardOrder())
                {
                }

                public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    bool wantToTrash = DoesHandHaveCombinationToTrash(gameState) &&
                                       HasCardFromInHand(TrashOrder(), gameState);

                    if (acceptableChoice(PlayerActionChoice.Trash))
                    {
                        if (wantToTrash && !ShouldBuyProvince(gameState))
                        {
                            return PlayerActionChoice.Trash;
                        }
                        else if (gameState.players.CurrentPlayer.AvailableCoins >= 5 &&
                            gameState.players.CurrentPlayer.AvailableCoins < 8)
                        {
                            return PlayerActionChoice.PlusCoin;
                        }
                        else
                        {
                            return PlayerActionChoice.GainCard;
                        }
                    }
                    else
                    {
                        if (HasExactlyOneAction(gameState))
                        {
                            return PlayerActionChoice.TopDeck;
                        }
                        else if (ShouldGainCopper(gameState))
                        {
                            return PlayerActionChoice.GainCard;
                        }
                        else
                        {
                            return PlayerActionChoice.Discard;
                        }
                    }
                }

                override public Card GetCardFromHandToTopDeck(GameState gameState, CardPredicate acceptableCard, bool isOptional)
                {
                    Card result = this.discardOrder.GetPreferredCardReverse(gameState, card => gameState.players.CurrentPlayer.Hand.HasCard(card) && acceptableCard(card));
                    if (result != null)
                    {
                        return result;
                    }

                    return base.GetCardFromHandToTopDeck(gameState, acceptableCard, isOptional);
                }

                public override bool ShouldPutCardInHand(GameState gameState, Card card)
                {
                    if (!ShouldBuyProvince(gameState) &&
                        gameState.players.CurrentPlayer.Hand.Where(test => test.Is<CardTypes.Count>()).Any())
                    {
                        return false;
                    }
                    return true;
                }
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(ShouldBuyProvince),
                           CardAcceptance.For<CardTypes.Library>(gameState => CountAllOwned<CardTypes.Library>(gameState) < 1),
                           CardAcceptance.For<CardTypes.Count>(gameState => CountAllOwned<CardTypes.Count>(gameState) < 1));

                var buildOrder = new CardPickByBuildOrder(
                    new CardTypes.FishingVillage(),
                    new CardTypes.Library(),
                    new CardTypes.Count(),
                    new CardTypes.Library());

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For<CardTypes.PoorHouse>(gameState => CountAllOwned<CardTypes.PoorHouse>(gameState) < 2 &&
                                                                                CountAllOwned<CardTypes.Count>(gameState) >= 1),
                           CardAcceptance.For<CardTypes.FishingVillage>());

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.FishingVillage>(ShouldPlayAction),
                           CardAcceptance.For<CardTypes.PoorHouse>(ShouldPlayPoorHouse),
                           CardAcceptance.For<CardTypes.Library>(ShouldPlayLibraryBeforeCount),
                           CardAcceptance.For<CardTypes.Count>(),
                           CardAcceptance.For<CardTypes.Library>(ShouldPlayLibrary));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Estate>(),
                           CardAcceptance.For<CardTypes.Copper>(),
                           CardAcceptance.For<CardTypes.Silver>());
            }

            private static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<CardTypes.Province>(),
                           CardAcceptance.For<CardTypes.Duchy>(),
                           CardAcceptance.For<CardTypes.Copper>(),
                           CardAcceptance.For<CardTypes.Estate>(),
                           CardAcceptance.For<CardTypes.PoorHouse>(),
                           CardAcceptance.For<CardTypes.Library>(),
                           CardAcceptance.For<CardTypes.FishingVillage>(),
                           CardAcceptance.For<CardTypes.Count>());
            }

            private static bool DoesHandHaveCombinationToTrash(GameState gameState)
            {
                int countToTrash = CountInHandFrom(TrashOrder(), gameState);
                int countInHand = gameState.players.CurrentPlayer.Hand.Count;

                return (countInHand - countToTrash <= 2);
            }

            private static bool ShouldBuyProvince(GameState gameState)
            {
                return CountAllOwnedMatching(TrashOrder(), gameState) <= 3;
            }

            private static bool ShouldPlayLibraryBeforeCount(GameState gameState)
            {
                int countToTrash = CountAllOwnedMatching(TrashOrder(), gameState);
                return countToTrash >= 3 ? true : false;
            }

            private static bool ShouldPlayLibrary(GameState gameState)
            {
                if (!ShouldPlayAction(gameState))
                {
                    return false;
                }

                if (gameState.players.CurrentPlayer.Hand.Where(card => card.isAction && !card.Is<CardTypes.Library>()).Count() > 0 &&
                    gameState.players.CurrentPlayer.AvailableActions == 1)
                {
                    return false;
                }

                return true;
            }

            private static bool ShouldPlayPoorHouse(GameState gameState)
            {
                if (!ShouldPlayAction(gameState))
                {
                    return false;
                }

                return gameState.players.CurrentPlayer.Hand.Where(card => card.isTreasure).Count() <= 3;
            }

            private static bool ShouldPlayAction(GameState gameState)
            {
                return !ShouldTopDeckAndTrash(gameState);
            }

            private static bool ShouldTopDeckAndTrash(GameState gameState)
            {
                return HasExactlyOneActionOtherThanCount(gameState) && !ShouldBuyProvince(gameState);
            }

            private static bool HasExactlyOneAction(GameState gameState)
            {
                var currentPlayer = gameState.players.CurrentPlayer;
                if (currentPlayer.Hand.Where(card => card.isAction).Count() == 1)
                {
                    return true;
                }

                return false;
            }

            private static bool HasExactlyOneActionOtherThanCount(GameState gameState)
            {
                var currentPlayer = gameState.players.CurrentPlayer;
                if (!currentPlayer.Hand.HasCard<CardTypes.Count>())
                {
                    return false;
                }

                if (currentPlayer.Hand.Where(card => card.isAction).Count() != 2)
                {
                    return false;
                }

                if (currentPlayer.Hand.HasCard<CardTypes.Library>() && currentPlayer.AvailableActions >= 2)
                {
                    return false;
                }

                return true;
            }

            private static bool ShouldGainCopper(GameState gameState)
            {
                var currentPlayer = gameState.players.CurrentPlayer;
                if (currentPlayer.Hand.Where(card => card.isAction).Count() > 0)
                {
                    return false;
                }

                int countToTrash = CountInHandFrom(TrashOrder(), gameState);
                int countInHand = gameState.players.CurrentPlayer.Hand.Count;

                if (countInHand - countToTrash > 0)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
