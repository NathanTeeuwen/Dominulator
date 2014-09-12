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
    class GetAvailableStrategies
        : IRequestWithJsonResponse
    {
        public object GetResponse(WebService service)
        {            
            return Program.strategyLoader.AllStrategies().Select(action => action.PlayerName).OrderBy(name => name).ToArray();
        }
    }
}
