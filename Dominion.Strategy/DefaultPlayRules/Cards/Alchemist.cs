using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Alchemist
      : DerivedPlayerAction
    {
        public Alchemist(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }

        public override bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
        {
            return true;
        }
    }

}
