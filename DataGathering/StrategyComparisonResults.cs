using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominion;
using Dominion.Strategy;

namespace Dominion.Data
{    
    public class StrategyComparisonResults
    {
        public readonly StrategyComparison comparison;

        public readonly HistogramData pointSpreadHistogramData;
        public readonly HistogramData gameEndOnTurnHistogramData;
        public readonly StatsPerTurnGameLog statGatherer;
        public readonly int[] winnerCount;
        public int maxTurnNumber;                
        public int tieCount;

        public StrategyComparisonResults(StrategyComparison comparison, bool gatherStats)
        {
            this.comparison = comparison;
            this.statGatherer = gatherStats ? new StatsPerTurnGameLog(2, comparison.gameConfig.cardGameSubset) : null;
            this.winnerCount = new int[2];
            this.tieCount = 0;
            this.maxTurnNumber = -1;
            this.pointSpreadHistogramData = new HistogramData();
            this.gameEndOnTurnHistogramData = new HistogramData();
        }

        public void WriteCompactScore(System.IO.TextWriter textWriter)
        {
            textWriter.WriteLine("{0}, {1}, {2}", PlayerWinPercent(0), PlayerWinPercent(1), TiePercent);
        }

        public void WriteVerboseScore(System.IO.TextWriter textWriter)
        {            
            for (int index = 0; index < winnerCount.Length; ++index)
            {
                textWriter.WriteLine("{1}% win for {0}", comparison.playerActions[index].name, PlayerWinPercent(index));
            }
            if (tieCount > 0)
            {
                textWriter.WriteLine("{0}% there is a tie.", TiePercent);
            }
            textWriter.WriteLine();            
        }

        public double TiePercent
        {
            get
            {
                return tieCount / (double)comparison.numberOfGames * 100;
            }
        }

        public double PlayerWinPercent(int player)
        {        
            return this.winnerCount[player] / (double)comparison.numberOfGames * 100;
        }

        public double WinDifference
        {
            get
            {
                double diff = PlayerWinPercent(0) - PlayerWinPercent(1);
                return diff;
            }
        }

        public void ShowDistribution(System.IO.TextWriter textWriter)
        {
            textWriter.WriteLine("");
            textWriter.WriteLine("Player 1 Score Delta distribution");
            textWriter.WriteLine("=================================");
            this.pointSpreadHistogramData.WriteBuckets(textWriter);
        }
    }

}
