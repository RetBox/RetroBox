using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using RetroBox.API.Data;
using RetroBox.Common.Data;
using RetroBox.Common.Xplat;

namespace RetroBox.Windows
{
    internal sealed class WinExecs : CommonExec
    {
        public override IEnumerable<FoundExe> FindExe(string folder)
        {
            const string emuName = "86Box";
            var files = Directory.GetFiles(folder, $"{emuName}*.exe", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var fileVer = FileVersionInfo.GetVersionInfo(file);
                var exeBuild = $"b{fileVer.FilePrivatePart}";
                var exeVer = Statics.FindByBuild(exeBuild);
                if (exeVer == null)
                    continue;
                var exeDir = Path.GetDirectoryName(file);
                if (exeDir == null)
                    continue;
                yield return new FoundExe(file, exeVer, exeDir);
            }
        }
    }
}