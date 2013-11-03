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
    public class StrategyComparisonRequest
        : ComparisonDescription,
          IRequestWithHtmlResponse
    {
        public string GetResponse(WebService service)
        {
            StrategyComparisonResults comparisonResults = service.GetResultsFor(this);
            var HtmlReportGenerator = new HtmlReportGenerator(comparisonResults);
            return HtmlReportGenerator.CreateHtmlReport();
        }
    }
}
