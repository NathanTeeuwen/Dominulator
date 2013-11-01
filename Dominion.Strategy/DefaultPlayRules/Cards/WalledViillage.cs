using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class WalledVillage
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public WalledVillage(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
        {
            return true;
        }
    }

}
