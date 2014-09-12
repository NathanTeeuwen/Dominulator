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
    [Serializable]
    public class GetStrategyText
        : IRequestWithJsonResponse
    {
        public string name { get; set; }

        public object GetResponse(WebService service)
        {
            return Program.strategyLoader.GetPlayerSource(this.name);
        }
    }
}