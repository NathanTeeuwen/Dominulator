using Dominion;
using System;
using System.Linq;

namespace Program.DefaultStrategies
{
    internal class IllGottenGainsAlwaysGainCopper
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public IllGottenGainsAlwaysGainCopper(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldGainCard(GameState gameState, Card card)
        {
            return true;
        }
    }

    internal class IllGottenGains
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public IllGottenGains(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
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
            int maxValue = minValue + Strategy.CountInHand(Cards.IllGottenGains, gameState);

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

            int coppersToGain = PlayerAction.CostOfCard(cardType, gameState) - minValue;

            return (coppersToGain > 0);
        }
    }
}