using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Chancellor
       : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Chancellor(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldPutDeckInDiscard(GameState gameState)
        {
            return true;
        }
    }
}