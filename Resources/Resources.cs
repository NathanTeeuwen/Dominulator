using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resources
{
    public class ResourceLoader
    {
        Dictionary<string, string> mapCaseInsensitiveToCaseSensitive;
        System.Reflection.Assembly assembly;

        public ResourceLoader(System.Reflection.Assembly assembly)
        {
            this.mapCaseInsensitiveToCaseSensitive = GetEmbeddedResourceMapping(assembly);
            this.assembly = assembly;
        }

        public string GetEmbeddedContent(string defaultNamespace, string content)
        {
            string resourceName1 = content.Replace("/", ".");
            string resourceName2 = defaultNamespace + resourceName1;
            string resourceName3 = resourceName2.ToLower();
            string resourceName = mapCaseInsensitiveToCaseSensitive[resourceName3];
            
            string result;
            using (System.IO.Stream stream = this.assembly.GetManifestResourceStream(resourceName))
            using (var reader = new System.IO.StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }            

            return result;
        }

        public byte[] GetEmbeddedContentAsBinary(string defaultNamespace, string content)
        {
            string resourceName1 = content.Replace("/", ".");
            string resourceName2 = defaultNamespace + resourceName1;
            string resourceName3 = resourceName2.ToLower();
            string resourceName;
            if (!mapCaseInsensitiveToCaseSensitive.TryGetValue(resourceName3, out resourceName))
            {
                return null;
            }
            
            using (System.IO.Stream stream = this.assembly.GetManifestResourceStream(resourceName))
            using (var reader = new System.IO.BinaryReader(stream))
            {
                var result = new byte[stream.Length];
                reader.Read(result, 0, (int)stream.Length);
                return result;
            }            
        }

        private static Dictionary<string, string> GetEmbeddedResourceMapping(System.Reflection.Assembly assembly)
        {            
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
