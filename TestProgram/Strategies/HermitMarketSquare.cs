using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Author: Sparafucile + SheCantSayNo 

namespace Program
{
    public static partial class Strategies
    {
        public static class HermitMarketSquare
        {
            public static PlayerAction Player()
            {
                return new MyPlayerAction();
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction()
                    : base("HermitMarketSquare",                        
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(TrashOrder()),
                        trashOrder: TrashOrder())
                {
                }

                private static CardPickByPriority PurchaseOrder()
                {
                    return new CardPickByPriority(
                               CardAcceptance.For(Cards.Province),
                               CardAcceptance.For(Cards.Duchy, gameState => CountAllOwned(Cards.Province, gameState) > 0),
                               CardAcceptance.For(Cards.Estate, gameState => CountAllOwned(Cards.Province, gameState) > 0),
                               CardAcceptance.For(Cards.Hermit, ShouldGainHermit),
                               CardAcceptance.For(Cards.MarketSquare, ShouldGainMarketSquare));
                }

                private static CardPickByPriority ActionOrder(ICardPicker trashOrder)
                {
                    return new CardPickByPriority(
                               CardAcceptance.For(Cards.Madman, ShouldPlayMadman(trashOrder)),
                               CardAcceptance.For(Cards.Hermit, IsDoingMegaTurn),
                               CardAcceptance.For(Cards.MarketSquare, ShouldPlayMarketSquare(trashOrder)),
                               CardAcceptance.For(Cards.Hermit, gameState => CountAllOwned(Cards.Madman, gameState) - CountAllOwned(Cards.Hermit, gameState) < 3),
                               CardAcceptance.For(Cards.Hermit, gameState => CountAllOwned(Cards.Province, gameState) > 0));
                }

                private static CardPickByPriority TrashOrder()
                {
                    return new CardPickByPriority(
                               CardAcceptance.For(Cards.Curse),
                               CardAcceptance.For(Cards.RuinedVillage),
                               CardAcceptance.For(Cards.RuinedMarket),
                               CardAcceptance.For(Cards.Survivors),
                               CardAcceptance.For(Cards.RuinedLibrary),
                               CardAcceptance.For(Cards.AbandonedMine),
                               CardAcceptance.For(Cards.Necropolis, gameState => IsDoingMegaTurn(gameState)),
                               CardAcceptance.For(Cards.OvergrownEstate, gameState => IsDoingMegaTurn(gameState)),
                               CardAcceptance.For(Cards.Hovel, gameState => IsDoingMegaTurn(gameState)),
                               CardAcceptance.For(Cards.Estate, gameState => IsDoingMegaTurn(gameState)),
                               CardAcceptance.For(Cards.Hermit, IsDoingMegaTurn));
                }
            }

            private static bool ShouldGainHermit(GameState gameState)
            {
                if (PlayBigHermit(gameState))
                    return CountHermitsEverGained(gameState) < 9 && CountAllOwned(Cards.MarketSquare, gameState) == 0;

                return CountHermitsEverGained(gameState) < 7 && CountAllOwned(Cards.MarketSquare, gameState) == 0;
            }

            private static bool ShouldGainMarketSquare(GameState gameState)
            {
                PlayerState self = gameState.Self;

                //Prioritize gaining Madmen over buying Market Squares once you have three squares
                if (CountAllOwned(Cards.Hermit, gameState) > CountInDeckAndDiscard(Cards.Hermit, gameState) + CountInHand(Cards.Hermit, gameState) &&
                    self.Hand.Count < 4 &&
                    CountAllOwned(Cards.MarketSquare, gameState) > 2)
                {
                    return false;
                }

                return true;
            }

            private static bool PlayBigHermit(GameState gameState)
            {
                //Play the 9-Hermit version if possible
                return CountOfPile(Cards.Hermit, gameState) + CountHermitsEverGained(gameState) >= 9;
            }

            private static GameStatePredicate ShouldPlayMadman(ICardPicker trashOrder)
            {
                return delegate(GameState gameState)
                {
                    PlayerState self = gameState.Self;

                    if (!self.Hand.HasCard(Cards.Madman))
                        return false;

                    if (CountAllOwned(Cards.Province, gameState) > 0)
                        return true;

                    if (ShouldStartMegaTurn(gameState))
                        return true;

                    if (IsDoingMegaTurn(gameState))
                    {
                        if (self.CardsInDeckAndDiscard.Count() > 5)
                            return true;
                    }

                    return false;
                };
            }

            private static GameStatePredicate ShouldPlayMarketSquare(ICardPicker trashOrder)
            {
                return delegate(GameState gameState)
                {
                    var self = gameState.Self;

                    if (!IsDoingMegaTurn(gameState))
                    {
                        return !CanTrashForGold(gameState, trashOrder);
                    }
                    else
                    {

                        if (ShouldPlayMadman(trashOrder)(gameState))
                            return false;

                        int numberOfProvincesCanAfford = self.ExpectedCoinValueAtEndOfTurn / Cards.Province.CurrentCoinCost(self);
                        if (self.AvailableBuys < numberOfProvincesCanAfford)
                            return true;

                        return !CanTrashForGold(gameState, trashOrder);
                    }
                };
            }

            private static bool CanTrashForGold(GameState gameState, ICardPicker trashOrder)
            {
                PlayerState self = gameState.Self;

                return trashOrder.GetPreferredCard(gameState, c => (self.Hand.HasCard(c) || self.Discard.HasCard(c)) && CardTypes.Hermit.CanTrashCard(c)) != null &&
                       self.Hand.HasCard(Cards.Hermit) &&
                       self.Hand.HasCard(Cards.MarketSquare);
            }

            private static bool ShouldStartMegaTurn(GameState gameState)
            {
                PlayerState self = gameState.Self;

                int CountMSNotInPlay = CountInDeckAndDiscard(Cards.MarketSquare, gameState) +
                  CountInHand(Cards.MarketSquare, gameState);

                if (self.Hand.Count < 5 ||
                    self.Hand.CountOf(Cards.Madman) < 2 ||
                    CountMSNotInPlay < 3)
                    return false;

                if (PlayBigHermit(gameState))
                {
                    return self.AllOwnedCards.CountOf(Cards.Madman) >= 6 &&                           
                           CountHermitsEverGained(gameState) >= 9;
                }
                else
                {
                    return self.AllOwnedCards.CountOf(Cards.Madman) >= 4 &&
                           CountHermitsEverGained(gameState) >= 7;
                }
            }

            private static bool IsDoingMegaTurn(GameState gameState)
            {
                PlayerState self = gameState.Self;

                return self.Hand.Count > 6;
            }

            private static int CountHermitsEverGained(GameState gameState)
            {
                PlayerState self = gameState.Self;
                return CountAllOwned(Cards.Hermit, gameState) + CountAllOwned(Cards.Madman, gameState);
            }
        }
    }
}