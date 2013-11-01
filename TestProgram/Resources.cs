using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    static class Resources
    {       
        public static string GetEmbeddedContent(string defaultNamespace, string content)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = defaultNamespace + content;

            string result;
            using (System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new System.IO.StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }
    }
}
