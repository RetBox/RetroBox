using RetroBox.API.Xplat;
using RetroBox.Common;

namespace RetroBox.Linux
{
    internal sealed class LinuxFolders : IPlatFolder
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