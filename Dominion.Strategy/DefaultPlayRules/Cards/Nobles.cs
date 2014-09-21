using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Nobles
       : DerivedPlayerAction
    {
        public Nobles(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {
            if (gameState.Self.Hand.HasCard(Dominion.Cards.Nobles) && gameState.Self.AvailableActions == 0)
                return PlayerActionChoice.PlusAction;

            if (playerAction.IsGainingCard(Dominion.Cards.Province, gameState))
                return PlayerActionChoice.PlusCard;

            if (gameState.Self.Hand.AnyWhere(card => card.isAction) && gameState.Self.AvailableActions == 0)
                return PlayerActionChoice.PlusAction;            

            return PlayerActionChoice.PlusCard;
        }
    }
}