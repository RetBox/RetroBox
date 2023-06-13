using System.Collections.Generic;

namespace RetroBox.Common.Xplat
{
    public class StartBoxArg
    {
        public string? Exe { get; set; }
        public string? Config { get; set; }
        public string? VmPath { get; set; }
        public string? RomPath { get; set; }
        public string? VmName { get; set; }
        public string? LogFile { get; set; }
        public bool Settings { get; set; }
        public bool FullScreen { get; set; }
        public IDictionary<string, string>? Vars { get; set; }
    }
}