using RetroBox.API.Xplat;
using RetroBox.Common.Tools;

namespace RetroBox.Common.Xplat
{
    public abstract class CommonFolder : IPlatFolder
    {
        public abstract string GetDefaultConfigPath();
        public abstract string GetDefaultTempPath();
        public abstract string GetDefaultHomePath();

        public virtual string GetCurrentExePath() => Apps.GetExeFolder();
    }
}