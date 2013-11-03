using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Dominion;
using Dominion.Strategy;

namespace Program.WebService
{
    class PerTurnGraph
       : ComparisonDescription
    {
        protected object GetLineGraphData(
            StrategyComparisonResults comparisonResults,
            string title,
            ForwardAndReversePerTurnPlayerCounters counters)
        {
            if (!counters.forwardTotal.HasNonZeroData)
            {
                return null;
            }

            int maxTurn = comparisonResults.gameEndOnTurnHistogramData.GetXAxisValueCoveringUpTo(97);

            var options = GoogleChartsHelper.GetLineGraphOptions(
               "title",
               comparisonResults.comparison.playerActions[0].PlayerName,
               comparisonResults.comparison.playerActions[1].PlayerName,
               counters,
               comparisonResults.statGatherer.turnCounters,
               maxTurn);

            return options;

        }
    }
}