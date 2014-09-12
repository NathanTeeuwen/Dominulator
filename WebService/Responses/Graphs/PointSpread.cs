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
    public class PointSpread
     : ComparisonDescription,
       IRequestWithJsonResponse
    {
        public object GetResponse(WebService service)
        {
            StrategyComparisonResults comparisonResults = service.GetResultsFor(this);

            var options = GoogleChartsHelper.GetLineGraphOptions(
                "Point Spread",
                "Score",
                "Percentage",
                comparisonResults.pointSpreadHistogramData.GetXAxis(),
                comparisonResults.pointSpreadHistogramData.GetYAxis());

            return options;
        }
    }
}