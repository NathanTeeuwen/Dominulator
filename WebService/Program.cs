using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CardTypes = Dominion.CardTypes;
using Dominion;
using Dominion.Strategy;

namespace Program
{
    class Program
    {
        static void Main()
        {
            System.Console.WriteLine("Checking Card Images ...");
            CheckForAllCardImages();

            System.Console.WriteLine("Loading strategies ...");
            if (!Program.strategyLoader.Load())
                return;
            System.Console.WriteLine("Done");            
          
            System.Console.WriteLine("Running Web Service ...");
            new WebService.WebService().Run();
        }        

        private static void CheckForAllCardImages()
        {
            foreach (Card card in Cards.AllCardsList)
            {
                if (card == CardTypes.Prize.card || card == CardTypes.Ruins.card)
                    continue;
                if (resourceLoader.GetEmbeddedContentAsBinary("Webservice.Resources.cards.", card.ProgrammaticName + ".jpg") == null)
                {
                    System.Console.WriteLine("Warning, image missing for: {0}", card.name);
                };
            }
        }       

        static string GetOuputFilename(string filename)
        {
            return "..\\..\\Results\\" + filename;
        }

        internal static DynamicStrategyLoader strategyLoader = new DynamicStrategyLoader();

        static Resources.ResourceLoader resourceLoader = new Resources.ResourceLoader(System.Reflection.Assembly.GetExecutingAssembly());

        public static string GetEmbeddedContent(string content)
        {
            return resourceLoader.GetEmbeddedContent("Webservice.Resources.", content);
        }

        public static byte[] GetEmbeddedContentAsBinary(string content)
        {
            return resourceLoader.GetEmbeddedContentAsBinary("Webservice.Resources.", content);
        }
    }
}
