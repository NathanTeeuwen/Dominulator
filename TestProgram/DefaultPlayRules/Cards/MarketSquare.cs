using Dominion;
using System;
using System.Linq;

namespace Program.DefaultStrategies
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