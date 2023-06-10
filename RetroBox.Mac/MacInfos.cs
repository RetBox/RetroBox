using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using ByteSizeLib;
using RetroBox.Common.Xplat;

namespace RetroBox.Mac
{
    internal sealed class MacInfos : CommonComputer
    {
        private static long Memory;
        private static string Caption;

        static MacInfos()
        {
            var rawMem = GetCtlByName("hw.memsize");
            Memory = (long)ByteSize.FromBytes(rawMem).MebiBytes;

            var sys = ReadPlist("/System/Library/CoreServices/SystemVersion.plist");
            var pn = sys["ProductName"];
            var uv = sys["ProductUserVisibleVersion"];
            Caption = $"{pn} {uv}";
        }

        public override long HostMemory => Memory;
        protected override string OSName => Caption;

        internal static IDictionary<string,string> ReadPlist(string file)
        {
            var dict = new Dictionary<string, string>();
            var lines = File.ReadAllLines(file, Encoding.UTF8);
            var lastKey = string.Empty;
            foreach(var rawLine in lines)
            {
                var line = rawLine.Trim();
                var tmp = "<key>";
                if (line.StartsWith(tmp))
                {
                    lastKey = line.Split(tmp, 2)[1].Split('<', 2)[0];
                    continue;
                }
                tmp = "<string>";
                if (line.StartsWith(tmp))
                {
                    var text = line.Split(tmp, 2)[1].Split('<', 2)[0];
                    dict[lastKey] = text;
                }
            }
            return dict;
        }

        private static long GetCtlByName(string name)
        {
            var size = (IntPtr)IntPtr.Size;
            sysctlbyname(name, out var res, ref size, IntPtr.Zero, 0);
            return res.ToInt64();
        }

        [DllImport("libc")]
        private static extern int sysctlbyname([MarshalAs(UnmanagedType.LPStr)] string property,
            out IntPtr output, ref IntPtr oldLen, IntPtr newp, uint newlen);
    }
}