using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class StoneMason
      : DerivedPlayerAction
    {
        public StoneMason(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }

        public override int GetCoinAmountToOverpayForCard(GameState gameState, Card card)
        {
            Card cardToOverpayFor = this.playerAction.purchaseOrder.GetPreferredCard(gameState, c => c.CurrentCoinCost(gameState.Self) <= gameState.Self.AvailableCoins);
            if (cardToOverpayFor == null)
                return 0;
            return cardToOverpayFor.CurrentCoinCost(gameState.Self);            
        }        
    }

}
