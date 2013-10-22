using Dominion;
using System;
using System.Linq;

namespace Program.DefaultStrategies
{
    internal class HorseTraders
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public HorseTraders(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromHandToDiscard(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return playerAction.DefaultGetCardFromHandToDiscard(gameState, acceptableCard, isOptional);
        }

        public override bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            return true;
        }
    }
}