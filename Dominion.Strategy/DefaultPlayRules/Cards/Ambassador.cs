using Dominion;
using System;
using System.Linq;

namespace Program.DefaultPlayRules
{
    internal class Ambassador
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Ambassador(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {
            return playerAction.trashOrder.GetPreferredCard(gameState, card => gameState.Self.Hand.HasCard(card) && acceptableCard(card));
        }

        public override int GetCountToReturnToSupply(Card card, GameState gameState)
        {
            return 2;
        }
    }

}
