using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class RoyalSeal
      : DerivedPlayerAction
    {
        public RoyalSeal(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }

        public override bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
        {
            return !card.isVictory;
        }
    }

}
