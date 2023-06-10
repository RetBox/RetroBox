using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ByteSizeLib;
using RetroBox.Common.Xplat;

namespace RetroBox.Linux
{
    internal sealed class LinuxInfos : CommonComputer
    {
        private static readonly long MemSize;
        private static readonly string PrettyName;

        static LinuxInfos()
        {
            var memInfo = ReadLines("/proc/meminfo", ':');
            var memTotal = memInfo["MemTotal"].Split(' ').First();
            MemSize = (long)ByteSize.FromKiloBytes(long.Parse(memTotal)).MegaBytes;

            var osRel = ReadLines("/etc/os-release", '=');
            PrettyName = osRel["PRETTY_NAME"];
        }

        internal static IDictionary<string, string> ReadLines(string file, char sep)
        {
            var dict = new Dictionary<string, string>();
            var lines = File.ReadLines(file, Encoding.UTF8);
            foreach (var line in lines)
            {
                var parts = line.Split(sep, 2);
                var key = parts[0].Trim();
                var val = parts[1].Trim().Trim('"').Trim();
                dict[key] = val;
            }
            return dict;
        }

        public override long HostMemory => MemSize;
        protected override string OSName => PrettyName;
    }
}