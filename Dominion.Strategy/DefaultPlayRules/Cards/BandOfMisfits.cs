using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{   
    internal class BandOfMisfits
       : DerivedPlayerAction
    {
        public BandOfMisfits(DefaultPlayerAction playerAction)
            : base(playerAction)
        {
        }

        public override Card GetCardFromSupplyToPlay(GameState gameState, CardPredicate acceptableCard)
        {
            throw new NotImplementedException();
        }
    }
}
