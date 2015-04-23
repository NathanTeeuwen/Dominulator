using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Urchin
      : DerivedPlayerAction
    {
        public Urchin(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }

        public override bool ShouldTrashCard(GameState gameState, Card card)
        {
            if (card == Dominion.Cards.Urchin &&
                gameState.CurrentContext.IsSelfPlaying(gameState) && 
                gameState.CurrentContext.Reason == CardContextReason.CardReacting && 
                gameState.CurrentContext.CurrentCard.isAttack)
            {
                if (Strategy.CountAllOwned(Dominion.Cards.Mercenary, gameState) == 0)
                    return true;
                else
                    return false;
            }

            return base.ShouldTrashCard(gameState, card);
        }

    }

}
