using System;
using RetroBox.Common.Xplat;

namespace RetroBox.Mac
{
    internal sealed class MacInfos : CommonComputer
    {
        public override long HostMemory => throw new InvalidOperationException();
    }
}