using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
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