using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{
    internal class Messenger
       : DerivedPlayerAction
    {
        public Messenger(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }
        
        public override bool ShouldPutDeckInDiscard(GameState gameState)
        {            
            return Chancellor.ShouldPutDeckInDiscardBasedOnAverageValue(gameState);                     
        }
    }
}