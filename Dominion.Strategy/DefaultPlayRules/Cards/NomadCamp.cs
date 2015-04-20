using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class NomadCamp
      : DerivedPlayerAction
    {
        public NomadCamp(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
        {
            return card == Dominion.Cards.NomadCamp;
        }
    }

}
