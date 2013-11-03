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

            var options = GoogleChartsHelper.GetLineGraphOptions(
                "Probablity of Game Being over by turn",
                "Score",
                "Percentage",
                comparisonResults.pointSpreadHistogramData.GetXAxis(),
                comparisonResults.pointSpreadHistogramData.GetYAxisIntegrated());

            return options;
        }
    }
}