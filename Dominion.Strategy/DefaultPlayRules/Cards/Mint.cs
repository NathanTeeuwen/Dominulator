using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Mint
      : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public Mint(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {
            return playerAction.purchaseOrder.GetPreferredCard(gameState, acceptableCard);            
        }        
    }
}