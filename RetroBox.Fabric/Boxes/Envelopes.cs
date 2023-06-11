using System.IO;
using RetroBox.Common;

namespace RetroBox.Fabric.Boxes
{
    public static class Envelopes
    {
        internal static IMetaMachine GetEnvelope(Machine machine)
            => GetEnvelope(machine.File, machine);

        public const string FileName = "envelope.json";

        internal static IMetaMachine GetEnvelope(string boxConfigFile, Machine src)
        {
            var boxConfigDir = Path.GetDirectoryName(boxConfigFile)!;
            var envConfigFile = Path.Combine(boxConfigDir, FileName);

            Envelope env;
            if (File.Exists(envConfigFile))
            {
                env = Serials.ReadJsonFile<Envelope>(envConfigFile)!;
            }
            else
            {
                var boxLocalFile = Path.GetFileName(boxConfigFile);
                env = new Envelope { File = boxLocalFile, Description = "" };
                Serials.WriteJsonFile(env, envConfigFile);
            }
            return new MetaMachine(src, env);
        }
    }
}