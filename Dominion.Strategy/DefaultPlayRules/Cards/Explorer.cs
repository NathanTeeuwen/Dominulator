using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Explorer
      : DerivedPlayerAction
    {
        public Explorer(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {
            return Dominion.Cards.Province;
        }
    }

}
