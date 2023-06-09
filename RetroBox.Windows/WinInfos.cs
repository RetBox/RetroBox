using System;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using ByteSizeLib;
using RetroBox.Common.Xplat;

#pragma warning disable CA1416

namespace RetroBox.Windows
{
    internal sealed class WinInfos : CommonComputer
    {
        private static readonly string? Caption;
        private static readonly long Memory;

        static WinInfos()
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            var manage = searcher.Get().OfType<ManagementBaseObject>().First();
            Caption = manage.Properties["Caption"].Value.ToString();

            var status = new MEMORYSTATUSEX();
            status.Init();
            if (!GlobalMemoryStatusEx(ref status))
                throw new Win32Exception();
            var winMem = status.ullTotalPhys;
            var tmpMem = ByteSize.FromBytes(winMem).MebiBytes;
            Memory = (long)Math.Round(tmpMem, MidpointRounding.ToPositiveInfinity);
        }

        protected override string OSName => Caption;
        public override long HostMemory => Memory;

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public void Init()
            {
                dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }
    }
}