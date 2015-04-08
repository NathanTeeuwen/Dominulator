using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Island
      : DerivedPlayerAction
    {
        public Island(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }

        public override Card GetCardFromHandToIsland(GameState gameState)
        {
            Card result = gameState.Self.Hand.FindCard(card => card.isVictory && card != Dominion.Cards.Island);
            if (result != null)
                return result;

            if (gameState.Self.Hand.HasCard(Dominion.Cards.Island))
                return Dominion.Cards.Island;

            if (!gameState.Self.Hand.Any)
                return null;

            result = this.playerAction.discardOrder.GetPreferredCard(gameState, gameState.Self.Hand.HasCard);
            if (result != null)
                return result;

            return base.GetCardFromHandToDiscard(gameState, c => true, isOptional: false);
        }
    }

}
