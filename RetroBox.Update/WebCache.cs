using System;
using System.IO;
using System.Threading.Tasks;

namespace RetroBox.Update
{
    public sealed class WebCache
    {
        private readonly string _root;

        public WebCache(string root)
        {
            var dir = Path.Combine(root, "web");
            _root = Directory.CreateDirectory(dir).FullName;
        }

        public async Task<Stream> GetStreamAsync(string url)
        {
            var cacheFile = url.Split(":", 2)[1].TrimStart('/')
                .Replace(".", "-").Replace("/", "_");
            cacheFile = Path.Combine(_root, cacheFile);

            if (File.Exists(cacheFile) &&
                DateTime.UtcNow - File.GetLastWriteTimeUtc(cacheFile) <= TimeSpan.FromHours(4))
            {
                return File.OpenRead(cacheFile);
            }

            var client = HttpCentral.GetClient(null);
            var bytes = await client.GetByteArrayAsync(url);
            await File.WriteAllBytesAsync(cacheFile, bytes);
            return new MemoryStream(bytes);
        }
    }
}