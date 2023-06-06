using RetroBox.API;
using RetroBox.Common;

namespace RetroBox.Mac
{
    public sealed class MacPlatform : IPlatform
    {
        public string GetDefaultConfigPath()
        {
            const string subPath = $"Library/Application Support/{nameof(RetroBox)}";
            var path = Env.Resolve("HOME", subPath);
            return path!;
        }

        public string GetDefaultTempPath()
        {
            var path = Env.Resolve("TMPDIR");
            return path!;
        }

        public string GetDefaultHomePath()
        {
            var path = Env.Resolve("HOME");
            return path!;
        }
    }
}