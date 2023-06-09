using System;
using RetroBox.Common.Xplat;

namespace RetroBox.Linux
{
    internal sealed class LinuxInfos : CommonComputer
    {
        public override long HostMemory => throw new InvalidOperationException();
    }
}