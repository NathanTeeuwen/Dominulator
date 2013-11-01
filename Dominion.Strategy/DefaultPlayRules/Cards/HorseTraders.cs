using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
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