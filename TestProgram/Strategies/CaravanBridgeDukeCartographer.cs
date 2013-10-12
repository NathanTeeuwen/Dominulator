using Dominion;
using Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    public static partial class Strategies
    {
        public static class CaravanBridgeDukeCartographer
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
                    : base( "CaravanBridgeDukeCartographer",
                            playerNumber,
                            purchaseOrder: PurchaseOrder(),                            
                            actionOrder: ActionOrder())
                {
                }

                public override Card GetCardFromRevealedCardsToTopDeck(GameState gameState, PlayerState player)                
                {
                    BagOfCards revealedCards = player.CardsBeingRevealed;
                    var self = gameState.Self;
                    Card result = TopDeckOrder().GetPreferredCard(
                        gameState,
                        card => revealedCards.HasCard(card));

                    if (result != null)
                    {
                        return result;
                    }

                    foreach (Card card in revealedCards)
                    {
                        bool shouldDiscard = card.isVictory || card == Copper.card;
                        if (!shouldDiscard)
                        {
                            return card;
                        }
                    }

                    return null;
                }

                public override Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState, PlayerState player)
                {
                    return player.CardsBeingRevealed.FirstOrDefault();
                }

                public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
                {
                    return PlayerActionChoice.Discard;
                }
            }            

            private static ICardPicker PurchaseOrder()
            {
                var highPriority = new CardPickByPriority(
                    CardAcceptance.For(Province.card, ShouldBuyProvinceOverDuchyDuke),
                    CardAcceptance.For(Duke.card, gameState => ShouldBuyDukeOverDuchy(gameState) && ShouldBuyDuchyDukeOverPowerUp(gameState)),
                    CardAcceptance.For(Duchy.card, ShouldBuyDuchyDukeOverPowerUp),
                    CardAcceptance.For(Duke.card, ShouldBuyDuchyDukeOverPowerUp),
                    CardAcceptance.For(Estate.card, gameState => CountOfPile(Province.card, gameState) < 2),                    
                    CardAcceptance.For(Duke.card, gameState => ShouldBuyDukeOverDuchy(gameState) && AtEndGame(gameState)),
                    CardAcceptance.For(Duchy.card, AtEndGame),
                    CardAcceptance.For(Duke.card, AtEndGame),                    
                    CardAcceptance.For(Caravan.card, CanDoubleCaravan),
                    CardAcceptance.For(Cartographer.card, gameState => CountAllOwned(Cartographer.card, gameState) < 2),
                    CardAcceptance.For(Gold.card, gameState => gameState.Self.AvailableBuys <= 1));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For(Bridge.card),
                    CardAcceptance.For(Silver.card),
                    CardAcceptance.For(Silver.card),
                    CardAcceptance.For(Bridge.card));

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For(Caravan.card),
                           CardAcceptance.For(Estate.card, gameState => CountOfPile(Province.card, gameState) < 4),
                           CardAcceptance.For(Silver.card));

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);               
            }

            private static bool CanDoubleCaravan(GameState gameState)
            {
                return gameState.Self.AvailableCoins >= 6 &&
                       gameState.Self.AvailableBuys > 1;
            }

            private static bool AtEndGame(GameState gameState)
            {
                return CountOfPile(Duchy.card, gameState) <= 6;
            }

            private static bool ShouldBuyDuchyDukeOverPowerUp(GameState gameState)
            {
                return (gameState.Self.AvailableBuys >= 1 &&
                        gameState.Self.AvailableCoins >= 8);
            }

            private static bool ShouldBuyProvinceOverDuchyDuke(GameState gameState)
            {
                /*
                if (gameState.Self.AvailableBuys == 1 &&
                    CountOfPile(Province.card, gameState) == 1)
                {
                    return true;
                }

                return false;*/
                return false;
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For(Cartographer.card),
                           CardAcceptance.For(Caravan.card),
                           CardAcceptance.For(Bridge.card));
            }

            private static CardPickByPriority TopDeckOrder()
            {
                return new CardPickByPriority(                           
                           CardAcceptance.For(Cartographer.card),
                           CardAcceptance.For(Bridge.card),
                           CardAcceptance.For(Silver.card),                                                      
                           CardAcceptance.For(Caravan.card));
            }

            private static bool ShouldBuyDukeOverDuchy(GameState gameState)
            {     
                /*
                int duchyCount = CountAllOwned(Duchy.card, gameState);
                int dukeCount = CountAllOwned(Duke.card, gameState);

                return VictoryFor(duchyCount, dukeCount + 1) > VictoryFor(duchyCount + 1, dukeCount);*/
                return false;
            }

            private static int VictoryFor(int duchy, int duke)
            {
                return 3 * duchy + duke * duchy;
            }
        }
    }
}
