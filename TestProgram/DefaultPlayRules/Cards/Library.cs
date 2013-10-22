using Dominion;
using System;
using System.Linq;

namespace Program.DefaultStrategies
{
    internal class Library
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Library(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldPutCardInHand(GameState gameState, Card card)
        {
            return !playerAction.discardOrder.DoesCardPickerMatch(gameState, card);
        }
    }
}