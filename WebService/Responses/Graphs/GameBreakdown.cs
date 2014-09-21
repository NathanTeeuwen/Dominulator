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
    [Serializable]
    class GameBreakdown
        : ComparisonDescription,
          IRequestWithJsonResponse
    {
        public object GetResponse(WebService service)
        {
            StrategyComparisonResults comparisonResults = service.GetResultsFor(this);

            if (comparisonResults == null)
            {
                return null;
            }

            var data = new List<object>();
            data.Add(new string[] { "Player", "Percent" });

            for (int index = 0; index < comparisonResults.winnerCount.Length; ++index)
            {
                data.Add(new object[] { comparisonResults.comparison.playerActions[index].PlayerName, comparisonResults.PlayerWinPercent(index) });
            }
            if (comparisonResults.tieCount > 0)
            {
                data.Add(new object[] { "Tie", comparisonResults.TiePercent });
            }

            var options = new Dictionary<string, object>();
            options.Add("title", "Game Breakdown");

            var result = new Dictionary<string, object>();
            result.Add("data", data);
            result.Add("options", options);
            result.Add("type", "pie");
            return result;
        }
    }
}