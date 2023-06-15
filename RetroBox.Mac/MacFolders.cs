using RetroBox.API.Xplat;
using RetroBox.Common;
using RetroBox.Common.Xplat;

namespace RetroBox.Mac
{
    internal sealed class MacFolders : CommonFolder
    {
        public override string GetDefaultConfigPath()
        {
            const string subPath = $"Library/Application Support/{nameof(RetroBox)}";
            var path = Env.Resolve("HOME", subPath);
            return path!;
        }

        public override string GetDefaultTempPath()
        {
            var path = Env.Resolve("TMPDIR");
            return path!;
        }

        public override string GetDefaultHomePath()
        {
            var path = Env.Resolve("HOME");
            return path!;
        }
    }
}