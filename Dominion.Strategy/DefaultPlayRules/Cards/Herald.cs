using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Herald
      : DerivedPlayerAction
    {
        public Herald(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }

        public override int GetCoinAmountToOverpayForCard(GameState gameState, Card card)
        {            
            if (gameState.Self.AvailableBuys > 0 && 
                gameState.Self.AvailableCoins >= Dominion.Cards.Herald.DefaultCoinCost)
            {
                return 0;
            }

            return gameState.Self.AvailableCoins;
        }        
    }

}
