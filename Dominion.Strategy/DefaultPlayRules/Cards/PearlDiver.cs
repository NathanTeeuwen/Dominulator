using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class PearlDiver
      : DerivedPlayerAction
    {
        public PearlDiver(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }

        public override bool ShouldPutCardOnTopOfDeck(Card card, GameState gameState)
        {
            if (this.playerAction.discardOrder.DoesCardPickerMatch(gameState, card))
                return false;

            return true;
        } 
    }

}
