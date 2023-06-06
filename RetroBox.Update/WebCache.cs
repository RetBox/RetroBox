using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace RetroBox.Update
{
    internal static class WebCache
    {
        private static readonly HttpClient Client = new();
        private static readonly string Root;

        static WebCache()
        {
            Root = Directory.CreateDirectory(nameof(WebCache)).FullName;
        }

        public static async Task<Stream> GetStreamAsync(string url)
        {
            var cacheFile = url.Split(":", 2)[1].TrimStart('/')
                .Replace(".", "-").Replace("/", "_");
            cacheFile = Path.Combine(Root, cacheFile);

            if (File.Exists(cacheFile) &&
                DateTime.UtcNow - File.GetLastWriteTimeUtc(cacheFile) <= TimeSpan.FromHours(4))
            {
                return File.OpenRead(cacheFile);
            }

            var bytes = await Client.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(cacheFile, bytes);
            return new MemoryStream(bytes);
        }
    }
}