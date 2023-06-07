using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace RetroBox.Update
{
    public static class WebLoader
    {
        public static async Task Download(string? reqUri, string filePath,
            WebReceiveHandler receive, CancellationToken token)
        {
            if (File.Exists(filePath))
                return;
            var dirPath = Path.GetDirectoryName(filePath)!;
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            var client = HttpCentral.GetClient(receive);
            var stream = await client.GetStreamAsync(reqUri, token);
            await using var file = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await stream.CopyToAsync(file, token);
        }
    }
}