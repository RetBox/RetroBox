using System;
using System.Runtime.InteropServices;
using RetroBox.API.Xplat;
using RetroBox.Linux;
using RetroBox.Mac;
using RetroBox.Windows;

namespace RetroBox.Fabric
{
    public static class Platforms
    {
        public static IPlatform My { get; } = LoadPlatform();

        private static IPlatform LoadPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new WinPlatform();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return new MacPlatform();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                return new LinuxPlatform();

            throw new InvalidOperationException("Sorry, your OS is not supported!");
        }
    }
}