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
    public class WebService
    {
        static string root = "/dominion";
        static string baseUrl = "http://localhost:8081" + root + "/";
        static string resourcePrefix = root + "/resources/";
        
        static JavaScriptSerializer js = new JavaScriptSerializer();

        public static Type[] availaleGraphs = new Type[]
        {
            typeof(GameBreakdown),
            typeof(PointSpread),
            typeof(ProbabilityPlayerIsAheadAtEndOfRound),
            typeof(ProbabilityGameEndingOnTurn),
            typeof(ProbabilityGameBeingOverByTurn),
            typeof(VictoryPointTotalPerTurn),            
            typeof(CoinToSpendPerTurn)
        };

        static Type[] services = new Type[]
        {            
            typeof(GetAvailableStrategies),
            typeof(StrategyComparisonRequest),
            typeof(GetKingdomCardImages),
            typeof(GetStrategyText),
            typeof(GetAvailableGraphs),
            typeof(GetGameLog),                        
            typeof(CheckStrategyCode),
            typeof(GetAvailableKingdomCards)
        };

        private string defaultPage = Program.GetEmbeddedContent("Dominulator.html");

        private static Dictionary<string, PlayerAction> playerActionCache = new Dictionary<string, PlayerAction>();

        internal static PlayerAction GetPlayerActionFromCode(string code)
        {
            PlayerAction result;
            if (!playerActionCache.TryGetValue(code, out result))
            {
                result = Program.strategyLoader.GetPlayerActionFromCode(code);
                playerActionCache[code] = result;
            }            
            return result;
        }

        private Dictionary<ComparisonDescription, StrategyComparisonResults> resultsCache = new Dictionary<ComparisonDescription, StrategyComparisonResults>();
        private Dictionary<string, Type> mapNameToServiceType;

        public WebService()
        {
            this.mapNameToServiceType = new Dictionary<string, Type>();
            foreach (var type in services)
            {
                this.mapNameToServiceType.Add(type.Name, type);
            }
            foreach (var type in availaleGraphs)
            {
                this.mapNameToServiceType.Add(type.Name, type);
            }
        }

        internal StrategyComparisonResults GetResultsFor(ComparisonDescription descr)
        {
            StrategyComparisonResults result = null;
            if (!this.resultsCache.TryGetValue(descr, out result))
            {
                GameConfigBuilder builder = new GameConfigBuilder();
                PlayerAction playerAction1 = descr.Player1Action;
                PlayerAction playerAction2 = descr.Player2Action;

                if (playerAction1 != null && playerAction2 != null)
                {
                    System.Console.WriteLine("Playing {0} vs {1}", playerAction1.PlayerName, playerAction2.PlayerName);
                    PlayerAction.SetKingdomCards(builder, new PlayerAction[] { playerAction1, playerAction2 });

                    var gameConfig = builder.ToGameConfig();

                    var strategyComparison = new StrategyComparison(playerAction1, playerAction2, gameConfig, rotateWhoStartsFirst: true, numberOfGames: 1000);
                    result = strategyComparison.ComparePlayers(
                        gameIndex => null,
                        gameIndex => null,
                        shouldParallel: true,
                        gatherStats: true);

                    this.resultsCache.Add(descr, result);
                }
                else
                {
                    this.resultsCache.Add(descr, null);
                }
            }

            return result;
        }

        private string LocalIPAddress()
        {            
            System.Net.IPHostEntry host;
            string localIP = "";
            host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (System.Net.IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                localIP = ip.ToString();
                break;
                }
            }
            return localIP;         
        }

        private bool IsConnectedToInternet()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }

        private bool TryConnectLocalIP()
        {            
            using (var listener = new System.Net.HttpListener())
            {
                listener.Prefixes.Add(ExternalUrl);                
                try
                {
                    listener.Start();                    
                }
                catch(System.Net.HttpListenerException e)
                {
                    e.ToString();
                    //System.Console.WriteLine(e);
                    return false;
                }
                listener.Close();
            }

            return true;
        }

        private string ExternalUrl
        {
            get
            {
                return "http://" + LocalIPAddress() + ":8081" + root + "/";
            }
        }

        public void Run()
        {
            if (!IsConnectedToInternet())
            {
                System.Console.WriteLine("Not connected to a network");
                return;
            }

            bool useLocalIP = true;
            if (!TryConnectLocalIP())
            {
                useLocalIP = false;
                System.Console.WriteLine("Could not connect through local IP address");
            }
            
            System.Console.WriteLine("Connect using {0}", baseUrl.TrimEnd('/'));
            if (useLocalIP)
            {
                System.Console.WriteLine("Connect using {0}", ExternalUrl.TrimEnd('/'));
            }

            using (var listener = new System.Net.HttpListener())
            {
                listener.Prefixes.Add(baseUrl);
                if (useLocalIP)
                { 
                    listener.Prefixes.Add(ExternalUrl);
                }

                listener.Start();
                while (true)
                {
                    System.Net.HttpListenerContext ctx = listener.GetContext();
                    System.Net.HttpListenerRequest request = ctx.Request;
                    System.Console.WriteLine(request.Url);

                    System.Net.HttpListenerResponse response = ctx.Response;

                    string urlString = request.Url.ToString();
                    string rawUrl = request.RawUrl.ToString();

                    string responseText = null;
                    
                    byte[] reponseBuffer = null;
                    if (rawUrl == root)
                    {
                        responseText = defaultPage;
                        responseText = responseText.Replace(baseUrl, urlString + "/");
                        response.ContentType = "text/HTML";
                    }
                    else if (rawUrl.StartsWith(WebService.resourcePrefix))
                    {
                        string resourceName = rawUrl.Remove(0, WebService.resourcePrefix.Length);
                        if (rawUrl.EndsWith(".jpg"))
                        {
                            reponseBuffer = Program.GetEmbeddedContentAsBinary(resourceName);
                            response.ContentType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                        }
                        else
                        {
                            responseText = Program.GetEmbeddedContent(resourceName);
                            response.ContentType = "text/css";
                        }
                    }
                    else if (rawUrl.StartsWith(root + "/"))
                    {
                        var streamReader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding);
                        var jsonRequest = streamReader.ReadToEnd();

                        string requestedPage = rawUrl.Remove(0, (root + "/").Length);
                        object unserializedObject = null;

                        Type serviceType;
                        if (mapNameToServiceType.TryGetValue(requestedPage, out serviceType))
                        {
                            unserializedObject = js.Deserialize(jsonRequest, serviceType);
                            if (unserializedObject == null)
                            {
                                unserializedObject = Activator.CreateInstance(serviceType);
                            }
                        }
                        else
                        {
                            System.Console.WriteLine("Unrecognized Service");
                        }

                        if (unserializedObject is IRequestWithHtmlResponse)
                        {
                            responseText = ((IRequestWithHtmlResponse)unserializedObject).GetResponse(this);
                        }
                        else if (unserializedObject is IRequestWithJsonResponse)
                        {
                            object o = ((IRequestWithJsonResponse)unserializedObject).GetResponse(this);
                            var serialized = js.Serialize(o);
                            responseText = serialized;
                        }
                        response.ContentType = "text/html";
                    }                    

                    if (response.ContentType != null && response.ContentType.StartsWith("text") && responseText != null)
                    {
                        response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
                        reponseBuffer = System.Text.Encoding.UTF8.GetBytes(responseText);
                    }

                    if (reponseBuffer != null)
                    {
                        //These headers to allow all browsers to get the response
                        response.Headers.Add("Access-Control-Allow-Credentials", "true");
                        response.Headers.Add("Access-Control-Allow-Origin", "*");
                        response.Headers.Add("Access-Control-Origin", "*");

                        //response.StatusCode = 200;
                        //response.StatusDescription = "OK";                    
                        // Get a response stream and write the response to it.
                        response.ContentLength64 = reponseBuffer.Length;
                        System.IO.Stream output = response.OutputStream;
                        output.Write(reponseBuffer, 0, reponseBuffer.Length);
                        output.Close();
                    }
                    response.Close();
                }
            }
        }        
    }
}
