using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Masquerade
      : DerivedPlayerAction
    {
        public Masquerade(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }

        public override Card GetCardFromHandToPassLeft(GameState gameState)
        {
            return base.GetCardFromHandToTrash(gameState, c => true, isOptional: false, cardsTrashedSoFar:gameState.emptyCardCollection);
        }
    }

}
