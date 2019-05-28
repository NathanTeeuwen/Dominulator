using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class SecretChamber
      : DerivedPlayerAction
    {
        public SecretChamber(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override bool ShouldRevealCardFromHand(GameState gameState, Card card)
        {
            return true;
        }
    }

}
