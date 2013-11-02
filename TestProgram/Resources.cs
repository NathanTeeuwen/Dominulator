using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Program
{
    static class Resources
    {
        static Dictionary<string, string> mapCaseInsensitiveToCaseSensitive = GetEmbeddedResourceMapping();

        public static string GetEmbeddedContent(string defaultNamespace, string content)
        {
            string resourceName1 = content.Replace("/", ".");
            string resourceName2 = defaultNamespace + resourceName1;
            string resourceName3 = resourceName2.ToLower();
            string resourceName = mapCaseInsensitiveToCaseSensitive[resourceName3];

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string result;
            using (System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new System.IO.StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        private static Dictionary<string, string> GetEmbeddedResourceMapping()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string[] resources = assembly.GetManifestResourceNames();

            Dictionary<string, string> resourceMap = new Dictionary<string, string>();

            foreach (string resource in resources)
            {
                resourceMap[resource.ToLower()] = resource;
            }

            return resourceMap;
        }
    }
}
