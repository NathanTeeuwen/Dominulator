using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Amulet
      : DerivedPlayerAction
    {
        public Amulet(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {
            if ( Strategy.CountOfPile(Dominion.Cards.Province, gameState) > 7 &&
                 Strategy.CountInHandFrom(this.playerAction.trashOrder, gameState) >= 2 )
                return PlayerActionChoice.Trash;
            else if ((gameState.Self.ExpectedCoinValueAtEndOfTurn == 5) || 
                    (gameState.Self.ExpectedCoinValueAtEndOfTurn == 7))
                return PlayerActionChoice.PlusCoin;
            else
                return PlayerActionChoice.GainCard;
        }

    }

}
