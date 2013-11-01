using Dominion;
using Dominion.Strategy;
using System;
using System.Linq;

namespace Dominion.Strategy.DefaultPlayRules.Cards
{   
    internal class BandOfMisfits
       : UnimplementedPlayerAction
    {
        private readonly PlayerAction playerAction;

        public BandOfMisfits(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public override Card GetCardFromSupplyToPlay(GameState gameState, CardPredicate acceptableCard)
        {
            throw new NotImplementedException();
        }
    }
}
