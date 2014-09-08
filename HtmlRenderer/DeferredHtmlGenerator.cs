using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominion.Data;

namespace HtmlRenderer
{
    public class DeferredHtmlGenerator
        : IDisposable
    {
        int outstandingTasks = 0;

        public void AddResults(StrategyComparisonResults results, Func<string, string> GetOutputFilename)
        {
            System.Threading.Interlocked.Increment(ref outstandingTasks);
            // write out HTML report summary
            var thread = new System.Threading.Thread(delegate()
            {
                var generator = new HtmlReportGenerator(results);

                generator.CreateHtmlReport(GetOutputFilename(results.comparison.playerActions[0].PlayerName + " VS " + results.comparison.playerActions[1].PlayerName + ".html"));
                System.Threading.Interlocked.Decrement(ref outstandingTasks);
            });
            thread.Start();
        }

        public void Dispose()
        {
            WaitForAllBackgroundTasks();
        }

        private void WaitForAllBackgroundTasks()
        {
            while (this.outstandingTasks > 0)
            {
                System.Threading.Thread.Sleep(1);
            }
        }
    }
}
