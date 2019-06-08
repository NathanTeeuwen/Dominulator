using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominion
{
    class ResultSetRanker
    {
        List<Result> resultList = new List<Result>();        

        public ResultSetRanker()
        {            
        }

        public void AddResult(string playerName1, string playerName2, double percentDiff)
        {
            this.resultList.Add(new Result(playerName1, playerName2, percentDiff));
        }

        class Result
        {
            public string playerName1;
            public string playerName2;
            public double percentDiff;

            public Result(string playerName1, string playerName2, double percentDiff)
            {
                this.playerName1 = playerName1;
                this.playerName2 = playerName2;
                this.percentDiff = percentDiff;
            }
        }

        public void WriteRanking()
        {
            if (this.resultList.Count == 0)
                return;

            int maxLength = Math.Max(this.resultList.Select(r => r.playerName1.Length).Max(), this.resultList.Select(r => r.playerName2.Length).Max());

            bool haveTransititionedBetweenNegAndPos = true;
            foreach (var result in resultList.OrderBy(r => r.percentDiff))
            {
                if (result.percentDiff > 0 && haveTransititionedBetweenNegAndPos)
                {
                    haveTransititionedBetweenNegAndPos = false;
                    System.Console.WriteLine("=====>");
                }
                if (result.percentDiff > 0)
                    System.Console.WriteLine("{0} beats {1} {2:F1}% more often", FormatName(result.playerName1, maxLength), FormatName(result.playerName2, maxLength), result.percentDiff);
                else if (result.percentDiff < 0)
                    System.Console.WriteLine("{0} beats {1} {2:F1}% more often", FormatName(result.playerName2, maxLength), FormatName(result.playerName1, maxLength), -result.percentDiff);
            } 
        }

        private static string FormatName(string name, int maxLength)
        {
            const int capLength = 50;
            var result = name.PadRight(maxLength);
            if (result.Length > capLength)
                result = result.Substring(0, capLength);
            return result;
        }
    }
}
