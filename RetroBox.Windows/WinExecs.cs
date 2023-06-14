using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using RetroBox.API.Data;
using RetroBox.Common;
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
                yield return new FoundExe(file, exeVer);
            }
        }

        public override void FindSystemic(string home, out List<FoundExe> exe, out List<FoundRom> rom)
        {
            exe = new List<FoundExe>();
            rom = new List<FoundRom>();
            FindPrograms(exe, rom);
            FindPortableSys(exe, rom);
            FindPortableUser(exe, rom, home);
        }

        private void FindPrograms(ICollection<FoundExe> exe, ICollection<FoundRom> rom)
        {
            const string id = "Program";
            var dirs = new HashSet<string>
            {
                Env.Resolve("ProgramFiles") ?? string.Empty,
                Env.Resolve("ProgramFiles(x86)") ?? string.Empty
            };
            foreach (var dir in dirs)
                FindOneExe(exe, rom, dir, id);
        }

        private void FindPortableSys(ICollection<FoundExe> exe, ICollection<FoundRom> rom)
        {
            const string id = "Portable";
            var drive = Env.Resolve("HOMEDRIVE") ?? Env.Resolve("SystemDrive");
            var prefix = Path.Combine(drive!, "Portable");
            FindOneExe(exe, rom, prefix, id);
        }

        private void FindPortableUser(ICollection<FoundExe> exe, ICollection<FoundRom> rom, string home)
        {
            const string id = "PortableU";
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

        public ProcessStartInfo CreateStartArg(StartBoxArg a, string vmId, string hWnd)
        {
            var info = CreateStartArg(a);
            var args = info.ArgumentList;
            if (!string.IsNullOrWhiteSpace(vmId))
            {
                args.Add("--hwnd");
                args.Add($"{vmId},{hWnd}");
            }
            return info;
        }
    }
}