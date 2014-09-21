using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Library
        : DerivedPlayerAction
    {
        public Library(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override bool ShouldPutCardInHand(GameState gameState, Card card)
        {
            return !playerAction.discardOrder.DoesCardPickerMatch(gameState, card);
        }
    }
}