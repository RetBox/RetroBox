using System;
using System.Collections.Generic;
using System.IO;
using RetroBox.API.Data;
using RetroBox.Common;

namespace RetroBox.Fabric.Config
{
    public static class Configs
    {
        public static IConfig Default { get; } = new SimpleConfig();

        public const string FileName = "RetroBox.json";

        public static IGlobalConfig Global { get; private set; } = LoadGlobalConfig();

        private static IGlobalConfig LoadGlobalConfig()
        {
            var appDate = Default.AppData;
            var gConfigFile = Path.Combine(appDate, FileName);

            GlobalConfig env;
            if (File.Exists(gConfigFile))
            {
                env = Serials.ReadJsonFile<GlobalConfig>(gConfigFile)!;
            }
            else
            {
                env = new GlobalConfig();
                Serials.WriteJsonFile(env, gConfigFile);
            }

            if (ReloadPreparedEnv(env, Array.Empty<FoundExe>(), Array.Empty<FoundRom>())) // TODO
                Serials.WriteJsonFile(env, gConfigFile);

            return env;
        }

        // TODO
        public static void ReloadConfig() => Global = LoadGlobalConfig();

        private static bool ReloadPreparedEnv(GlobalConfig global,
            IEnumerable<FoundExe> exes, IEnumerable<FoundRom> roms)
        {
            var cfg = Default;
            var plat = Platforms.My;

            var exe = new Dictionary<string, EmuExe>();
            foreach (var file in exes)
            {
                var exeId = file.Version.ReleaseId!;
                exe[exeId] = new EmuExe(file.File, file.Version.FilePrivatePart);
            }
            global.InstalledEmu = exe;

            var rom = new Dictionary<string, string>();
            foreach (var file in roms)
            {
                var exeId = file.ReleaseId!;
                rom[exeId] = file.Path;
            }
            global.InstalledRom = rom;

            return true;
        }
    }
}