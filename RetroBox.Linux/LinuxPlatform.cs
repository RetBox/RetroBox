using RetroBox.API;
using RetroBox.Common;

namespace RetroBox.Linux
{
    public sealed class LinuxPlatform : IPlatform
    {
        public string GetDefaultConfigPath()
        {
            var path = Env.Resolve("XDG_CONFIG_HOME", nameof(RetroBox))
                       ?? Env.Resolve("HOME", $".config/{nameof(RetroBox)}");
            return path!;
        }

        public string GetDefaultTempPath()
        {
            var path = Env.Resolve("TEMP") ?? "/tmp";
            return path;
        }

        public string GetDefaultHomePath()
        {
            var path = Env.Resolve("HOME");
            return path!;
        }
    }
}