using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Masterpiece
      : DerivedPlayerAction
    {
        public Masterpiece(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }

        public override int GetCoinAmountToOverpayForCard(GameState gameState, Card card)
        {
            return gameState.Self.AvailableCoins;
        }
    }

}
