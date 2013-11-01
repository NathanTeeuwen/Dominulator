using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class MarketSquare
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public MarketSquare(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldPlayerDiscardCardFromHand(GameState gameState, Card card)
        {
            return true;
        }
    }
}