using System;
using RetroBox.API;

namespace RetroBox.Windows.Core
{
    internal sealed class HandleObj : IDisposable, IHasTag
    {
        public string? Tag { get; set; }

        public uint? VmId { get; set; }

        public IntPtr? HWnd { get; set; }

        public IntPtr? Client { get; set; }

        public void Dispose()
        {
            HWnd = null;
            Client = null;
        }
    }
}