using System.Collections.Generic;
using System.IO;
using System.Linq;
using RetroBox.API.Data;
using RetroBox.API.Xplat;

namespace RetroBox.Common.Xplat
{
    public abstract class CommonExec : IPlatExec
    {
        public abstract IEnumerable<FoundExe> FindExe(string folder);

        public IEnumerable<FoundRom> FindRom(string folder)
        {
            var files = Directory.GetFiles(folder, "LICENSE", SearchOption.AllDirectories);
            foreach (var rawFile in files)
            {
                var exeDir = Path.GetDirectoryName(rawFile);
                if (exeDir == null)
                    continue;
                var exePts = exeDir.Split(Path.DirectorySeparatorChar).Reverse();
                var exeVer = exePts.SkipWhile(e => e == "e").FirstOrDefault();
                if (exeVer == null)
                    continue;
                yield return new FoundRom(exeVer, exeDir);
            }
        }
    }
}