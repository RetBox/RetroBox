﻿using System.IO;

namespace RetroBox.Fabric.Config
{
    internal sealed class SimpleConfig : IConfig
    {
        public SimpleConfig()
        {
            var sys = Platforms.My.Folders;
            AppData = sys.GetDefaultConfigPath();
            HomePath = sys.GetDefaultHomePath();
            TempPath = sys.GetDefaultTempPath();
            ExePath = sys.GetCurrentExePath();
        }

        public string ExePath { get; set; }
        public string HomePath { get; set; }
        public string TempPath { get; set; }

        public string AppData { get; set; }
        public string EmuRoot => Path.Combine(AppData, "emu");
        public string RomRoot => Path.Combine(AppData, "rom");
        public string CacheRoot => Path.Combine(AppData, "cache");
        public string MachineRoot => Path.Combine(HomePath, $"{nameof(RetroBox)} VMs");
        public string TemplateRoot => Path.Combine(ExePath, "Templates");
    }
}