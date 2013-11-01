using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Scheme
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Scheme(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }
        
        public override Card GetCardFromPlayToTopDeckDuringCleanup(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            // put the most preferred card on top first
            return playerAction.actionOrder.GetPreferredCard(gameState, card => gameState.Self.CardsInPlay.HasCard(card) && acceptableCard(card));
        }
    }

}
