using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class IllGottenGainsAlwaysGainCopper
        : DerivedPlayerAction
    {
        public IllGottenGainsAlwaysGainCopper(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override bool ShouldGainCard(GameState gameState, Card card)
        {
            return true;
        }
    }

    internal class IllGottenGains
        : DerivedPlayerAction
    {
        public IllGottenGains(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override bool ShouldGainCard(GameState gameState, Card card)
        {
            Card result = playerAction.gainOrder.GetPreferredCard(
                gameState,
                c => c == card);

            // do a reasonable default for gaining copper on ill gotten gain
            if (result == null)
            {
                return ShouldGainCopper(gameState, playerAction.purchaseOrder);
            }

            return false;
        }

        private static bool ShouldGainCopper(GameState gameState, ICardPicker gainOrder)
        {
            PlayerState self = gameState.Self;

            int minValue = self.ExpectedCoinValueAtEndOfTurn;
            int maxValue = minValue + Strategy.CountInHand(Dominion.Cards.IllGottenGains, gameState);

            if (maxValue == minValue)
                return false;

            CardPredicate shouldGainCard = delegate(Card card)
            {
                int currentCardCost = card.CurrentCoinCost(self);

                return currentCardCost >= minValue &&
                        currentCardCost <= maxValue;
            };

            Card cardType = gainOrder.GetPreferredCard(gameState, shouldGainCard);
            if (cardType == null)
                return false;

            int coppersToGain = DefaultPlayerAction.CostOfCard(cardType, gameState) - minValue;

            return (coppersToGain > 0);
        }
    }
}