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
        public static string GetEmbeddedContent(string content)
        {
            var task = GetEmbeddedContentAsync(content);            
            task.Wait();
            return task.Result;
        }

        public static async Task<string> GetEmbeddedContentAsync(string content)
        {
            string fullPath = "HtmlRenderer.Portable\\Resources\\" + content;
            Windows.Storage.StorageFile manifestFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(fullPath);            
            string result = await Windows.Storage.FileIO.ReadTextAsync(manifestFile);
            return result;
        }        
    }
}