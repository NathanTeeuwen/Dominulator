using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class ScryingPool
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public ScryingPool(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldPlayerDiscardCardFromDeck(GameState gameState, PlayerState player, Card card)
        {
            if (player == gameState.Self)
            {
                return !card.isAction;
            }
            else
            {
                return !playerAction.discardOrder.DoesCardPickerMatch(gameState, card);
            }
        }
    }

}
