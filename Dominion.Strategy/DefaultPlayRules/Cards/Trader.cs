using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Trader
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Trader(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromHandToTrash(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return this.playerAction.DefaultGetCardFromHandToTrash(gameState, acceptableCard, isOptional);
        }

        public override bool ShouldRevealCardFromHandForCard(GameState gameState, Card card, Card cardFor)
        {
            return playerAction.trashOrder.DoesCardPickerMatch(gameState, cardFor) &&
                   !playerAction.purchaseOrder.DoesCardPickerMatch(gameState, cardFor);
        }
    }
}