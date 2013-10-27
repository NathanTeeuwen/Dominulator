using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    class CompareBigMoneyWithCardDescription
             : IScoreSpecies<BigMoneyWithCardDescription>
    {
        PlayerAction playerAction;

        public CompareBigMoneyWithCardDescription(PlayerAction playerAction)
        {
            this.playerAction = playerAction;
        }

        public double Compare(BigMoneyWithCardDescription left, BigMoneyWithCardDescription right)
        {
            double leftScore = left.GetScoreVs(this.playerAction);
            double rightScore = right.GetScoreVs(this.playerAction);

            return leftScore - rightScore;
        }        
    }  
}
