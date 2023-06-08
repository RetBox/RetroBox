using System.IO;

namespace RetroBox.Fabric
{
    internal sealed class SimpleConfig : IConfig
    {
        public SimpleConfig()
        {
            var sys = Platforms.Sys;
            AppData = sys.GetDefaultConfigPath();
            HomePath = sys.GetDefaultHomePath();
            TempPath = sys.GetDefaultTempPath();
        }

        public string HomePath { get; set; }
        public string TempPath { get; set; }

        public string AppData { get; set; }
        public string EmuRoot => Path.Combine(AppData, "emu");
        public string RomRoot => Path.Combine(AppData, "rom");
        public string CacheRoot => Path.Combine(AppData, "cache");
    }
}