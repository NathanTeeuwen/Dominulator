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
        static void Main(string[] args)
        {
            FindOptimalPlayForEachCardWithBigMoney();
        }

        static void FindOptimalPlayForEachCardWithBigMoney()
        {
            PlayerAction bigMoneyPlayer = Strategies.BigMoney.Player();
            using (var htmlRender = new HtmlRenderer.DeferredHtmlGenerator())
            using (var testoutput = new TestOutput())
            {
                foreach (Card card in Dominion.Strategy.MissingDefaults.FullyImplementedKingdomCards())
                {
                    var playerAction = StrategyOptimizer.FindBestBigMoneyWithCardVsStrategy(bigMoneyPlayer, card, logProgress:false);
                    var results = Dominion.Data.StrategyComparison.Compare(playerAction, bigMoneyPlayer, shouldParalell: true);
                    htmlRender.AddResults(results, TestOutput.GetOutputFilename);
                    testoutput.ComparePlayers(playerAction, bigMoneyPlayer, createHtmlReport:false);
                }
            }
        }      
    }
}
