using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class MarketSquare
      : DerivedPlayerAction
    {
        public MarketSquare(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override bool ShouldPlayerDiscardCardFromHand(GameState gameState, Card card)
        {
            return true;
        }
    }
}