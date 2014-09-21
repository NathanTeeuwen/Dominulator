using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Trader
      : DerivedPlayerAction
    {
        public Trader(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }        

        public override bool ShouldRevealCardFromHandForCard(GameState gameState, Card card, Card cardFor)
        {
            return playerAction.trashOrder.DoesCardPickerMatch(gameState, cardFor) &&
                   !playerAction.purchaseOrder.DoesCardPickerMatch(gameState, cardFor);
        }
    }
}