using System.IO;
using RetroBox.Common;
using RetroBox.Fabric.Boxes;

namespace RetroBox.Manager.Boxes
{
    public static class MetaMachines
    {
        public static string? GetFolder(this MetaMachine machine)
            => Path.GetDirectoryName(machine.M?.File);

        public static MetaMachine Wrap((Machine src, Envelope env) tuple)
        {
            var (src, env) = tuple;
            return new MetaMachine { M = src, E = env };
        }

        public static byte[] Black { get; } = Resources.LoadBytes("black.png", typeof(MetaMachine));
    }
}