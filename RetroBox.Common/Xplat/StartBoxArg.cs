using System.Collections.Generic;

namespace RetroBox.Common.Xplat
{
    public class StartBoxArg
    {
        public string? TempDir { get; set; }
        public string? CallId { get; set; }

        public string? ExeFile { get; set; }
        public string? WorkDir { get; set; }
        public IReadOnlyDictionary<string, string?>? Vars { get; set; }
        public IReadOnlyCollection<string>? ExtraArgs { get; set; }

        public string? Config { get; set; }
        public string? VmPath { get; set; }
        public string? RomPath { get; set; }
        public string? VmName { get; set; }
        public string? LogFile { get; set; }
        public bool Settings { get; set; }
        public bool FullScreen { get; set; }
    }
}