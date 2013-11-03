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
    class CoinToSpendPerTurn
     : PerTurnGraph,
       IRequestWithJsonResponse
    {
        public object GetResponse(WebService service)
        {
            StrategyComparisonResults comparisonResults = service.GetResultsFor(this);

            return GetLineGraphData(comparisonResults,
                "Coin To Spend Per Turn",
                comparisonResults.statGatherer.coinToSpend);            
        }
    }
}