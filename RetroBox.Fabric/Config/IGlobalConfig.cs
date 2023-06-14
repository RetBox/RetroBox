using System.Collections.Generic;

namespace RetroBox.Fabric.Config
{
    public interface IGlobalConfig
    {
        Dictionary<string, EmuExe>? InstalledEmu { get; }

        Dictionary<string, string>? InstalledRom { get; }
    }
}