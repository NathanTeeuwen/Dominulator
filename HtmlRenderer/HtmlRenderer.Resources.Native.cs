using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IO = System.IO;
using Dominion;

namespace HtmlRenderer
{
    partial class HtmlRenderer
    {       
        static Resources.ResourceLoader resourceLoader = new Resources.ResourceLoader(System.Reflection.Assembly.GetExecutingAssembly());

        public static string GetEmbeddedContent(string content)
        {
            return resourceLoader.GetEmbeddedContent("HtmlRenderer.Resources.", content);
        }

        public static byte[] GetEmbeddedContentAsBinary(string content)
        {
            return resourceLoader.GetEmbeddedContentAsBinary("HtmlRenderer.Resources.", content);
        }
    }
}