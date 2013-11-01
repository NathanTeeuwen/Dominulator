using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Catacombs
        : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Catacombs(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override PlayerActionChoice ChooseBetween(GameState gameState, IsValidChoice acceptableChoice)
        {
            return PlayerActionChoice.PutInHand;
        }

        public override Card GetCardFromSupplyToGain(GameState gameState, CardPredicate acceptableCard, bool isOptional)
        {
            return playerAction.DefaultGetCardFromSupplyToGain(gameState, acceptableCard, isOptional);
        }
    }
}