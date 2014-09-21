using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class ScryingPool
      : DerivedPlayerAction
    {
        public ScryingPool(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
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
