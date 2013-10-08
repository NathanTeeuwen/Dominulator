using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Author: SheCantSayNo

namespace Program
{
    public static partial class Strategies
    {
        public static class HermitMarketSqaure
        {
            public static PlayerAction Player(int playerNumber)
            {
                return new MyPlayerAction(playerNumber);
            }

            class MyPlayerAction
                : PlayerAction
            {
                public MyPlayerAction(int playerNumber)
                    : base("HermitMarketSquare",
                        playerNumber,
                        purchaseOrder: PurchaseOrder(),
                        actionOrder: ActionOrder(TrashOrder()),
                        trashOrder: TrashOrder())
                {
                }

                private static CardPickByPriority PurchaseOrder()
                {
                    return new CardPickByPriority(
                               CardAcceptance.For<CardTypes.Province>(),
                               CardAcceptance.For<CardTypes.Duchy>(gameState => CountAllOwned<CardTypes.Province>(gameState) > 0),
                               CardAcceptance.For<CardTypes.Estate>(gameState => CountAllOwned<CardTypes.Province>(gameState) > 0),
                               CardAcceptance.For<CardTypes.Hermit>(ShouldGainHermit),
                               CardAcceptance.For<CardTypes.MarketSquare>());
                }

                private static CardPickByPriority ActionOrder(ICardPicker trashOrder)
                {
                    return new CardPickByPriority(
                               CardAcceptance.For<CardTypes.Madman>(ShouldPlayMadman(trashOrder)),
                               CardAcceptance.For<CardTypes.MarketSquare>(ShouldPlayMarketSquare(trashOrder)),
                               CardAcceptance.For<CardTypes.Hermit>());
                }

                private static CardPickByPriority TrashOrder()
                {
                    return new CardPickByPriority(
                               CardAcceptance.For<CardTypes.Estate>(gameState => (IsDoingMegaTurn(gameState) || CountAllOwned<CardTypes.Estate>(gameState) > 1) && CountAllOwned<CardTypes.Province>(gameState) == 0),
                               CardAcceptance.For<CardTypes.Hermit>(IsDoingMegaTurn));
                }                     
            }

            private static bool ShouldGainHermit(GameState gameState)
            {
                return CountHermitsEverGained(gameState) < 7;
            }

            private static GameStatePredicate ShouldPlayMadman(ICardPicker trashOrder)
            {
                return delegate(GameState gameState)
                {
                    PlayerState currentPlayer = gameState.players.CurrentPlayer;

                    if (!currentPlayer.Hand.HasCard<CardTypes.Madman>())
                        return false;

                    if (CountAllOwned<CardTypes.Province>(gameState) > 0)
                        return true;

                    if (ShouldStartMegaTurn(gameState))
                        return true;

                    if (IsDoingMegaTurn(gameState))
                    {
                        /*
                        if (CountInHand<CardTypes.Madman>(gameState) == 1 && CanTrashForGold(gameState, trashOrder))
                            return false;
                        */

                        if (currentPlayer.CardsInDeckAndDiscard.Count() > 5)
                            return true;
                    }

                    return false;
                };
            }

            private static GameStatePredicate ShouldPlayMarketSquare(ICardPicker trashOrder)
            {
                return delegate(GameState gameState)
                {
                    var currentPlayer = gameState.players.CurrentPlayer;

                    if (!IsDoingMegaTurn(gameState))
                    {
                        return !CanTrashForGold(gameState, trashOrder);                        
                    }
                    else
                    {
                        if (ShouldPlayMadman(trashOrder)(gameState))
                            return false;

                        int numberOfProvincesCanAfford = currentPlayer.ExpectedCoinValueAtEndOfTurn / Card.Type<CardTypes.Province>().CurrentCoinCost(currentPlayer);
                        if (currentPlayer.AvailableBuys < numberOfProvincesCanAfford)
                            return true;

                        return !CanTrashForGold(gameState, trashOrder);
                    }
                };
            }

            private static bool CanTrashForGold(GameState gameState, ICardPicker trashOrder)
            {
                PlayerState currentPlayer = gameState.players.CurrentPlayer;

                return trashOrder.GetPreferredCard(gameState, c => (currentPlayer.Hand.HasCard(c) || currentPlayer.Discard.HasCard(c)) && CardTypes.Hermit.CanTrashCard(c)) != null &&
                       currentPlayer.Hand.HasCard<CardTypes.Hermit>() &&
                       currentPlayer.Hand.HasCard<CardTypes.MarketSquare>();
            }

            private static bool ShouldStartMegaTurn(GameState gameState)
            {
                PlayerState currentPlayer = gameState.players.CurrentPlayer;

                return currentPlayer.Hand.Count >= 5 &&
                       currentPlayer.Hand.Where(c => c.Is<CardTypes.Madman>()).Count() >= 2 &&
                       currentPlayer.AllOwnedCards.Where(c => c.Is<CardTypes.Madman>()).Count() >= 4 &&
                       CountHermitsEverGained(gameState) >= 7;
            }

            private static bool IsDoingMegaTurn(GameState gameState)
            {
                PlayerState currentPlayer = gameState.players.CurrentPlayer;

                return currentPlayer.Hand.Count > 6;
            }

            private static int CountHermitsEverGained(GameState gameState)
            {
                PlayerState currentPlayer = gameState.players.CurrentPlayer;
                return CountAllOwned<CardTypes.Hermit>(gameState) + CountAllOwned<CardTypes.Madman>(gameState);
            }
        }
    }
}