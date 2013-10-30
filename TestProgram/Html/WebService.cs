using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{    
    interface IJSONWithHtmlResponse
    {
        string GetResponse();
    }

    [Serializable]
    public class StrategyComparisonRequest 
        : IJSONWithHtmlResponse
    {
        public string player1 { get; set; }
        public string player2 { get; set; }

        public string GetResponse()
        {
            return this.player1 + " vs " + this.player2;
        }
    }        

    class WebService
    {
        static string baseUrl = "http://localhost:8081/dominion/";
        static string defaultPage = HtmlRenderer.GetEmbeddedContent("Dominulator.html");

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

                if (urlString + "/" == baseUrl)
                {
                    responseText = defaultPage;
                    response.ContentType = "text/html";
                }
                else if (urlString.StartsWith(baseUrl))
                {
                    var streamReader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding);
                    var jsonRequest = streamReader.ReadToEnd();

                    var js = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var data1 = Uri.UnescapeDataString(jsonRequest);

                    string requestedPage = urlString.Remove(0, baseUrl.Length);
                    object unserializedObject = null;

                    if (requestedPage == "StrategyComparisonRequest")                    
                    {
                        unserializedObject = js.Deserialize(data1, typeof(StrategyComparisonRequest));
                    }

                    if (unserializedObject is IJSONWithHtmlResponse)
                    {
                        responseText = ((IJSONWithHtmlResponse)unserializedObject).GetResponse();
                        //These headers to allow all browsers to get the response
                        response.Headers.Add("Access-Control-Allow-Credentials", "true");
                        response.Headers.Add("Access-Control-Allow-Origin", "*");
                        response.Headers.Add("Access-Control-Origin", "*");
                        response.ContentType = "text/html";
                        response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
                    }                    
                }

                if (responseText != null)
                {
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
