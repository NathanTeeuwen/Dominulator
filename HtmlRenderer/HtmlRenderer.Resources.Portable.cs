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
//        static Resources.ResourceLoader resourceLoader = new Resources.ResourceLoader(System.Reflection.Assembly.GetExecutingAssembly());

        public static string GetEmbeddedContent(string content)
        {
            var task = GetEmbeddedContentAsync(content);
            task.RunSynchronously();
            return task.Result;
        }

        public static byte[] GetEmbeddedContentAsBinary(string content)
        {
            var task = GetEmbeddedContentAsBinaryAsync(content);
            task.RunSynchronously();
            return task.Result;
        }

        public static async Task<string> GetEmbeddedContentAsync(string content)
        {
            Windows.Storage.StorageFile manifestFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(content);            
            string result = await Windows.Storage.FileIO.ReadTextAsync(manifestFile);
            return result;
        }

        public static async Task<byte[]> GetEmbeddedContentAsBinaryAsync(string content)
        {
            Windows.Storage.StorageFile manifestFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(content);
            Windows.Storage.Streams.IBuffer buffer = await Windows.Storage.FileIO.ReadBufferAsync(manifestFile);
            byte[] rawBytes = new byte[buffer.Length];
            using (var reader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
            {
                reader.ReadBytes(rawBytes);
            }

            return rawBytes;
        }
    }
}