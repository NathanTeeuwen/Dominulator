using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Steward
      : DerivedPlayerAction
    {
        public Steward(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {

            if ((Strategy.CountInHand(Dominion.Cards.Copper, gameState) > 0) && (Strategy.CountInHand(Dominion.Cards.Gold, gameState) < 0))
                return PlayerActionChoice.Trash;
            else if ((Strategy.CountInHand(Dominion.Cards.Copper, gameState) > 0) && (Strategy.CountInHand(Dominion.Cards.Gold, gameState) > 0))
                return PlayerActionChoice.PlusCoin;
            else
                return PlayerActionChoice.PlusCard;

        }

    }

}
