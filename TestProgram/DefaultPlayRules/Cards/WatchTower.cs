using Dominion;
using System;
using System.Linq;

namespace Program.DefaultStrategies
{
    internal class Watchtower
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Watchtower(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            return true;
        }
    }
}