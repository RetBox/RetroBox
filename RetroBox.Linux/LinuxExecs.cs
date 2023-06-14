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
            var files = Directory.GetFiles(folder, $"{emuName}*.AppImage", SearchOption.AllDirectories);
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
                yield return new FoundExe(file, exeVer);
            }
        }

        public override void FindSystemic(string home, out List<FoundExe> exe, out List<FoundRom> rom)
        {
            exe = new List<FoundExe>();
            rom = new List<FoundRom>();
            FindPackage(exe, rom);
            FindSysAdmin(exe, rom);
            FindFlatpakSys(exe, rom);
            FindFlatpakUser(exe, rom, home);
            FindPortable(exe, rom, home);
        }

        private void FindPortable(ICollection<FoundExe> exe, ICollection<FoundRom> rom, string home)
        {
            const string id = "Portable";
            var prefix = Path.Combine(home, "Portable");
            foreach (var item in FindExe(prefix))
            {
                exe.Add(item with { Version = item.Version.Copy(id) });

                var dir = Path.GetDirectoryName(item.File) ?? prefix;
                foreach (var rItem in FindRom(dir))
                    rom.Add(rItem with { ReleaseId = id });
            }
        }

        private void FindPackage(ICollection<FoundExe> exe, ICollection<FoundRom> rom)
        {
            const string relId = "System";
            const string prefix = "/usr";
            FindDistro(exe, rom, prefix, relId);
        }

        private void FindSysAdmin(ICollection<FoundExe> exe, ICollection<FoundRom> rom)
        {
            const string relId = "SystemL";
            const string prefix = "/usr/local";
            FindDistro(exe, rom, prefix, relId);
        }

        private void FindDistro(ICollection<FoundExe> exe, ICollection<FoundRom> rom, string prefix, string relId)
        {
            var tmp = Path.Combine(prefix, "bin/86Box");
            if (File.Exists(tmp))
                exe.Add(new FoundExe(tmp, new FileVerMeta { ReleaseId = relId }));
            tmp = Path.Combine(prefix, "share/86Box/roms");
            if (Directory.Exists(tmp))
                rom.Add(new FoundRom(relId, tmp));
        }

        private void FindFlatpakSys(ICollection<FoundExe> exe, ICollection<FoundRom> rom)
        {
            const string relId = "Flatpak";
            const string prefix = "/var/lib";
            FindFlatpak(exe, rom, prefix, relId);
        }

        private void FindFlatpakUser(ICollection<FoundExe> exe, ICollection<FoundRom> rom, string home)
        {
            const string relId = "FlatpakU";
            var prefix = Path.Combine(home, ".local/share");
            FindFlatpak(exe, rom, prefix, relId);
        }

        private void FindFlatpak(ICollection<FoundExe> exe, ICollection<FoundRom> rom, string prefix, string relId)
        {
            var tmp = Path.Combine(prefix, "flatpak/exports/bin/net._86box._86Box");
            if (File.Exists(tmp))
                exe.Add(new FoundExe(tmp, new FileVerMeta { ReleaseId = relId }));
            tmp = Path.Combine(prefix, "flatpak/runtime/net._86box._86Box.ROMs/x86_64/stable/active/files");
            if (Directory.Exists(tmp))
                rom.Add(new FoundRom(relId, tmp));
        }
    }
}