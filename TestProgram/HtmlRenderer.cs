using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IO = System.IO;
using Dominion;

namespace Program
{
    delegate void HtmlContentInserter();

    class HtmlRenderer
    {
        private int divIndex = 0;
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
            this.UsingGoogleChartsAPI();
            this.UsingJQuery();
            this.UsingJQueryUI();
            EndTag(); //head
            BeginTag("body");
        }

        public void InsertExpander(
            string title,
            HtmlContentInserter content,
            bool collapseByDefault = true)
        {
            int currentChartIndex = this.divIndex++;
            string divId = "accordion_div" + currentChartIndex;

            this.textWriter.WriteLine("<div id='" + divId + "'>");
            this.textWriter.Indent();
            this.Header3(title);
            this.BeginTag("div");
            this.textWriter.Indent();
            content();
            this.textWriter.Unindent();
            this.EndTag();
            this.textWriter.Unindent();
            this.textWriter.WriteLine("</div>");
            this.BeginJavascriptTag();
            this.textWriter.WriteLine("$( '#" + divId + "' ).accordion({ ");
            this.textWriter.Indent();
            this.textWriter.WriteLine("collapsible: true,");
            this.textWriter.WriteLine("heightStyle: 'content',");
            this.textWriter.Unindent();
            this.textWriter.WriteLine("});");

            // collapse after initialziation so the graphs draw correctly
            if (collapseByDefault)
            {
                this.textWriter.WriteLine("$(  '#" + divId + "').accordion( 'option', 'active', false);");
            }
            
            this.EndTag();
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

            int currentChartIndex = this.divIndex++;
            string divId = "chart_div" + currentChartIndex;

            this.textWriter.WriteLine("<div id='" + divId + @"' style='width: 900px; height: 500px;'></div>");
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
                this.textWriter.WriteLine("var chart = new google.visualization.LineChart(document.getElementById('" + divId + "'));");
                this.textWriter.WriteLine("chart.draw(data, options);");
            this.textWriter.Unindent();
            this.textWriter.WriteLine("}");
            this.EndTag();            
        }

        public void InsertPieChart(
           string title,
           string labelTitle,           
           string dataTitle,
           string[] labels,
           float[] data)
        {
            int currentChartIndex = this.divIndex++;
            string divId = "chart_div" + currentChartIndex;

            this.textWriter.WriteLine("<div id='" + divId + @"' style='width: 900px; height: 500px;'></div>");
            this.BeginJavascriptTag();
            this.textWriter.WriteLine("google.setOnLoadCallback(drawChart);");
            this.textWriter.WriteLine("function drawChart() {");
            this.textWriter.Indent();
            this.textWriter.WriteLine("var data = google.visualization.arrayToDataTable([");
            this.textWriter.Indent();
            this.textWriter.Write("['" + labelTitle + "', '" + dataTitle + "'],");            
            for (int dataIndex = 0; dataIndex < labels.Length; ++dataIndex)
            {
                this.textWriter.Write("['" + labels[dataIndex] + "', " + data[dataIndex] + "],");                
            }
            this.textWriter.Unindent();
            this.textWriter.WriteLine("]);");
            this.textWriter.WriteLine();
            this.textWriter.WriteLine("var options = {");
            this.textWriter.Indent();
                this.textWriter.WriteLine("title: '" + title + "',");
                //this.textWriter.WriteLine("is3D: true");            
            this.textWriter.Unindent();
            this.textWriter.WriteLine("};");
            this.textWriter.WriteLine();
            this.textWriter.WriteLine("var chart = new google.visualization.PieChart(document.getElementById('" + divId + "'));");
            this.textWriter.WriteLine("chart.draw(data, options);");
            this.textWriter.Unindent();
            this.textWriter.WriteLine("}");
            this.EndTag();
        }

        public void InsertColumnChart(
           string title,
           string xAxisLabel,
           string[] seriesLabels,
           string[] xAxis,
           float[][] seriesData)
        {
            int multiplesOfFifteen = (xAxis.Length + 14) / 15;

            int currentChartIndex = this.divIndex++;
            string divId = "chart_div" + currentChartIndex;

            this.textWriter.WriteLine("<div id='" + divId + @"' style='width: 900px; height: 500px;'></div>");
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
                this.textWriter.Write("['" + xAxis[dataIndex] + "'");
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
                this.textWriter.WriteLine("title: '" + title + "',");
                this.textWriter.WriteLine("hAxis: {");
                this.textWriter.Indent();
                    this.textWriter.WriteLine("title: '" + xAxisLabel + "'");
                    this.textWriter.Unindent();
                this.textWriter.WriteLine("}");
            this.textWriter.Unindent();
            this.textWriter.WriteLine("};");
            this.textWriter.WriteLine();
            this.textWriter.WriteLine("var chart = new google.visualization.ColumnChart(document.getElementById('" + divId + "'));");
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

        // HTML writing helpers

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

        private void UsingGoogleChartsAPI()
        {
            this.BeginJavascriptTag();
            this.textWriter.WriteLine("google.load('visualization', '1', {packages:['corechart']});");
            this.EndTag();
        }

        private void UsingJQuery()
        {                        
            this.textWriter.WriteLine(@"<script src='http://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js'></script>");            
        }

        private void UsingJQueryUI()
        {                        
            this.textWriter.WriteLine(@"<script src='http://ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/jquery-ui.min.js'></script>");            
            this.textWriter.WriteLine(@"<link href='http://ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/themes/cupertino/jquery-ui.css' type='text/css' rel='Stylesheet' />");   
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

        public void Header3(string text)
        {
            this.textWriter.WriteLine("<h3>{0}</h3>", text);
        }
    }
}
