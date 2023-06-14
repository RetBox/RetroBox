using System;
using RetroBox.API.Data;
using System.Collections.Generic;
using System.IO;
using RetroBox.Common.Xplat;
using System.Linq;
using System.Text;
using System.Threading;
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

        public override void FindSystemic(string home, out List<FoundExe> exe, out List<FoundRom> rom,
            CancellationToken token)
        {
            exe = new List<FoundExe>();
            rom = new List<FoundRom>();
            token.ThrowIfCancellationRequested();
            FindHomebrew(exe, rom, home);
            token.ThrowIfCancellationRequested();
            FindAppsUser(exe, rom, home);
            token.ThrowIfCancellationRequested();
            FindPortable(exe, rom, home);
        }

        private void FindHomebrew(ICollection<FoundExe> exe, ICollection<FoundRom> rom, string home)
        {
            const string id = "App";
            var prefix = "/Applications";
            FindOneExe(exe, rom, prefix, id);

            var dir = Path.Combine(home, "Library","Application Support","net.86box.86Box","roms");
            foreach (var rItem in FindRom(dir))
                rom.Add(rItem with { ReleaseId = id });
        }

        private void FindAppsUser(ICollection<FoundExe> exe, ICollection<FoundRom> rom, string home)
        {
            const string id = "AppU";
            var prefix = Path.Combine(home, "Applications");
            FindOneExe(exe, rom, prefix, id);
        }

        private void FindPortable(ICollection<FoundExe> exe, ICollection<FoundRom> rom, string home)
        {
            const string id = "Portable";
            var prefix = Path.Combine(home, "Portable");
            FindOneExe(exe, rom, prefix, id);
        }

        private void FindOneExe(ICollection<FoundExe> exe, ICollection<FoundRom> rom, string prefix, string id)
        {
            foreach (var item in FindExe(prefix))
            {
                exe.Add(item with { Version = item.Version.Copy(id) });

                var dir = Path.GetDirectoryName(item.File) ?? prefix;
                foreach (var rItem in FindRom(dir))
                    rom.Add(rItem with { ReleaseId = id });
            }
        }
    }
}