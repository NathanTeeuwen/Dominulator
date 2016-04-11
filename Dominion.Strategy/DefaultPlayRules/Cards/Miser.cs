using Dominion;
using Dominion.Strategy;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Miser
      : DerivedPlayerAction
    {
        public Miser(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {
            if (acceptableChoice(PlayerActionChoice.PutCopperOnTavernMat))
            {
                if (Strategy.CountInHand(Dominion.Cards.Copper, gameState) > 0)
                    return PlayerActionChoice.PutCopperOnTavernMat;
            }

            if (acceptableChoice(PlayerActionChoice.PlusCoinPerCoppperOnTavernMat))
            {
                return PlayerActionChoice.PlusCoinPerCoppperOnTavernMat;
            }
            return base.ChooseBetween(gameState, acceptableChoice);
        }

    }

}
