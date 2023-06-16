using System.IO;
using RetroBox.Common;

namespace RetroBox.Fabric.Boxes
{
    public static class Envelopes
    {
        internal static (Machine src, Envelope env) GetEnvelope(Machine machine)
            => GetEnvelope(machine.File, machine);

        public const string FileName = "envelope.json";

        internal static (Machine src, Envelope env) GetEnvelope(string boxConfigFile, Machine src)
        {
            var boxConfigDir = Path.GetDirectoryName(boxConfigFile)!;
            var envConfigFile = Path.Combine(boxConfigDir, FileName);

            Envelope env;
            if (File.Exists(envConfigFile))
            {
                env = Serials.ReadJsonFile<Envelope>(envConfigFile)!;
                env.InternalFile = envConfigFile;
            }
            else
            {
                var boxLocalFile = Path.GetFileName(boxConfigFile);
                env = new Envelope
                {
                    File = boxLocalFile, Description = "", InternalFile = envConfigFile
                };
                env.Save();
            }
            return (src, env);
        }
    }
}