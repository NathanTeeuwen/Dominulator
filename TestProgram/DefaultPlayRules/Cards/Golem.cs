using Dominion;
using System;
using System.Linq;

namespace Program.DefaultStrategies
{
    internal class Golem
       : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Golem(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card ChooseCardToPlayFirst(GameState gameState, Card card1, Card card2)
        {
            Card result = playerAction.actionOrder.GetPreferredCard(
                gameState,
                card => card == card1 || card == card2);

            // choose a reasonable default
            if (result == null)
            {
                result = playerAction.defaultActionOrder.GetPreferredCard(
                    gameState,
                    card => card == card1 || card == card2);
            }

            return result;
        }
    }
}