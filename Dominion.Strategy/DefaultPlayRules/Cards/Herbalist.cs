using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Herbalist
      : DerivedPlayerAction
    {
        public Herbalist(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }

        public override Card GetCardFromPlayToTopDeckDuringCleanup(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {                  
            return this.playerAction.treasurePlayOrder.GetPreferredCard(gameState, card => gameState.Self.CardsInPlay.Contains(card));
        }
    }

}
