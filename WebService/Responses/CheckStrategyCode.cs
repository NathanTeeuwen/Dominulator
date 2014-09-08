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
    public class CheckStrategyCode
        : IRequestWithJsonResponse
    {
        public string code { get; set; }

        public object GetResponse(WebService service)
        {
            var result = new Dictionary<string, object>();
            
            DynamicStrategyLoader.CompiledResult compiledResult = Program.strategyLoader.DynamicallyLoadFromSource(this.code);
            if (compiledResult.error != null)
            {
                result.Add("line", compiledResult.error.Line);
                result.Add("column", compiledResult.error.Column);
                result.Add("message", compiledResult.error.ErrorText);
            }
            return result;
        }
    }
}