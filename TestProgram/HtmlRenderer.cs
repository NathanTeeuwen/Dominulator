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
            this.BeginJavascriptTag();
            this.textWriter.WriteLine("google.load('visualization', '1', {packages:['corechart']});");
            this.EndTag();
            EndTag(); //head
            BeginTag("body");
        }

        public void InsertLineGraph(
            string title,
            string xAxisLabel,
            string seriesLabel,            
            int[] xAxis,
            float[] seriesData)
        {
            InsertLineGraph(title, xAxisLabel, new string[] { seriesLabel }, xAxis, new float[][] { seriesData });
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
            InsertLineGraph(title, xAxisLabel, new string[] { series1Label, series2Label }, xAxis, new float[][] { series1, series2 });
        }

        public void InsertLineGraph(
            string title,
            string xAxisLabel,
            string[] seriesLabels,            
            int[] xAxis,
            float[][] seriesData)
        {
            int multiplesOfFifteen = (xAxis.Length + 14)/15;

            int currentChartIndex = this.chartIndex++;
            this.textWriter.WriteLine("<div id='chart_div" + currentChartIndex + @"' style='width: 900px; height: 500px;'></div>");
            this.BeginJavascriptTag();            
            this.textWriter.WriteLine("google.setOnLoadCallback(drawChart);");
            this.textWriter.WriteLine("function drawChart() {");
            this.textWriter.Indent();
                this.textWriter.WriteLine("var data = google.visualization.arrayToDataTable([");
                this.textWriter.Indent();
                    this.textWriter.Write("['" + xAxisLabel + "'");
                    for (int seriesIndex = 0; seriesIndex < seriesLabels.Length; ++seriesIndex)
                    {
                        this.textWriter.Write(", '" + seriesLabels[seriesIndex] + "'");
                    }
                    this.textWriter.WriteLine("],");
                    for (int dataIndex = 0; dataIndex < xAxis.Length; ++dataIndex)
                    {
                        this.textWriter.Write("[" + xAxis[dataIndex]);
                        for (int seriesIndex = 0; seriesIndex < seriesData.Length; ++seriesIndex)
                        {
                            this.textWriter.Write(", " + seriesData[seriesIndex][dataIndex]);
                        }
                        this.textWriter.WriteLine("],");
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

        public void WriteLine()
        {
            LineBreak();
        }

        public void WriteLine(string text)
        {
            this.textWriter.Write(text);
            LineBreak();
        }

        public void WriteLine(string text, params object[] args)
        {
            this.textWriter.Write(text, args);
            this.LineBreak();
        }

        public void BeginTag(string tag)
        {
            WriteTag(tag);
            this.textWriter.Indent();
            this.openTags.Push(tag);
        }

        public void EndTag()
        {
            this.textWriter.Unindent();
            this.WriteEndTag(this.openTags.Pop());
        }

        public void LineBreak()
        {
            this.textWriter.WriteLine("<br>");
        }

        public void Header1(string text)
        {
            this.textWriter.WriteLine("<h1>{0}</h1>", text);
        }
    }
}
