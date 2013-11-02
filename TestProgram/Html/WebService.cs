using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Dominion;
using Dominion.Strategy;

namespace Program
{
    static class GoogleChartsHelper
    {
        public static object GetLineGraphOptions(           
           string title,           
           string player1Name,
           string player2Name,
           ForwardAndReversePerTurnPlayerCounters forwardAndReverseCounters,
           ForwardAndReversePerTurnPlayerCounters turnCounters,
           int throughTurn)
        {
            return GetLineGraphOptions(
                title,
                "Turn",                               
                player1Name,
                player2Name,
                Enumerable.Range(1, throughTurn).ToArray(),
                forwardAndReverseCounters.forwardTotal.GetAveragePerTurn(0, throughTurn, turnCounters.forwardTotal),
                forwardAndReverseCounters.forwardTotal.GetAveragePerTurn(1, throughTurn, turnCounters.forwardTotal));
        }

        public static object GetLineGraphOptions(
           string title,
           string xAxisLabel,
           string series1Label,
           string series2Label,
           int[] xAxis,
           float[] series1,
           float[] series2)
        {
            return GetLineGraphOptions(title, xAxisLabel, new string[] { series1Label, series2Label }, xAxis, new float[][] { series1, series2 });
        }

        public static object GetLineGraphOptions(            
            string title,
            string xAxisLabel,
            string seriesLabel,
            int[] xAxis,
            float[] seriesData)
        {
            return GetLineGraphOptions(title, xAxisLabel, new string[] { seriesLabel }, xAxis, new float[][] { seriesData });
        }

        public static object GetLineGraphOptions(            
            string title,
            string xAxisLabel,
            string[] seriesLabels,
            int[] xAxis,
            float[][] seriesData)
        {
            var data = new List<object>();

            var labels = new List<string>();
            labels.Add(xAxisLabel);
            foreach (string label in seriesLabels)
                labels.Add(label);

            int numberOfDataPoints = seriesData[0].Length;
            data.Add(labels);
            for (int index = 0; index < numberOfDataPoints; ++index)
            {
                var row = new List<object>();
                row.Add(xAxis[index]);
                for (int i = 0; i < seriesData.Length; ++i)
                {
                    row.Add(seriesData[i][index]);
                }
                data.Add(row);
            }

            var options = new Dictionary<string, object>();
            options.Add("title", "Point Spread");
            options.Add("hAxis", GetHAxisOptions(xAxis));

            var result = new Dictionary<string, object>();
            result.Add("data", data);
            result.Add("options", options);
            result.Add("type", "line");
            
            return result;
        }

        public static object GetHAxisOptions(int[] xAxis)
        {
            int multiplesOfFifteen = (xAxis.Length + 14)/15;

            var gridLines = new Dictionary<string, object>();
            gridLines.Add("count", xAxis.Length / multiplesOfFifteen);

            var hAxis = new Dictionary<string, object>();
            hAxis.Add("gridlines", gridLines);

            return hAxis;
        }
    }

    interface IRequestWithHtmlResponse
    {
        string GetResponse(WebService service);
    }

    interface IRequestWithJsonResponse
    {
        object GetResponse(WebService service);
    }

    [Serializable]
    public class ComparisonDescription
    {
        public string player1 { get; set; }
        public string player2 { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals((ComparisonDescription)obj);
        }

        public bool Equals(ComparisonDescription other)
        {
 	         return this.player1 == other.player1 &&
                    this.player2 == other.player2;
        }

        public override int GetHashCode()
        {
 	        return this.player1.GetHashCode() ^ 
                   this.player2.GetHashCode();
        }

        public PlayerAction Player1Action
        {
            get
            {
                return GetPlayerActionFromCode(this.player1);
            }
        }

        public PlayerAction Player2Action
        {
            get
            {
                return GetPlayerActionFromCode(this.player2);
            }
        }

        private PlayerAction GetPlayerActionFromCode(string code)
        {
            return Program.strategyLoader.GetPlayerActionFromCode(code);            
        }
    }

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

    public class KingdomCardImages
        : ComparisonDescription,
          IRequestWithJsonResponse
    {
        public object GetResponse(WebService service)
        {
            var playerActions = new List<PlayerAction>();
            var player1 = this.Player1Action;
            if (player1 != null)
                playerActions.Add(player1);
            var player2 = this.Player2Action;
            if (player2 != null)
                playerActions.Add(player2);

            var builder = new GameConfigBuilder();
            PlayerAction.SetKingdomCards(builder, playerActions.ToArray());            
            return builder.ToGameConfig().kingdomPiles.OrderBy(card => card.DefaultCoinCost).Select(card => GetCardImageName(card)).ToArray();
        }

        private string GetCardImageName(Card card)
        {
            return "cards/" + card.ProgrammaticName + ".jpg";
        }
    }

    class PerTurnGraph
       : ComparisonDescription
    {
        protected object GetLineGraphData(
            StrategyComparisonResults comparisonResults,
            string title,
            ForwardAndReversePerTurnPlayerCounters counters)
        {
            int maxTurn = comparisonResults.gameEndOnTurnHistogramData.GetXAxisValueCoveringUpTo(97);

            var options = GoogleChartsHelper.GetLineGraphOptions(
               "title",
               comparisonResults.comparison.playerActions[0].PlayerName,
               comparisonResults.comparison.playerActions[1].PlayerName,
               counters,
               comparisonResults.statGatherer.turnCounters,
               maxTurn);

            return options;
        }
    }   

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
                data.Add(new object[] { comparisonResults.comparison.playerActions[index].name, comparisonResults.PlayerWinPercent(index) });              
            }
            if (comparisonResults.tieCount > 0)
            {
                data.Add(new object[] { "Tie", comparisonResults.TiePercent} );
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

    public class PointSpread
     : ComparisonDescription, 
       IRequestWithJsonResponse
    {
        public object GetResponse(WebService service)
        {
            StrategyComparisonResults comparisonResults = service.GetResultsFor(this);

            var options = GoogleChartsHelper.GetLineGraphOptions(
                "Point Spread",
                "Score", 
                "Percentage",
                comparisonResults.pointSpreadHistogramData.GetXAxis(),
                comparisonResults.pointSpreadHistogramData.GetYAxis());
         
            return options;
        }
    }   
    /*
    class ProbabilityPlayerIsAheadAtEndOfRound
        : PerTurnGraph,
          IRequestWithJsonResponse
    {
        public object GetResponse(WebService service)
        {
            StrategyComparisonResults comparisonResults = service.GetResultsFor(this);

            return GetLineGraphData(comparisonResults,
                "Probability player is ahead in points at end of round ",
                comparisonResults.statGatherer.oddsOfBeingAheadOnRoundEnd);
        }
    }
    */
    class VictoryPointTotalPerTurn
       : PerTurnGraph,
         IRequestWithJsonResponse
    {
        public object GetResponse(WebService service)
        {
            StrategyComparisonResults comparisonResults = service.GetResultsFor(this);

            return GetLineGraphData(comparisonResults,
                "Victory Point Total Per Turn",
                comparisonResults.statGatherer.victoryPointTotal);
        }
    }
    /*
    public class ProbabilityGameEndingOnTurn
     : ComparisonDescription,
       IRequestWithJsonResponse
    {
        public object GetResponse(WebService service)
        {
            StrategyComparisonResults comparisonResults = service.GetResultsFor(this);

            var options = GoogleChartsHelper.GetLineGraphOptions(
                "Probability of Game Ending on Turn",
                "Score",
                "Percentage",
                comparisonResults.pointSpreadHistogramData.GetXAxis(),
                comparisonResults.pointSpreadHistogramData.GetYAxis());

            return options;
        }
    } */ 

    // commmands

    class GetGameLog
     : ComparisonDescription,
       IRequestWithJsonResponse
    {
        public int gameNumber { get; set; }
        
        public object GetResponse(WebService service)
        {
            StrategyComparisonResults comparisonResults = service.GetResultsFor(this);
            if (comparisonResults == null)
                return null;

            string result = comparisonResults.comparison.GetHumanReadableGameLog(gameNumber-1);

            return result;
        }
    }

    [Serializable]
    class GetAvailableStrategies
        : IRequestWithJsonResponse
    {
        public object GetResponse(WebService service)
        {
            return Program.AllBuiltInStrategies().Select(action => action.PlayerName).OrderBy(name => name).ToArray();
        }
    }

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

    public class WebService
    {
        static string baseUrl = "http://localhost:8081/dominion/";
        static string resourcePrefix = baseUrl + "resources/";
        static JavaScriptSerializer js = new JavaScriptSerializer();

        public static Type[] availaleGraphs = new Type[]
        {
            typeof(GameBreakdown),
            typeof(PointSpread),
            //typeof(ProbabilityPlayerIsAheadAtEndOfRound),
            typeof(VictoryPointTotalPerTurn),            
        };

        static Type[] services = new Type[]
        {            
            typeof(GetAvailableStrategies),
            typeof(StrategyComparisonRequest),
            typeof(KingdomCardImages),
            typeof(GetStrategyText),
            typeof(GetAvailableGraphs),
            typeof(GetGameLog),                        
        };

        private string defaultPage = HtmlRenderer.GetEmbeddedContent("Dominulator.html");        
        
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
                    System.Console.WriteLine("Playing {0} vs {1}", playerAction1.name, playerAction2.name);
                    PlayerAction.SetKingdomCards(builder, new PlayerAction[] { playerAction1, playerAction2 });

                    var gameConfig = builder.ToGameConfig();

                    var strategyComparison = new StrategyComparison(playerAction1, playerAction2, gameConfig, firstPlayerAdvantage: false, numberOfGames: 1000);
                    result = strategyComparison.ComparePlayers(
                        gameIndex => null,
                        gameIndex => null,
                        shouldParallel: true,
                        gatherStats: true);

                    this.resultsCache.Add(descr, result);
                }                
            }

            return result;
        }

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
                byte[] reponseBuffer = null;
                if (urlString + "/" == baseUrl)
                {
                    responseText = defaultPage;
                    response.ContentType = "text/HTML";
                }
                else if (urlString.StartsWith(WebService.resourcePrefix))
                {
                    string resourceName = urlString.Remove(0, WebService.resourcePrefix.Length);                    

                    if (urlString.EndsWith(".jpg"))
                    {
                        reponseBuffer = HtmlRenderer.GetEmbeddedContentAsBinary(resourceName);                                                
                        response.ContentType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                    }
                    else
                    {
                        responseText = HtmlRenderer.GetEmbeddedContent(resourceName);
                        response.ContentType = "text/css";
                    }
                }
                else if (urlString.StartsWith(baseUrl))
                {
                    var streamReader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding);
                    var jsonRequest = streamReader.ReadToEnd();

                    string requestedPage = urlString.Remove(0, baseUrl.Length);
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
                
                if (response.ContentType.StartsWith("text"))
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
