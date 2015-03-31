using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Vault
      : DerivedPlayerAction
    {
        public Vault(DefaultPlayerAction playerAction)
            : base(playerAction)
        {            
        }

        public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {
            // some other player has played a vault.  U get to choose whether to discard cards.
            if (gameState.Self != gameState.players.CurrentPlayer)
            {
                if (base.playerAction.discardOrder.CountInSetMatching(gameState, gameState.Self.Hand) >= 2)                
                    return PlayerActionChoice.Discard;
                return PlayerActionChoice.Nothing;
            }

            throw new System.Exception("Unexpected choice for vault");
        }
    }

}
