using System;
using RetroBox.API.Data;
using System.Collections.Generic;
using System.IO;
using RetroBox.Common.Xplat;
using System.Linq;
using System.Text;
using RetroBox.Common.Data;

namespace RetroBox.Mac
{
    internal sealed class MacExecs : CommonExec
    {
        public override IEnumerable<FoundExe> FindExe(string folder)
        {
            const string emuName = "86Box";
            var dirs = Directory.GetDirectories(folder, $"{emuName}*.app", SearchOption.AllDirectories);
            foreach (var dir in dirs)
            {
                var contents = Path.Combine(dir, "Contents");
                var macos = Path.Combine(contents, "MacOS");
                var file = Directory.GetFiles(macos).First();
                var info = Path.Combine(contents, "Info.plist");
                var infoLines = File.ReadAllLines(info, Encoding.UTF8);
                var infoLine = infoLines.SkipWhile(i => !i.Contains("CFBundleVersion")).Skip(1);
                var infoTxt = infoLine.First().Split('>', '<');
                var verTxt = infoTxt.Skip(2).First();
                var verObj = Version.Parse(verTxt);
                var exeBuild = $"b{verObj.Revision}";
                var exeVer = Statics.FindByBuild(exeBuild);
                if (exeVer == null)
                    continue;
                var exeDir = Path.GetDirectoryName(dir);
                if (exeDir == null)
                    continue;
                var linkFile = Path.Combine(exeDir, emuName);
                if (File.Exists(linkFile))
                    file = linkFile;
                yield return new FoundExe(file, exeVer);
            }
        }
    }
}