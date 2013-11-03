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
    class GetGameLog
     : ComparisonDescription,
       IRequestWithJsonResponse
    {
        public int gameNumber { get; set; }

        public object GetResponse(WebService service)
        {
            StrategyComparisonResults comparisonResults = service.GetResultsFor(this);
            if (comparisonResults == null)
                return null;

            string result = comparisonResults.comparison.GetHumanReadableGameLog(gameNumber - 1);

            return result;
        }
    }
}