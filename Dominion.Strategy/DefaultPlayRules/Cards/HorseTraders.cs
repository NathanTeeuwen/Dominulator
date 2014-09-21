using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class HorseTraders
        : DerivedPlayerAction
    {
        public HorseTraders(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            return true;
        }
    }
}