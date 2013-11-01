using Dominion;
using System;
using System.Linq;

namespace Program.DefaultPlayRules
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