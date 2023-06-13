using System.Collections.Generic;
using RetroBox.API.Data;

namespace RetroBox.Common.Data
{
    public static class Statics
    {
        private static readonly Dictionary<string, FileVerMeta> _versions;

        static Statics()
        {
            var jsonText = Resources.LoadText("86BoxVersions.json");
            _versions = Serials.ReadJson<Dictionary<string, FileVerMeta>>(jsonText)!;
        }

        public static FileVerMeta? FindByBuild(string build)
        {
            _versions.TryGetValue(build, out var meta);
            return meta;
        }

        public static FileVerMeta Copy(this IFileVerMeta meta, string? relId = null)
        {
            var res = new FileVerMeta
            {
                ReleaseId = relId ?? meta.ReleaseId,
                FileMajorPart = meta.FileMajorPart,
                FileMinorPart = meta.FileMinorPart,
                FileBuildPart = meta.FileBuildPart,
                FilePrivatePart = meta.FilePrivatePart,
                FileVersion = meta.FileVersion
            };
            return res;
        }
    }
}