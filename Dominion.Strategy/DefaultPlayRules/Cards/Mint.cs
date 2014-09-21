using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Mint
      : DerivedPlayerAction
    {
        public Mint(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override Card GetCardFromHandToReveal(GameState gameState, CardPredicate acceptableCard)
        {
            return playerAction.purchaseOrder.GetPreferredCard(gameState, acceptableCard);            
        }        
    }
}