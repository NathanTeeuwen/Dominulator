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
                        bool shouldDiscard = card.isVictory || card.Is<Copper>();
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
                    CardAcceptance.For<Province>(ShouldBuyProvinceOverDuchyDuke),
                    CardAcceptance.For<Duke>(gameState => ShouldBuyDukeOverDuchy(gameState) && ShouldBuyDuchyDukeOverPowerUp(gameState)),
                    CardAcceptance.For<Duchy>(ShouldBuyDuchyDukeOverPowerUp),
                    CardAcceptance.For<Duke>(ShouldBuyDuchyDukeOverPowerUp),
                    CardAcceptance.For<Estate>(gameState => CountOfPile<Province>(gameState) < 2),                    
                    CardAcceptance.For<Duke>(gameState => ShouldBuyDukeOverDuchy(gameState) && AtEndGame(gameState)),
                    CardAcceptance.For<Duchy>(AtEndGame),
                    CardAcceptance.For<Duke>(AtEndGame),                    
                    CardAcceptance.For<Caravan>(CanDoubleCaravan),
                    CardAcceptance.For<Cartographer>(gameState => CountAllOwned<Cartographer>(gameState) < 2),
                    CardAcceptance.For<Gold>(gameState => gameState.Self.AvailableBuys <= 1));

                var buildOrder = new CardPickByBuildOrder(
                    CardAcceptance.For<Bridge>(),
                    CardAcceptance.For<Silver>(),
                    CardAcceptance.For<Silver>(),
                    CardAcceptance.For<Bridge>());

                var lowPriority = new CardPickByPriority(
                           CardAcceptance.For<Caravan>(),
                           CardAcceptance.For<Estate>(gameState => CountOfPile<Province>(gameState) < 4),
                           CardAcceptance.For<Silver>());

                return new CardPickConcatenator(highPriority, buildOrder, lowPriority);               
            }

            private static bool CanDoubleCaravan(GameState gameState)
            {
                return gameState.Self.AvailableCoins >= 6 &&
                       gameState.Self.AvailableBuys > 1;
            }

            private static bool AtEndGame(GameState gameState)
            {
                return CountOfPile<Duchy>(gameState) <= 6;
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
                    CountOfPile<Province>(gameState) == 1)
                {
                    return true;
                }

                return false;*/
                return false;
            }

            private static CardPickByPriority ActionOrder()
            {
                return new CardPickByPriority(
                           CardAcceptance.For<Cartographer>(),
                           CardAcceptance.For<Caravan>(),
                           CardAcceptance.For<Bridge>());
            }

            private static CardPickByPriority TopDeckOrder()
            {
                return new CardPickByPriority(                           
                           CardAcceptance.For<Cartographer>(),
                           CardAcceptance.For<Bridge>(),
                           CardAcceptance.For<Silver>(),                                                      
                           CardAcceptance.For<Caravan>());
            }

            private static bool ShouldBuyDukeOverDuchy(GameState gameState)
            {     
                /*
                int duchyCount = CountAllOwned<Duchy>(gameState);
                int dukeCount = CountAllOwned<Duke>(gameState);

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
