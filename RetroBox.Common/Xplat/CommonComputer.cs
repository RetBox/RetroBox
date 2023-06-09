using System.Net;
using RetroBox.API.Xplat;
using static System.Runtime.InteropServices.RuntimeInformation;

// ReSharper disable InconsistentNaming

namespace RetroBox.Common.Xplat
{
    public abstract class CommonComputer : IPlatComputer
    {
        public string HostName => Dns.GetHostName();

        public abstract long HostMemory { get; }

        protected virtual string OSName => OSDescription;
        public string HostOS => $"{OSName} ({OSArchitecture.ToString().ToLower()})";

        public string NetRuntime => $"{FrameworkDescription} ({ProcessArchitecture.ToString().ToLower()})";
    }
}