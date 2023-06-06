using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using RetroBox.API.Update;

namespace RetroBox.Update
{
    public static class RuntimeFilter
    {
        public static List<Release> FindMatch(this IEnumerable<Release> releases, bool ignoreSrc = true)
        {
            var osToSearch = GetCurrentOsName();
            var archToSearch = GetCurrentOsArch();

            var res = new List<Release>();
            foreach (var release in releases)
            {
                var filtered = release.Artifacts
                    .Where(a => a.Arch.Contains(archToSearch) && a.OS == osToSearch)
                    .ToList();
                if (filtered.Count == 1)
                {
                    res.Add(release with { Artifacts = filtered });
                    continue;
                }
                if (filtered.Count >= 2)
                {
                    res.Add(release with { Artifacts = filtered.Take(1).ToList() });
                    continue;
                }
                if (ignoreSrc)
                    continue;
                var srcZip = release.Artifacts
                    .FirstOrDefault(a => a.Name.StartsWith("Source ") && a.Type == FileType.Zip);
                if (srcZip != null)
                {
                    res.Add(release with { Artifacts = new List<Artifact> { srcZip } });
                }
            }

            return res;
        }

        private static OsArch GetCurrentOsArch()
        {
            OsArch res = default;
            var arch = RuntimeInformation.OSArchitecture;
            switch (arch)
            {
                case Architecture.X86:
                    res = OsArch.x86;
                    break;
                case Architecture.X64:
                    res = OsArch.x86_64;
                    break;
                case Architecture.Arm:
                    res = OsArch.arm32;
                    break;
                case Architecture.Arm64:
                    res = OsArch.arm64;
                    break;
            }
            return res;
        }

        private static OsName GetCurrentOsName()
        {
            OsName res = default;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                res = OsName.Windows;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                res = OsName.MacOS;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                res = OsName.Linux;
            return res;
        }
    }
}