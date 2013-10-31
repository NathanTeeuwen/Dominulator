using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Dominion;

namespace Program
{    
    interface IRequestWithHtmlResponse
    {
        string GetResponse();
    }

    interface IRequestWithJsonResponse
    {
        object GetResponse();
    }

    [Serializable]
    public class StrategyComparisonRequest 
        : IRequestWithHtmlResponse
    {
        public string player1 { get; set; }
        public string player2 { get; set; }

        public string GetResponse()
        {
            /*
            GameConfigBuilder builder = new GameConfigBuilder();
            PlayerAction playerAction1;
            PlayerAction playerAction2;
            PlayerAction.SetKingdomCards(builder, player1, player2);

            builder.useColonyAndPlatinum = useColonyAndPlatinum;
            builder.useShelters = useShelters;
            builder.CardSplit = split;

            if (startingDeckPerPlayer != null)
                builder.SetStartingDeckPerPlayer(startingDeckPerPlayer);

            var gameConfig = builder.ToGameConfig();            

            var generator = new HtmlReportGenerator(
                gameConfig,
                firstPlayerAdvantage:false,
                numberOfGames:1000,
                new PlayerAction[] { playerAction1, playerAction2},
            */
            return this.player1 + " vs " + this.player2;
        }
    }

    [Serializable]
    public class GetAvailableStrategies
        : IRequestWithJsonResponse
    {
        public object GetResponse()
        {
            return Program.AllBuiltInStrategies().Select(action => action.PlayerName).OrderBy(name => name).ToArray();
        }
    }

    class WebService
    {
        static string baseUrl = "http://localhost:8081/dominion/";
        static string defaultPage = HtmlRenderer.GetEmbeddedContent("Dominulator.html");
        static JavaScriptSerializer js = new JavaScriptSerializer();

        public void Run()
        {
            var listener = new System.Net.HttpListener();
            listener.Prefixes.Add(baseUrl);
            listener.Start();
            while (true)
            {
                System.Net.HttpListenerContext ctx = listener.GetContext();
                System.Net.HttpListenerRequest request = ctx.Request;
                System.Console.WriteLine(request.Url);

                System.Net.HttpListenerResponse response = ctx.Response;                

                string urlString = request.Url.ToString();

                string responseText = null;

                urlString.TrimEnd('/');

                if (urlString + "/" == baseUrl)
                {
                    responseText = defaultPage;
                    response.ContentType = "text/html";
                }
                else if (urlString.StartsWith(baseUrl))
                {
                    var streamReader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding);
                    var jsonRequest = streamReader.ReadToEnd();                                       

                    string requestedPage = urlString.Remove(0, baseUrl.Length);
                    object unserializedObject = null;

                    if (requestedPage == "StrategyComparisonRequest")                    
                    {
                        unserializedObject = js.Deserialize(jsonRequest, typeof(StrategyComparisonRequest));
                    }
                    else if (requestedPage == "GetAvailableStrategies")
                    {
                        unserializedObject = new GetAvailableStrategies();
                    }

                    if (unserializedObject is IRequestWithHtmlResponse)
                    {
                        responseText = ((IRequestWithHtmlResponse)unserializedObject).GetResponse();
                    }
                    else if (unserializedObject is IRequestWithJsonResponse)
                    {
                        object o = ((IRequestWithJsonResponse)unserializedObject).GetResponse();
                        var serialized = js.Serialize(o);
                        responseText = serialized;                        
                    }
                }                

                if (responseText != null)
                {
                    //These headers to allow all browsers to get the response
                    response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    response.Headers.Add("Access-Control-Allow-Origin", "*");
                    response.Headers.Add("Access-Control-Origin", "*");
                    response.ContentType = "text/html";
                    response.ContentEncoding = System.Text.UTF8Encoding.UTF8;

                    response.StatusCode = 200;
                    response.StatusDescription = "OK";
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseText);
                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
                response.Close();
            }
        }
    }
}
