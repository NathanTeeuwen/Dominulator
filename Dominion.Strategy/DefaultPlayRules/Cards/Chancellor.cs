using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Chancellor
       : DerivedPlayerAction
    {
        public Chancellor(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override bool ShouldPutDeckInDiscard(GameState gameState)
        {
            return true;
        }
    }
}