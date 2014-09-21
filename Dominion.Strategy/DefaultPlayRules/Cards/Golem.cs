using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Golem
       : DerivedPlayerAction
    {
        public Golem(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
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