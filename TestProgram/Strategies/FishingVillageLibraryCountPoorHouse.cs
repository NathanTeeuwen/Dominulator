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
                        gameState.Self.Hand.CountOf(Cards.Copper) > 0)
                    {
                        return false;
                    }
                    return true;
                }
            }

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                           CardAcceptance.For(Cards.Province, ShouldBuyProvince),
                           CardAcceptance.For(Cards.Library, gameState => CountAllOwned(Cards.Library, gameState) < 1),
                           CardAcceptance.For(Cards.Count, gameState => CountAllOwned(Cards.Count, gameState) < 1));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(Cards.FishingVillage),
                    CardAcceptance.For(Cards.Library),
                    CardAcceptance.For(Cards.Count),
                    CardAcceptance.For(Cards.Library));

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(Cards.PoorHouse, gameState => CountAllOwned(Cards.PoorHouse, gameState) < 2 &&
                                                                                CountAllOwned(Cards.Count, gameState) >= 1),
                           CardAcceptance.For(Cards.FishingVillage));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.FishingVillage, ShouldPlayAction),
                           CardAcceptance.For(Cards.PoorHouse, ShouldPlayPoorHouse),
                           CardAcceptance.For(Cards.Library, ShouldPlayLibraryBeforeCount),
                           CardAcceptance.For(Cards.Count),
                           CardAcceptance.For(Cards.Library, ShouldPlayLibrary));
            }

            private static CardPickByPriority TrashOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Estate),
                           CardAcceptance.For(Cards.Copper),
                           CardAcceptance.For(Cards.Silver));
            }

            private static CardPickByPriority DiscardOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cards.Province),
                           CardAcceptance.For(Cards.Duchy),
                           CardAcceptance.For(Cards.Copper),
                           CardAcceptance.For(Cards.Estate),
                           CardAcceptance.For(Cards.PoorHouse),
                           CardAcceptance.For(Cards.Library),
                           CardAcceptance.For(Cards.FishingVillage),
                           CardAcceptance.For(Cards.Count));
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

                if (gameState.Self.Hand.CountWhere(card => card.isAction && card != Cards.Library) > 0 &&
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
                if (!self.Hand.HasCard(Cards.Count))
                {
                    return false;
                }

                if (self.Hand.CountWhere(card => card.isAction) != 2)
                {
                    return false;
                }

                if (self.Hand.HasCard(Cards.Library) && self.AvailableActions >= 2)
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
