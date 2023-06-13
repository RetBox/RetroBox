using System.Collections.Generic;

namespace RetroBox.Fabric.Config
{
    public class GlobalConfig : IGlobalConfig
    {
        public Dictionary<string, EmuExe>? InstalledEmu { get; set; }
        
        public Dictionary<string, string>? InstalledRom { get; set; }
    }
}