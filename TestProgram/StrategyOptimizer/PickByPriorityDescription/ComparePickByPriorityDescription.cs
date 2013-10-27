using Dominion;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    class ComparePickByPriorityDescription
             : IScoreSpeciesVsEachOther<PickByPriorityDescription>
    {
        public double Compare(PickByPriorityDescription left, PickByPriorityDescription right)
        {
            //System.Console.WriteLine("Comparing: ");
            //left.Write(System.Console.Out);
            //System.Console.WriteLine("");
            //right.Write(System.Console.Out);
            //System.Console.WriteLine("");
            PlayerAction leftPlayer = new PlayerAction("Player1", left.ToCardPicker());
            PlayerAction rightPlayer = new PlayerAction("Player2", right.ToCardPicker());
            return Program.ComparePlayers(leftPlayer, rightPlayer, numberOfGames: 33, logGameCount:0, showCompactScore:false, showVerboseScore:false, createHtmlReport:false);
        }
    }  
}
