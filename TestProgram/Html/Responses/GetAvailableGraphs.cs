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
    // commmands        

    [Serializable]
    class GetAvailableGraphs
        : ComparisonDescription,
          IRequestWithJsonResponse
    {
        public object GetResponse(WebService service)
        {
            return WebService.availaleGraphs.Select(t => t.Name).ToArray();
        }
    }
}