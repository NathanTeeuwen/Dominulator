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
                            treasurePlayOrder: Default.DefaultTreasurePlayOrder(),
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
                        else if (gameState.Self.AvailableCoins >= 5 &&
                            gameState.Self.AvailableCoins < 8)
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
                    Card result = this.discardOrder.GetPreferredCardReverse(gameState, card => gameState.Self.Hand.HasCard(card) && acceptableCard(card));
                    if (result != null)
                    {
                        return result;
                    }

                    return base.GetCardFromHandToTopDeck(gameState, acceptableCard, isOptional);
                }

                public override bool ShouldPutCardInHand(GameState gameState, Card card)
                {
                    if (!ShouldBuyProvince(gameState) &&
                        gameState.Self.Hand.CountOf(CardTypes.Copper.card) > 0)
                    {
                        return false;
                    }
                    return true;
                }
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Province.card, ShouldBuyProvince),
                           CardAcceptance.For(CardTypes.Library.card, gameState => CountAllOwned(CardTypes.Library.card, gameState) < 1),
                           CardAcceptance.For(CardTypes.Count.card, gameState => CountAllOwned(CardTypes.Count.card, gameState) < 1));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(CardTypes.FishingVillage.card),
                    CardAcceptance.For(CardTypes.Library.card),
                    CardAcceptance.For(CardTypes.Count.card),
                    CardAcceptance.For(CardTypes.Library.card));

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(CardTypes.PoorHouse.card, gameState => CountAllOwned(CardTypes.PoorHouse.card, gameState) < 2 &&
                                                                                CountAllOwned(CardTypes.Count.card, gameState) >= 1),
                           CardAcceptance.For(CardTypes.FishingVillage.card));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.FishingVillage.card, ShouldPlayAction),
                           CardAcceptance.For(CardTypes.PoorHouse.card, ShouldPlayPoorHouse),
                           CardAcceptance.For(CardTypes.Library.card, ShouldPlayLibraryBeforeCount),
                           CardAcceptance.For(CardTypes.Count.card),
                           CardAcceptance.For(CardTypes.Library.card, ShouldPlayLibrary));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Estate.card),
                           CardAcceptance.For(CardTypes.Copper.card),
                           CardAcceptance.For(CardTypes.Silver.card));
            }

            private static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(CardTypes.Province.card),
                           CardAcceptance.For(CardTypes.Duchy.card),
                           CardAcceptance.For(CardTypes.Copper.card),
                           CardAcceptance.For(CardTypes.Estate.card),
                           CardAcceptance.For(CardTypes.PoorHouse.card),
                           CardAcceptance.For(CardTypes.Library.card),
                           CardAcceptance.For(CardTypes.FishingVillage.card),
                           CardAcceptance.For(CardTypes.Count.card));
            }

            private static bool DoesHandHaveCombinationToTrash(GameState gameState)
            {
                int countToTrash = CountInHandFrom(TrashOrder(), gameState);
                int countInHand = gameState.Self.Hand.Count;

                return (countInHand - countToTrash <= 2);
            }

            private static bool ShouldBuyProvince(GameState gameState)
            {
                return CountAllOwned(TrashOrder(), gameState) <= 3;
            }

            private static bool ShouldPlayLibraryBeforeCount(GameState gameState)
            {
                int countToTrash = CountAllOwned(TrashOrder(), gameState);
                return countToTrash >= 3 ? true : false;
            }

            private static bool ShouldPlayLibrary(GameState gameState)
            {
                if (!ShouldPlayAction(gameState))
                {
                    return false;
                }

                if (gameState.Self.Hand.CountWhere(card => card.isAction && card != CardTypes.Library.card) > 0 &&
                    gameState.Self.AvailableActions == 1)
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

                return gameState.Self.Hand.Where(card => card.isTreasure).Count() <= 3;
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
                var self = gameState.Self;
                if (self.Hand.CountWhere(card => card.isAction) == 1)
                {
                    return true;
                }

                return false;
            }

            private static bool HasExactlyOneActionOtherThanCount(GameState gameState)
            {
                var self = gameState.Self;
                if (!self.Hand.HasCard(CardTypes.Count.card))
                {
                    return false;
                }

                if (self.Hand.CountWhere(card => card.isAction) != 2)
                {
                    return false;
                }

                if (self.Hand.HasCard(CardTypes.Library.card) && self.AvailableActions >= 2)
                {
                    return false;
                }

                return true;
            }

            private static bool ShouldGainCopper(GameState gameState)
            {
                var self = gameState.Self;
                if (self.Hand.CountWhere(card => card.isAction) > 0)
                {
                    return false;
                }

                int countToTrash = CountInHandFrom(TrashOrder(), gameState);
                int countInHand = self.Hand.Count;

                if (countInHand - countToTrash > 0)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
