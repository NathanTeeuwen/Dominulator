using System;
using System.Threading.Tasks;

using Windows.Web.Http;

namespace Dominulator
{
    public class DominulatorWebClient
    {
        public async Task<Windows.Data.Json.JsonValue> GetAllCards()
        {
            var uri = new Uri("http://localhost:8081/dominion" + "/GetAvailableKingdomCards");
            
            try
            {                
                using (var httpClient = new HttpClient())
                {                    
                    string result = await httpClient.GetStringAsync(uri);
                    Windows.Data.Json.JsonValue jsonValue = Windows.Data.Json.JsonValue.Parse(result);
                    return jsonValue;
                }
            }
            catch
            {
                return null;
            }            
        }
    }
}
