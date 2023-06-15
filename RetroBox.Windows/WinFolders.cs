using RetroBox.Common;
using RetroBox.Common.Xplat;

namespace RetroBox.Windows
{
    internal sealed class WinFolders : CommonFolder
    {
        public override string GetDefaultConfigPath()
        {
            var path = Env.Resolve("LOCALAPPDATA", nameof(RetroBox));
            return path!;
        }

        public override string GetDefaultTempPath()
        {
            var path = Env.Resolve("TEMP");
            return path!;
        }

        public override string GetDefaultHomePath()
        {
            var path = Env.Resolve("USERPROFILE");
            return path!;
        }
    }
}