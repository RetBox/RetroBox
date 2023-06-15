using System.IO;
using RetroBox.Common;

namespace RetroBox.Fabric.Config
{
    public static class Configs
    {
        public static IConfig Default { get; } = new SimpleConfig();

        public const string FileName = "RetroBox.json";

        public static IGlobalConfig Global { get; } = LoadGlobalConfig();

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
            return env;
        }
    }
}