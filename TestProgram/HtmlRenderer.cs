using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IO = System.IO;
using Dominion;

namespace Program
{
    class HtmlRenderer
    {
        private int chartIndex = 0;
        private readonly IndentedTextWriter textWriter;
        private readonly Stack<string> openTags;

        public HtmlRenderer(IndentedTextWriter textWriter)
        {
            this.textWriter = textWriter;
            this.openTags = new Stack<string>();
        }

        public void Begin()
        {
            BeginTag("html");
            BeginTag("head");
            this.textWriter.WriteLine(@"<script type='text/javascript' src='https://www.google.com/jsapi'></script>");
            EndTag(); //head
            BeginTag("body");
        }

        public void InsertLineGraph(
            string title,
            string xAxisLabel,
            string series1Label,
            string series2Label,
            int[] xAxis,
            float[] series1,
            float[] series2)
        {
            int multiplesOfFifteen = (xAxis.Length + 14)/15;

            int currentChartIndex = this.chartIndex++;
            this.textWriter.WriteLine("<div id='chart_div" + currentChartIndex + @"' style='width: 900px; height: 500px;'></div>");
            this.BeginJavascriptTag();
            this.textWriter.WriteLine("google.load('visualization', '1', {packages:['corechart']});");
            this.textWriter.WriteLine("google.setOnLoadCallback(drawChart);");
            this.textWriter.WriteLine("function drawChart() {");
            this.textWriter.Indent();
                this.textWriter.WriteLine("var data = google.visualization.arrayToDataTable([");
                this.textWriter.Indent();
                    this.textWriter.WriteLine("['" + xAxisLabel + "', '" + series1Label + "', '" + series2Label + "'],");
                    for (int seriesIndex = 0; seriesIndex < xAxis.Length; ++seriesIndex)
                    {
                        this.textWriter.WriteLine("[" + xAxis[seriesIndex] + ", " + series1[seriesIndex] + ", " + series2[seriesIndex] + "],");
                    }
                this.textWriter.Unindent();
                this.textWriter.WriteLine("]);");                                            
                this.textWriter.WriteLine();                
                this.textWriter.WriteLine("var options = {");              
                this.textWriter.Indent();
                    this.textWriter.WriteLine("title: '" + title +"',");
                    this.textWriter.WriteLine("hAxis: {");
                    this.textWriter.Indent();
                        this.textWriter.WriteLine("gridlines: {");
                        this.textWriter.Indent();
                            this.textWriter.WriteLine("count: " + xAxis.Length/multiplesOfFifteen); 
                        this.textWriter.Unindent();
                        this.textWriter.WriteLine("}");
                    this.textWriter.Unindent();
                    this.textWriter.WriteLine("}");
                this.textWriter.Unindent();
                this.textWriter.WriteLine("};");                
                this.textWriter.WriteLine();
                this.textWriter.WriteLine("var chart = new google.visualization.LineChart(document.getElementById('chart_div" + currentChartIndex + "'));");
                this.textWriter.WriteLine("chart.draw(data, options);");
            this.textWriter.Unindent();
            this.textWriter.WriteLine("}");
            this.EndTag();            
        }

        public void End()
        {
            EndTag(); //body
            EndTag(); //html
        }

        private void WriteTag(string tag)
        {
            this.textWriter.WriteLine("<" + tag + ">");   
        }

        private void WriteEndTag(string tag)
        {
            this.textWriter.WriteLine("</" + tag + ">");
        }

        private void BeginJavascriptTag()
        {
            this.textWriter.WriteLine("<script type='text/javascript'>");
            this.textWriter.Indent();
            this.openTags.Push("script");
        }

        private void BeginTag(string tag)
        {
            WriteTag(tag);
            this.textWriter.Indent();
            this.openTags.Push(tag);
        }

        private void EndTag()
        {
            this.textWriter.Unindent();
            this.WriteEndTag(this.openTags.Pop());
        }
    }
}
