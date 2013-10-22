using Dominion;
using System;
using System.Linq;

namespace Program.DefaultStrategies
{
    internal class Nobles
       : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Nobles(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {            
            if (gameState.Self.Hand.HasCard(Cards.Nobles) && gameState.Self.AvailableActions == 0)
                return PlayerActionChoice.PlusAction;

            if (playerAction.IsGainingCard(Cards.Province, gameState))
                return PlayerActionChoice.PlusCard;

            if (gameState.Self.Hand.AnyWhere(card => card.isAction) && gameState.Self.AvailableActions == 0)
                return PlayerActionChoice.PlusAction;            

            return PlayerActionChoice.PlusCard;
        }
    }
}