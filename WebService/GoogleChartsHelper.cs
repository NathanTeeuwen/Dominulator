using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Dominion;
using Dominion.Strategy;
using Dominion.Data;

namespace Program
{
    static class GoogleChartsHelper
    {
        // all of the objects are in fact JSON objects.

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
            options.Add("title", title);
            options.Add("hAxis", GetHAxisOptions(xAxis));

            var result = new Dictionary<string, object>();
            result.Add("data", data);
            result.Add("options", options);
            result.Add("type", "line");

            return result;
        }

        public static object GetHAxisOptions(int[] xAxis)
        {
            int multiplesOfFifteen = (xAxis.Length + 14) / 15;

            var gridLines = new Dictionary<string, object>();
            gridLines.Add("count", xAxis.Length / multiplesOfFifteen);

            var hAxis = new Dictionary<string, object>();
            hAxis.Add("gridlines", gridLines);

            return hAxis;
        }
    }
}