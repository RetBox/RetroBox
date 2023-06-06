using RetroBox.API;
using RetroBox.Common;

namespace RetroBox.Windows
{
    public sealed class WinPlatform : IPlatform
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