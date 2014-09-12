using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Strategies
{    
    public class CaravanBridgeDukeCartographer
        : Strategy
    {
            
        public static PlayerAction Player()
        {
            return new MyPlayerAction();
        }

        class MyPlayerAction
            : PlayerAction
        {
            public MyPlayerAction()
                : base( "CaravanBridgeDukeCartographer",                            
                        purchaseOrder: PurchaseOrder(),                            
                        actionOrder: ActionOrder())
            {
            }

            public override Card GetCardFromRevealedCardsToTopDeck(GameState gameState)
            {
                BagOfCards revealedCards = gameState.Self.CardsBeingRevealed;
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
                    bool shouldDiscard = card.isVictory || card == Cards.Copper;
                    if (!shouldDiscard)
                    {
                        return card;
                    }
                }

                return null;
            }

            public override Card GetCardFromRevealedCardsToPutOnDeck(GameState gameState)
            {
                return gameState.Self.CardsBeingRevealed.FirstOrDefault();
            }

            public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
            {
                return PlayerActionChoice.Discard;
            }
        }            

        private static ICardPicker PurchaseOrder()
        {
            var highPriority = new CardPickByPriority(
                CardAcceptance.For(Cards.Province, ShouldBuyProvinceOverDuchyDuke),
                CardAcceptance.For(Cards.Duke, gameState => ShouldBuyDukeOverDuchy(gameState) && ShouldBuyDuchyDukeOverPowerUp(gameState)),
                CardAcceptance.For(Cards.Duchy, ShouldBuyDuchyDukeOverPowerUp),
                CardAcceptance.For(Cards.Duke, ShouldBuyDuchyDukeOverPowerUp),
                CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 2),
                CardAcceptance.For(Cards.Duke, gameState => ShouldBuyDukeOverDuchy(gameState) && AtEndGame(gameState)),
                CardAcceptance.For(Cards.Duchy, AtEndGame),
                CardAcceptance.For(Cards.Duke, AtEndGame),
                CardAcceptance.For(Cards.Caravan, CanDoubleCaravan),
                CardAcceptance.For(Cards.Cartographer, gameState => CountAllOwned(Cards.Cartographer, gameState) < 2),
                CardAcceptance.For(Cards.Gold, gameState => gameState.Self.AvailableBuys <= 1));

            var buildOrder = new CardPickByBuildOrder(
                CardAcceptance.For(Cards.Bridge),
                CardAcceptance.For(Cards.Silver),
                CardAcceptance.For(Cards.Silver),
                CardAcceptance.For(Cards.Bridge));

            var lowPriority = new CardPickByPriority(
                        CardAcceptance.For(Cards.Caravan),
                        CardAcceptance.For(Cards.Estate, gameState => CountOfPile(Cards.Province, gameState) < 4),
                        CardAcceptance.For(Cards.Silver));

            return new CardPickConcatenator(highPriority, buildOrder, lowPriority);               
        }

        private static bool CanDoubleCaravan(GameState gameState)
        {
            return gameState.Self.AvailableCoins >= 6 &&
                    gameState.Self.AvailableBuys > 1;
        }

        private static bool AtEndGame(GameState gameState)
        {
            return CountOfPile(Cards.Duchy, gameState) <= 6;
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
                        CardAcceptance.For(Cards.Cartographer),
                        CardAcceptance.For(Cards.Caravan),
                        CardAcceptance.For(Cards.Bridge));
        }

        private static CardPickByPriority TopDeckOrder()
        {
            return new CardPickByPriority(
                        CardAcceptance.For(Cards.Cartographer),
                        CardAcceptance.For(Cards.Bridge),
                        CardAcceptance.For(Cards.Silver),
                        CardAcceptance.For(Cards.Caravan));
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
