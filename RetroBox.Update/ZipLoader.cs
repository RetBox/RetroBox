using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Mono.Unix;
using ZipFile = ICSharpCode.SharpZipLib.Zip.ZipFile;

namespace RetroBox.Update
{
    public static class ZipLoader
    {
        private const string AppImgExt = ".AppImage";

        public static async Task<string> UnCompress(string inputFile, string outPath,
            ProgressHandler receive, CancellationToken token)
        {
            if (Directory.Exists(outPath))
                return outPath;
            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outPath);

            if (inputFile.EndsWith(AppImgExt))
            {
                outPath = await LinkAppImage(inputFile, outPath, receive);
                return outPath;
            }

            using var zipFile = new ZipFile(inputFile);
            var (allSize, skip) = Prepare(zipFile);

            var holder = new ZipHolder
            {
                UncompressedSize = allSize,
                ProcessedBytes = 0L
            };

            const int bufferSize = 4096;
            var buffer = new byte[bufferSize];

            foreach (ZipEntry entry in zipFile)
            {
                token.ThrowIfCancellationRequested();

                var rawName = entry.Name;
                var tmp = rawName.Substring(Math.Min(rawName.Length, skip.Length));
                tmp = SystemFix.Correct(tmp);
                tmp = tmp.TrimStart(SystemFix.Slash);
                var name = tmp;

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

                if (entry is { CompressionMethod: CompressionMethod.Stored, Size: <= 32 } && !outName.EndsWith(".txt"))
                {
                    var linkBuff = new byte[entry.Size];
                    StreamUtils.ReadFully(stream, linkBuff);
                    var linkTgt = Encoding.UTF8.GetString(linkBuff).Trim();
                    if (!File.Exists(outName))
                        File.CreateSymbolicLink(outName, linkTgt);
                    continue;
                }

                await using var fileOut = File.Create(outName);
                var interval = TimeSpan.FromSeconds(1);
                StreamUtils.Copy(stream, fileOut, buffer, receive, interval, holder, rawName);

                if (outName.Contains(Path.Combine("Contents", "MacOS")))
                    FixMacPackage(outName, outPath);
            }
            return outPath;
        }

        private static void FixMacPackage(string name, string outPath)
        {
            var exeInfo = UnixFileSystemInfo.GetFileSystemEntry(name);
            exeInfo.FileAccessPermissions |= FileAccessPermissions.UserExecute;

            var exeName = Path.GetFileNameWithoutExtension(name);
            var lnkName = Path.Combine(outPath, exeName);
            if (!File.Exists(lnkName))
                File.CreateSymbolicLink(lnkName, exeInfo.FullName);
        }

        private static (long, string) Prepare(ZipFile zipFile)
        {
            var tmp = new Dictionary<int, ISet<string>>();
            var sep = '/';
            var size = 0L;
            foreach (var entry in zipFile.Cast<ZipEntry>())
            {
                size += entry.Size;
                var parts = entry.Name.Split(sep);
                for (var i = 0; i < parts.Length; i++)
                {
                    if (!tmp.TryGetValue(i, out var list))
                        tmp[i] = list = new HashSet<string>();
                    var part = parts[i];
                    if (string.IsNullOrWhiteSpace(part))
                        continue;
                    list.Add(part);
                }
            }
            var skip = tmp.TakeWhile(t => t.Value.Count <= 1 &&
                    !t.Value.First().EndsWith(".app"))
                .Select(t => t.Value.FirstOrDefault());
            var pre = string.Join(sep, skip);
            return (size, pre);
        }

        private static async Task<string> LinkAppImage(string inputFile, string outPath, ProgressHandler receive)
        {
            const int bufferSize = 4096;
            var buffer = new byte[bufferSize];

            var rawName = Path.GetFileName(inputFile);
            var info = new FileInfo(inputFile);
            var holder = new ZipHolder { UncompressedSize = info.Length, ProcessedBytes = 0L };
            var outName = Path.Combine(outPath, rawName);

            await using var fileIn = File.OpenRead(inputFile);
            await using var fileOut = File.Create(outName);
            var interval = TimeSpan.FromSeconds(1);
            StreamUtils.Copy(fileIn, fileOut, buffer, receive, interval, holder, rawName);

            var exeInfo = UnixFileSystemInfo.GetFileSystemEntry(outName);
            exeInfo.FileAccessPermissions |= FileAccessPermissions.UserExecute;

            var linkName = rawName.Split('-', 2).First();
            var outLink = Path.Combine(outPath, linkName);
            if (!File.Exists(outLink))
                File.CreateSymbolicLink(outLink, outName);

            return outPath;
        }
    }
}