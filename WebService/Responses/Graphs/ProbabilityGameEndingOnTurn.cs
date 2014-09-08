using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Dominion;
using Dominion.Strategy;
using Dominion.Data;

namespace Program.WebService
{    
    public class ProbabilityGameEndingOnTurn
     : ComparisonDescription,
       IRequestWithJsonResponse
    {
        public object GetResponse(WebService service)
        {
            StrategyComparisonResults comparisonResults = service.GetResultsFor(this);

            int maxTurn = comparisonResults.gameEndOnTurnHistogramData.GetXAxisValueCoveringUpTo(97);

            var options = GoogleChartsHelper.GetLineGraphOptions(
                "Probability of Game Ending on Turn",
                "Score",
                "Percentage",
                comparisonResults.gameEndOnTurnHistogramData.GetXAxis(maxTurn),
                comparisonResults.gameEndOnTurnHistogramData.GetYAxis(maxTurn));

            return options;
        }
    }
}