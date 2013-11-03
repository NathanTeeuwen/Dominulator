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
    public class ProbabilityGameBeingOverByTurn
     : ComparisonDescription,
       IRequestWithJsonResponse
    {
        public object GetResponse(WebService service)
        {
            StrategyComparisonResults comparisonResults = service.GetResultsFor(this);

            int maxTurn = comparisonResults.gameEndOnTurnHistogramData.GetXAxisValueCoveringUpTo(97);

            var options = GoogleChartsHelper.GetLineGraphOptions(
                "Probablity of Game Being over by turn",
                "Score",
                "Percentage",
                comparisonResults.gameEndOnTurnHistogramData.GetXAxis(maxTurn),
                comparisonResults.gameEndOnTurnHistogramData.GetYAxisIntegrated(maxTurn));

            return options;
        }
    }
}