using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Unix;
using RetroBox.API.Data;
using RetroBox.Common.Data;
using RetroBox.Common.Xplat;

namespace RetroBox.Linux
{
    internal sealed class LinuxExecs : CommonExec
    {
        public override IEnumerable<FoundExe> FindExe(string folder)
        {
            const string emuName = "86Box";
            var files = Directory.GetFiles(folder, "*.AppImage", SearchOption.AllDirectories);
            foreach (var rawFile in files)
            {
                var file = rawFile;
                var unix = UnixFileSystemInfo.GetFileSystemEntry(file);
                if (!unix.FileAccessPermissions.HasFlag(FileAccessPermissions.UserExecute))
                    continue;
                var exeName = Path.GetFileNameWithoutExtension(file);
                var exeNamePt = exeName.Split('-');
                if (exeNamePt[0] != emuName)
                    continue;
                var exeBuild = exeNamePt.Last();
                var exeVer = Statics.FindByBuild(exeBuild);
                if (exeVer == null)
                    continue;
                var exeDir = Path.GetDirectoryName(file);
                if (exeDir == null)
                    continue;
                var linkFile = Path.Combine(exeDir, emuName);
                if (File.Exists(linkFile))
                    file = linkFile;
                yield return new FoundExe(file, exeVer, exeDir);
            }
        }
    }
}