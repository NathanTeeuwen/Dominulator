using Dominion;
using System;
using System.Linq;

namespace Program.DefaultPlayRules
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
