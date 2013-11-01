using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Treasury
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Treasury(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
        {
            return true;
        }
    }

}
