using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Treasury
      : DerivedPlayerAction
    {
        public Treasury(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }

        public override bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
        {
            return true;
        }
    }

}
