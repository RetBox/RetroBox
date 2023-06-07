using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ZipFile = ICSharpCode.SharpZipLib.Zip.ZipFile;

namespace RetroBox.Update
{
    public static class ZipLoader
    {
        public static async Task<string> UnCompress(string inputFile, string outPath,
            ProgressHandler receive, CancellationToken token)
        {
            if (Directory.Exists(outPath))
                return outPath;
            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outPath);

            using var zipFile = new ZipFile(inputFile);

            var holder = new ZipHolder
            {
                UncompressedSize = zipFile.OfType<ZipEntry>().Sum(e => e.Size),
                ProcessedBytes = 0L
            };

            const int bufferSize = 4096;
            var buffer = new byte[bufferSize];

            foreach (ZipEntry entry in zipFile)
            {
                token.ThrowIfCancellationRequested();

                var rawName = entry.Name;
                var name = rawName;
                var tmp = SystemFix.Correct(rawName).Split(SystemFix.Slash, 2);
                if (tmp.Length == 2)
                    name = tmp.Last();

                if (entry.IsDirectory)
                {
                    var dir = Path.Combine(outPath, name);
                    Directory.CreateDirectory(dir);
                    continue;
                }
                if (!entry.IsFile)
                    continue;

                await using var stream = zipFile.GetInputStream(entry);
                var outName = Path.Combine(outPath, name);
                await using var fileOut = File.Create(outName);

                var interval = TimeSpan.FromSeconds(1);
                StreamUtils.Copy(stream, fileOut, buffer, receive, interval, holder, rawName);
            }
            return outPath;
        }
    }
}