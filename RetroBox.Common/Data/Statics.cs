using System.Collections.Generic;

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
    }
}