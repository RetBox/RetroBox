using System.Collections.Generic;
using System.Diagnostics;
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

        public abstract void FindSystemic(string home, out List<FoundExe> exe, out List<FoundRom> rom);

        protected ProcessStartInfo CreateStartArg(StartBoxArg a)
        {
            var info = new ProcessStartInfo(a.Exe!);
            if (a.Vars != null)
            {
                var env = info.EnvironmentVariables;
                foreach (var item in a.Vars)
                    env[item.Key] = item.Value;
            }
            var args = info.ArgumentList;
            if (!string.IsNullOrWhiteSpace(a.Config))
            {
                args.Add("--config");
                args.Add(a.Config);
            }
            if (a.FullScreen)
            {
                args.Add("--fullscreen");
            }
            if (!string.IsNullOrWhiteSpace(a.LogFile))
            {
                args.Add("--logfile");
                args.Add(a.LogFile);
            }
            if (!string.IsNullOrWhiteSpace(a.VmPath))
            {
                args.Add("--vmpath");
                args.Add(a.VmPath);
            }
            if (!string.IsNullOrWhiteSpace(a.RomPath))
            {
                args.Add("--rompath");
                args.Add(a.RomPath);
            }
            if (a.Settings)
            {
                args.Add("--settings");
            }
            if (!string.IsNullOrWhiteSpace(a.VmName))
            {
                args.Add("--vmname");
                args.Add(a.VmName);
            }
            return info;
        }
    }
}