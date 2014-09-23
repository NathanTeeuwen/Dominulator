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
            if ((Strategy.CountOfPile(Dominion.Cards.Province, gameState) > 7) &&
                    ((Strategy.CountInHand(Dominion.Cards.Copper, gameState) > 1)
                    || (Strategy.CountInHand(Dominion.Cards.Estate, gameState) > 1)
                   // ||((Strategy.CountInHand(Dominion.Cards.Estate, gameState) > 0) && (Strategy.CountInHand(Dominion.Cards.Copper, gameState) > 0))
                    ))
                return PlayerActionChoice.Trash;
            else if ((Strategy.CountInHand(Dominion.Cards.Silver, gameState) < 3) && (Strategy.CountInHand(Dominion.Cards.Gold, gameState) < 0))
                return PlayerActionChoice.PlusCoin;
            else
                return PlayerActionChoice.PlusCard;

        }

    }

}
