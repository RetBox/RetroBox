using RetroBox.Common;
using RetroBox.Common.Xplat;

namespace RetroBox.Linux
{
    internal sealed class LinuxFolders : CommonFolder
    {
        public override string GetDefaultConfigPath()
        {
            var path = Env.Resolve("XDG_CONFIG_HOME", nameof(RetroBox))
                       ?? Env.Resolve("HOME", $".config/{nameof(RetroBox)}");
            return path!;
        }

        public override string GetDefaultTempPath()
        {
            var path = Env.Resolve("TEMP") ?? "/tmp";
            return path;
        }

        public override string GetDefaultHomePath()
        {
            var path = Env.Resolve("HOME");
            return path!;
        }
    }
}