using RetroBox.API.Xplat;
using RetroBox.Common;

namespace RetroBox.Windows
{
    public sealed class WinFolders : IPlatFolder
    {
        public string GetDefaultConfigPath()
        {
            var path = Env.Resolve("LOCALAPPDATA", nameof(RetroBox));
            return path!;
        }

        public string GetDefaultTempPath()
        {
            var path = Env.Resolve("TEMP");
            return path!;
        }

        public string GetDefaultHomePath()
        {
            var path = Env.Resolve("USERPROFILE");
            return path!;
        }
    }
}