using Dominion;
using Dominion.Strategy;
using CardTypes = Dominion.CardTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Program.GeneticAlgorithm;

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
            int numberOfGames = 33;

            GameConfigBuilder builder = new GameConfigBuilder();
            PlayerAction.SetKingdomCards(builder, leftPlayer, rightPlayer);

            var gameConfig = builder.ToGameConfig();
            var rotateWhoStartsFirst = true;

            var strategyComparison = new Dominion.Data.StrategyComparison(leftPlayer, rightPlayer, gameConfig, rotateWhoStartsFirst, numberOfGames);

            var results = strategyComparison.ComparePlayers(
                gameIndex => null,
                gameIndex => null,
                shouldParallel: false,
                gatherStats: false,
                createGameLog: null);

            return results.WinDifference;
        }
    }  
}
